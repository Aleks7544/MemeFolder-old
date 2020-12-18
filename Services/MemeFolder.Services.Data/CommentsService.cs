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
    using Microsoft.AspNetCore.Http;

    public class CommentsService : ICommentsService
    {
        private readonly IRepository<Comment> commentsRepository;
        private readonly IRepository<Like> likesRepository;

        private readonly IMediaFilesService mediaFilesService;

        public CommentsService(IRepository<Comment> commentsRepository, IRepository<Like> likesRepository, IDeletableEntityRepository<ApplicationUser> usersRepository, IMediaFilesService mediaFilesService, IPostsService postsService, IUsersService usersService)
        {
            this.commentsRepository = commentsRepository;
            this.likesRepository = likesRepository;

            this.mediaFilesService = mediaFilesService;;
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

        public async Task EditCommentAsync(CreateCommentInputModel input, string commentId, string rootPath)
        {
            Comment comment = this.GetById<Comment>(commentId);

            comment.Text = input.Text;

            await this.AddMediaFilesToComment(input.MediaFiles, comment.UserId, comment.PostId, rootPath, comment);

            await this.commentsRepository.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(string commentId)
        {
            Comment comment = this.GetById<Comment>(commentId);

            foreach (var commentLike in comment.Likes)
            {
                await this.UpdateLike(commentLike.PostId, commentLike.UserId, ReactionType.None);
            }

            foreach (var commentMediaFile in comment.Media)
            {
                await this.mediaFilesService.RemoveCommentFromMediaFile(commentMediaFile.Id, comment);
            }

            this.commentsRepository.Delete(comment);

            await this.commentsRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetAllPopularComments<T>(string postId, int page, int itemsPerPage = 50) =>
            this.commentsRepository
                .All()
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.Likes.Select(l => l.CreatedOn >= DateTime.UtcNow.AddDays(-1)).Count())
                .ThenByDescending(c => c.Likes.Count)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();

        public IEnumerable<T> GetAllNew<T>(string postId, int page, int itemsPerPage = 50) =>
            this.commentsRepository
                .All()
                .OrderBy(c => c.CreatedOn)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();

        public T GetById<T>(string commentId) =>
            this.commentsRepository
                .All()
                .Where(c => c.Id == commentId)
                .To<T>()
                .FirstOrDefault();

        public async Task LikeComment(string commentId, string userId, ReactionType reaction)
        {
            Comment comment = this.GetById<Comment>(commentId);

            Like like = new Like
            {
                CommentId = commentId,
                UserId = userId,
                Reaction = reaction,
                CreatedOn = DateTime.UtcNow,
            };

            await this.likesRepository.AddAsync(like);
            await this.likesRepository.SaveChangesAsync();
        }

        public async Task UpdateLike(string commentId, string userId, ReactionType reaction)
        {
            Like like = this.likesRepository.All().FirstOrDefault(x => x.UserId == userId && x.CommentId == commentId);
            Comment comment = this.GetById<Comment>(commentId);

            if (reaction == ReactionType.None)
            {
                comment.Likes.Remove(like);

                this.likesRepository.Delete(like);
            }
            else
            {
                like.Reaction = reaction;

                await this.likesRepository.SaveChangesAsync();
            }
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
