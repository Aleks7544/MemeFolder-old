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
    using MemeFolder.Web.ViewModels.Posts;
    using MemeFolder.Web.ViewModels.MediaFiles;

    public class PostsService : IPostsService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "png", "gif", "jpeg", "mp4", "mp3", "wav", "ogg", "mp4" };
        private readonly IDeletableEntityRepository<Post> postsRepository;
        private readonly IRepository<Tag> tagsRepository;

        private readonly ITagsService tagsService;
        private readonly IMediaFilesService mediaFilesService;

        public PostsService(IDeletableEntityRepository<Post> postsRepository, IRepository<Tag> tagsRepository, ITagsService tagsService, IMediaFilesService mediaFilesService)
        {
            this.postsRepository = postsRepository;
            this.tagsRepository = tagsRepository;

            this.tagsService = tagsService;
            this.mediaFilesService = mediaFilesService;
        }

        public async Task CreateAsync(CreatePostInputModel input, string userId, string rootPath)
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

            foreach (var tagModel in input.Tags)
            {
                Tag tag = this.tagsRepository.All().FirstOrDefault(x => x.Id == tagModel.Id) ?? this.tagsService.CreateTagAsync(tagModel, userId).GetAwaiter().GetResult();

                post.Tags.Add(tag);
                tag.Posts.Add(post);
            }

            foreach (var mediaFileInput in input.MediaFiles)
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
                    await this.tagsService.AddMediaFileToTag(postTag, mediaFile);
                }
            }

            await this.postsRepository.AddAsync(post);
            await this.postsRepository.SaveChangesAsync();
        }

        public async Task EditAsync(string id, EditPostInputModel input)
        {
            Post post = this.postsRepository.All().FirstOrDefault(x => x.Id == id);

            post.Visibility = input.Visibility;
            post.Text = input.Text;

            if (post.)
            {
                
            }
        }

        public IEnumerable<T> GetAllPopular<T>(int page, int itemsPerPage = 25)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetAllNew<T>(int page, int itemsPerPage = 25)
        {
            throw new System.NotImplementedException();
        }

        public T GetById<T>(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task LikePost(string id, string userId, ReactionType reaction)
        {
            throw new System.NotImplementedException();
        }

        public Task PostComment(CreateCommentInput input, string id, string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task AddFollower(string id, string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task AddTag(string id, string tagId)
        {
            throw new System.NotImplementedException();
        }

        public Task CreatePostAsync(CreatePostInputModel input, string userId)
        {
            throw new NotImplementedException();
        }

        public Task RepostPostAsync(RepostPostInputModel input, string userId)
        {
            throw new NotImplementedException();
        }

        public Task EditPostAsync(EditPostInputModel input)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAllPopularPost<T>(int page, int itemsPerPage = 25)
        {
            throw new NotImplementedException();
        }
    }
}
