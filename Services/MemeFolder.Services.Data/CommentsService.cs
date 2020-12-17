namespace MemeFolder.Services.Data
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Mapping;
    using MemeFolder.Data.Common.Repositories;
    using MemeFolder.Data.Models;
    using MemeFolder.Data.Models.Enums;
    using MemeFolder.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Http;
    using MemeFolder.Web.ViewModels.MediaFiles;

    public class CommentsService : ICommentsService
    {
        private readonly IRepository<Comment> commentsRepository;

        private readonly IMediaFilesService mediaFilesService;

        public CommentsService(IRepository<Comment> commentsRepository, IMediaFilesService mediaFilesService)
        {
            this.commentsRepository = commentsRepository;
            this.mediaFilesService = mediaFilesService;
        }

        public async Task<Comment> CreateCommentAsync(CreateCommentInputModel input, string postId, string userId, string rootPath)
        {
            Comment comment = new Comment
            {
                PostId = postId,
                Text = input.Text,
                UserId = userId,
            };

            await this.AddMediaFilesToComment(input.MediaFiles, userId, postId, rootPath, comment);

            await this.commentsRepository.AddAsync(comment);
            await this.commentsRepository.SaveChangesAsync();

            return comment;
        }

        public Task EditCommentAsync(CreateCommentInputModel input, string postId, string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteCommentAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetAllPopularComments<T>(int page, int itemsPerPage = 100)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetAllNew<T>(int page, int itemsPerPage = 100)
        {
            throw new System.NotImplementedException();
        }

        public T GetById<T>(string id) =>
            this.commentsRepository
                .AllAsNoTracking()
                .Where(c => c.Id == id)
                .To<T>()
                .FirstOrDefault();

        public Task LikeComment(string id, string userId, ReactionType reaction)
        {
            throw new System.NotImplementedException();
        }

        public async Task AddMediaFilesToComment(IEnumerable<IFormFile> mediaFiles, string userId, string postId, string rootPath, Comment comment)
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

                comment.Media.Add(mediaFile);

                await this.mediaFilesService.AddCommentToMediaFile(mediaFile, comment);
            }
        }
    }
}
