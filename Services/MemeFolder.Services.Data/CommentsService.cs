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
    using MemeFolder.Web.ViewModels.Comments;

    public class CommentsService : ICommentsService
    {
        private readonly IRepository<Comment> commentsRepository;
        private readonly IRepository<MediaFile> mediaFilesRepository;
        private readonly IRepository<Like> likesRepository;

        public CommentsService(IRepository<Comment> commentsRepository, IRepository<MediaFile> mediaFilesRepository, IRepository<Like> likesRepository)
        {
            this.commentsRepository = commentsRepository;
            this.mediaFilesRepository = mediaFilesRepository;
            this.likesRepository = likesRepository;
        }

        public async Task CreateCommentAsync(BaseCommentInputModel input, string postId, string userId, string rootPath)
        {
            Comment comment = new Comment
            {
                PostId = postId,
                Text = input.Text,
                UserId = userId,
            };

            foreach (var inputMediaFile in input.MediaFiles)
            {
                comment.Media.Add(inputMediaFile);
                inputMediaFile.Comments.Add(comment);
            }

            await this.commentsRepository.AddAsync(comment);
            await this.commentsRepository.SaveChangesAsync();
            await this.mediaFilesRepository.SaveChangesAsync();
        }

        public async Task EditCommentAsync(string text, string commentId, string rootPath)
        {
            Comment comment = this.GetByIdWithTracking<Comment>(commentId);

            comment.Text = text;

            await this.commentsRepository.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(string commentId)
        {
            Comment comment = this.GetByIdWithTracking<Comment>(commentId);

            foreach (var commentLike in comment.Likes)
            {
                comment.Likes.Remove(commentLike);
                this.likesRepository.Delete(commentLike);

                await this.commentsRepository.SaveChangesAsync();
                await this.likesRepository.SaveChangesAsync();
            }

            foreach (var commentMediaFile in comment.Media)
            {
                comment.Media.Remove(commentMediaFile);
                commentMediaFile.Comments.Remove(comment);

                await this.commentsRepository.SaveChangesAsync();
                await this.mediaFilesRepository.SaveChangesAsync();
            }

            this.commentsRepository.Delete(comment);

            await this.commentsRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetAllPopularComments<T>(string postId, int page, int itemsPerPage = 50) =>
            this.commentsRepository
                .AllAsNoTracking()
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.Likes.Select(l => l.CreatedOn >= DateTime.UtcNow.AddDays(-1)).Count())
                .ThenByDescending(c => c.Likes.Count)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();

        public IEnumerable<T> GetAllNew<T>(string postId, int page, int itemsPerPage = 50) =>
            this.commentsRepository
                .AllAsNoTracking()
                .OrderBy(c => c.CreatedOn)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .To<T>()
                .ToList();

        public T GetByIdWithTracking<T>(string commentId) =>
            this.commentsRepository
                .All()
                .Where(c => c.Id == commentId)
                .To<T>()
                .FirstOrDefault();

        public async Task LikeComment(string commentId, string userId, ReactionType reaction)
        {
            Comment comment = this.GetByIdWithTracking<Comment>(commentId);

            Like like = new Like
            {
                CommentId = commentId,
                UserId = userId,
                Reaction = reaction,
                CreatedOn = DateTime.UtcNow,
            };

            await this.likesRepository.AddAsync(like);

            await this.likesRepository.SaveChangesAsync();
            await this.commentsRepository.SaveChangesAsync();
        }

        public async Task UpdateLike(string commentId, string userId, ReactionType reaction)
        {
            Like like = this.likesRepository.All().FirstOrDefault(x => x.UserId == userId && x.CommentId == commentId);
            Comment comment = this.GetByIdWithTracking<Comment>(commentId);

            if (reaction == ReactionType.None)
            {
                comment.Likes.Remove(like);

                this.likesRepository.Delete(like);

                await this.likesRepository.SaveChangesAsync();
                await this.commentsRepository.SaveChangesAsync();
            }
            else
            {
                like.Reaction = reaction;

                await this.likesRepository.SaveChangesAsync();
                await this.commentsRepository.SaveChangesAsync();
            }
        }
    }
}
