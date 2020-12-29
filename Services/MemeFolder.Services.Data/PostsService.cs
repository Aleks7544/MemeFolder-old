namespace MemeFolder.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MemeFolder.Data.Common.Repositories;
    using MemeFolder.Data.Models;
    using MemeFolder.Data.Models.Enums;
    using MemeFolder.Services.Mapping;
    using MemeFolder.Web.ViewModels.Posts;

    public class PostsService : IPostsService
    {
        private readonly IDeletableEntityRepository<Post> postsRepository;
        private readonly IRepository<Like> likesRepository;
        private readonly IRepository<MediaFile> mediaFilesRepository;
        private readonly IDeletableEntityRepository<Comment> commentsRepository;
        private readonly IRepository<Tag> tagsRepository;

        public PostsService(IDeletableEntityRepository<Post> postsRepository, IRepository<Like> likesRepository, IRepository<MediaFile> mediaFilesRepository, IDeletableEntityRepository<Comment> commentsRepository, IRepository<Tag> tagsRepository)
        {
            this.postsRepository = postsRepository;
            this.likesRepository = likesRepository;
            this.mediaFilesRepository = mediaFilesRepository;
            this.commentsRepository = commentsRepository;
            this.tagsRepository = tagsRepository;
        }

        public async Task<Post> CreatePostAsync(CreatePostInputModel input, string userId, string rootPath)
        {
            var post = new Post
            {
                CreatedOn = DateTime.UtcNow,
                PosterId = userId,
            };

            await this.postsRepository.AddAsync(post);
            await this.postsRepository.SaveChangesAsync();

            return post;
        }

        public async Task RepostPostAsync(CreatePostInputModel input, string userId, string postId, string rootPath)
        {
            Post post = this.CreatePostAsync(input, userId, rootPath).GetAwaiter().GetResult();

            post.RepostedPostId = postId;

            await this.postsRepository.AddAsync(post);
            await this.postsRepository.SaveChangesAsync();
        }

        public async Task EditPostAsync(string id, EditPostInputModel input, string rootPath)
        {
            Post post = this.GetByIdWithTracking<Post>(id);

            post.Visibility = input.Visibility;
            post.Text = input.Text;

            await this.postsRepository.SaveChangesAsync();
        }

        public async Task DeletePostAsync(string postId)
        {
            Post post = this.GetByIdWithTracking<Post>(postId);

            foreach (var postLike in post.Likes)
            {
                post.Likes.Remove(postLike);

                this.likesRepository.Delete(postLike);

                await this.likesRepository.SaveChangesAsync();
            }

            foreach (var postMediaFile in post.MediaFiles)
            {
                post.MediaFiles.Remove(postMediaFile);

                postMediaFile.Posts.Remove(post);

                await this.mediaFilesRepository.SaveChangesAsync();
            }

            foreach (var postComment in post.Comments)
            {
                post.Comments.Remove(postComment);

                this.commentsRepository.Delete(postComment);

                await this.commentsRepository.SaveChangesAsync();
            }

            foreach (var postTag in post.Tags)
            {
                post.Tags.Remove(postTag);

                postTag.Posts.Remove(post);

                await this.tagsRepository.SaveChangesAsync();
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

        public IEnumerable<T> GetAllPopularPostWithTags<T>(int page, ICollection<Tag> tags, int itemsPerPage = 25) =>
            this.postsRepository
                .AllAsNoTracking()
                .Where(p => p.Tags.Any(t => tags.Any(x => x == t)))
                .OrderByDescending(p => p.Tags.Count(t => tags.Any(x => x == t)))
                .ThenByDescending(p => p.Likes.Select(l => l.CreatedOn >= DateTime.UtcNow.AddDays(-1)).Count())
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();

        public IEnumerable<T> GetAllNewWithTags<T>(int page, ICollection<Tag> tags, int itemsPerPage = 25) =>
            this.postsRepository
                .AllAsNoTracking()
                .Where(p => p.Tags.Any(t => tags.Any(x => x == t)))
                .OrderByDescending(p => p.Tags.Count(t => tags.Any(x => x == t)))
                .ThenBy(x => x.CreatedOn)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();

        public T GetByIdWithTracking<T>(string postId) =>
            this.postsRepository
                .All()
                .Where(p => p.Id == postId)
                .To<T>()
                .FirstOrDefault();

        public async Task LikePost(string postId, string userId, ReactionType reaction)
        {
            Post post = this.GetByIdWithTracking<Post>(postId);

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
            await this.likesRepository.SaveChangesAsync();
        }

        public async Task UpdateLike(string postId, string userId, ReactionType reaction)
        {
            Like like = this.likesRepository.All().FirstOrDefault(x => x.UserId == userId && x.PostId == postId);
            Post post = this.GetByIdWithTracking<Post>(postId);

            if (reaction == ReactionType.None)
            {
                post.Likes.Remove(like);

                this.likesRepository.Delete(like);

                await this.likesRepository.SaveChangesAsync();
                await this.postsRepository.SaveChangesAsync();
            }
            else
            {
                like.Reaction = reaction;

                await this.likesRepository.SaveChangesAsync();
                await this.postsRepository.SaveChangesAsync();
            }
        }

        public async Task PostComment(Comment comment, string postId, string userId, string rootPath)
        {
            Post post = this.GetByIdWithTracking<Post>(postId);

            post.Comments.Add(comment);

            await this.postsRepository.SaveChangesAsync();
        }

        public async Task AddTagsToPost(ICollection<Tag> tags, Post post)
        {
            foreach (var tag in tags)
            {
                post.Tags.Add(tag);

                tag.Posts.Add(post);
            }

            await this.postsRepository.SaveChangesAsync();
            await this.tagsRepository.SaveChangesAsync();
        }

        public async Task RemoveTagsFromPost(ICollection<Tag> tags, Post post)
        {
            foreach (var tag in tags)
            {
                post.Tags.Remove(tag);

                tag.Posts.Remove(post);
            }

            await this.postsRepository.SaveChangesAsync();
            await this.tagsRepository.SaveChangesAsync();
        }

        public async Task AddMediaFilesToPost(ICollection<MediaFile> mediaFiles, Post post)
        {
            foreach (var mediaFile in mediaFiles)
            {
                post.MediaFiles.Add(mediaFile);

                mediaFile.Posts.Add(post);
            }

            await this.postsRepository.SaveChangesAsync();
            await this.mediaFilesRepository.SaveChangesAsync();
        }

        public async Task RemoveMediaFilesFromPost(ICollection<MediaFile> mediaFiles, Post post)
        {
            foreach (var mediaFile in mediaFiles)
            {
                post.MediaFiles.Remove(mediaFile);

                mediaFile.Posts.Remove(post);
            }

            await this.postsRepository.SaveChangesAsync();
            await this.mediaFilesRepository.SaveChangesAsync();
        }
    }
}
