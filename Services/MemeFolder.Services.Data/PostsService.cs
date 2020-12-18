namespace MemeFolder.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MemeFolder.Data.Common.Repositories;
    using MemeFolder.Data.Models;
    using MemeFolder.Data.Models.Enums;
    using MemeFolder.Services.Mapping;
    using MemeFolder.Web.ViewModels.Comments;
    using MemeFolder.Web.ViewModels.MediaFiles;
    using MemeFolder.Web.ViewModels.Posts;
    using MemeFolder.Web.ViewModels.Tags;

    public class PostsService : IPostsService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "png", "gif", "jpeg", "mp4", "mp3", "wav", "ogg", "mp4" };

        private readonly IDeletableEntityRepository<Post> postsRepository;
        private readonly IRepository<Tag> tagsRepository;
        private readonly IRepository<Like> likesRepository;

        private readonly ITagsService tagsService;
        private readonly IMediaFilesService mediaFilesService;
        private readonly ICommentsService commentsService;

        public PostsService(IDeletableEntityRepository<Post> postsRepository, IRepository<Tag> tagsRepository, IRepository<Like> likesRepository, ITagsService tagsService, IMediaFilesService mediaFilesService, ICommentsService commentsService)
        {
            this.postsRepository = postsRepository;
            this.tagsRepository = tagsRepository;
            this.likesRepository = likesRepository;

            this.tagsService = tagsService;
            this.mediaFilesService = mediaFilesService;
            this.commentsService = commentsService;
        }

        public async Task<string> CreatePostAsync(CreatePostInputModel input, string userId, string rootPath)
        {
            foreach (var inputMediaFile in input.MediaFiles)
            {
                string extension = Path.GetExtension(inputMediaFile.FileName).TrimStart('.');

                if (!this.allowedExtensions.Any(x => extension.EndsWith(x)))
                {
                    throw new Exception($"Invalid or unsupported file extension {extension}");
                }
            }

            var post = new Post
            {
                CreatedOn = DateTime.UtcNow,
                PosterId = userId,
            };

            this.AddTagsToPost(input.Tags, post);

            await this.AddMediaFilesToPost(input.MediaFiles, userId, rootPath, post);

            foreach (var tag in post.Tags)
            {
                await this.tagsService.AddPostToTagCollection(tag, post);
            }

            await this.postsRepository.AddAsync(post);
            await this.postsRepository.SaveChangesAsync();

            return post.Id;
        }

        public async Task<string> RepostPostAsync(CreatePostInputModel input, string userId, string postId, string rootPath)
        {
            Post post = this.GetById<Post>(this.CreatePostAsync(input, userId, rootPath).GetAwaiter().GetResult());

            post.RepostedPostId = postId;

            await this.postsRepository.AddAsync(post);
            await this.postsRepository.SaveChangesAsync();

            return post.Id;
        }

        public async Task<string> EditPostAsync(string id, EditPostInputModel input, string rootPath)
        {
            Post post = this.GetById<Post>(id);

            post.Visibility = input.Visibility;
            post.Text = input.Text;

            this.AddTagsToPost(input.NewTags, post);
            await this.AddMediaFilesToPost(input.NewMediaFiles, post.PosterId, rootPath, post);

            foreach (var inputRemovedTag in input.RemovedTags)
            {
                Tag tag = this.tagsService.GetById<Tag>(inputRemovedTag);

                post.Tags.Remove(tag);

                foreach (var postMediaFile in post.MediaFiles)
                {
                    await this.tagsService.RemoveMediaFileFromTagCollection(tag.Id, postMediaFile);
                }

                await this.tagsService.RemovePostFromTagCollection(inputRemovedTag, post);
            }

            foreach (var inputRemovedMediaFile in input.RemovedMediaFiles)
            {
                post.MediaFiles.Remove(this.mediaFilesService.GetById<MediaFile>(inputRemovedMediaFile));

                await this.mediaFilesService.RemovePostFromMediaFile(inputRemovedMediaFile, post);
            }

            await this.postsRepository.SaveChangesAsync();

            return post.Id;
        }

        public async Task DeletePostAsync(string postId)
        {
            Post post = this.GetById<Post>(postId);

            foreach (var postLike in post.Likes)
            {
                await this.UpdateLike(postLike.PostId, postLike.UserId, ReactionType.None);
            }

            foreach (var postMediaFile in post.MediaFiles)
            {
                await this.mediaFilesService.RemovePostFromMediaFile(postMediaFile.Id, post);
            }

            foreach (var postComment in post.Comments)
            {
                await this.commentsService.DeleteCommentAsync(postComment.Id);
            }

            foreach (var postTag in post.Tags)
            {
                await this.tagsService.RemovePostFromTagCollection(postTag.Id, post);
            }

            this.postsRepository.Delete(post);

            await this.postsRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetAllPopularPost<T>(int page, int itemsPerPage = 25) =>
            this.postsRepository
                .AllAsNoTracking()
                .OrderByDescending(p => p.Likes.Select(l => l.CreatedOn >= DateTime.UtcNow.AddDays(-1)).Count())
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();

        public IEnumerable<T> GetAllNew<T>(int page, int itemsPerPage = 25) =>
            this.postsRepository
                .AllAsNoTracking()
                .OrderBy(x => x.CreatedOn)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();

        public IEnumerable<T> GetAllPopularPostWithTags<T>(int page, IEnumerable<string> tagsIds, int itemsPerPage = 25)
        {
            List<Tag> tags = new List<Tag>();

            foreach (var tagId in tagsIds)
            {
                tags.Add(this.tagsService.GetById<Tag>(tagId));
            }

            return this.postsRepository
                .AllAsNoTracking()
                .Where(p => p.Tags.Any(t => tags.Any(x => x == t)))
                .OrderByDescending(p => p.Tags.Count(t => tags.Any(x => x == t)))
                .ThenByDescending(p => p.Likes.Select(l => l.CreatedOn >= DateTime.UtcNow.AddDays(-1)).Count())
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();
        }

        public IEnumerable<T> GetAllNewWithTags<T>(int page, IEnumerable<string> tagsIds, int itemsPerPage = 25)
        {
            List<Tag> tags = new List<Tag>();

            foreach (var tagId in tagsIds)
            {
                tags.Add(this.tagsService.GetById<Tag>(tagId));
            }

            return this.postsRepository
                .AllAsNoTracking()
                .Where(p => p.Tags.Any(t => tags.Any(x => x == t)))
                .OrderByDescending(p => p.Tags.Count(t => tags.Any(x => x == t)))
                .ThenBy(x => x.CreatedOn)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();
        }

        public T GetById<T>(string postId) =>
            this.postsRepository
                .AllAsNoTracking()
                .Where(p => p.Id == postId)
                .To<T>()
                .FirstOrDefault();

        public async Task LikePost(string postId, string userId, ReactionType reaction)
        {
            Post post = this.GetById<Post>(postId);

            Like like = new Like
            {
                PostId = postId,
                Reaction = reaction,
                UserId = userId,
                CreatedOn = DateTime.UtcNow,
            };

            post.Likes.Add(like);

            await this.likesRepository.AddAsync(like);
            await this.postsRepository.SaveChangesAsync();
        }

        public async Task UpdateLike(string postId, string userId, ReactionType reaction)
        {
            Like like = this.likesRepository.All().FirstOrDefault(x => x.UserId == userId && x.PostId == postId);
            Post post = this.GetById<Post>(postId);

            if (reaction == ReactionType.None)
            {
                post.Likes.Remove(like);

                this.likesRepository.Delete(like);
            }
            else
            {
                like.Reaction = reaction;

                await this.likesRepository.SaveChangesAsync();
            }
        }

        public async Task PostComment(CreateCommentInputModel input, string postId, string userId, string rootPath)
        {
            Comment comment = this.commentsService.CreateCommentAsync(input, postId, userId, rootPath).GetAwaiter().GetResult();

            Post post = this.GetById<Post>(postId);

            post.Comments.Add(comment);

            await this.postsRepository.SaveChangesAsync();
        }

        public void AddTagsToPost(IEnumerable<CreateTagInputModel> tags, Post post)
        {
            foreach (var tagModel in tags)
            {
                Tag tag = this.tagsRepository.All().FirstOrDefault(x => x.Id == tagModel.Id) ?? this.tagsService.CreateTagAsync(tagModel).GetAwaiter().GetResult();

                post.Tags.Add(tag);
                tag.Posts.Add(post);
            }
        }

        public async Task AddMediaFilesToPost(IEnumerable<Microsoft.AspNetCore.Http.IFormFile> mediaFiles, string userId, string rootPath, Post post)
        {
            foreach (var mediaFileInput in mediaFiles)
            {
                CreateMediaFileInputModel mediaFileInputModel = new CreateMediaFileInputModel
                {
                    Extension = Path.GetExtension(mediaFileInput.FileName).TrimStart('.'),
                    RootPath = rootPath,
                    MediaFile = mediaFileInput,
                };

                MediaFile mediaFile = this.mediaFilesService.CreateMediaFile(mediaFileInputModel, userId).GetAwaiter().GetResult();

                post.MediaFiles.Add(mediaFile);

                await this.mediaFilesService.AddPostToMediaFile(mediaFile, post);

                foreach (var postTag in post.Tags)
                {
                    await this.tagsService.AddMediaFileToTagCollection(postTag, mediaFile);
                }
            }
        }
    }
}
