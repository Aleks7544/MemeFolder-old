namespace MemeFolder.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MemeFolder.Data.Models;
    using MemeFolder.Data.Models.Enums;
    using MemeFolder.Web.ViewModels.Comments;
    using Microsoft.AspNetCore.Http;

    public interface ICommentsService
    {
        Task<Comment> CreateCommentAsync(CreateCommentInputModel input, string postId, string userId, string rootPath);

        Task EditCommentAsync(CreateCommentInputModel input, string commentId, string rootPath);

        Task DeleteCommentAsync(string commentId);

        IEnumerable<T> GetAllPopularComments<T>(string postId, int page, int itemsPerPage = 50);

        IEnumerable<T> GetAllNew<T>(string postId, int page, int itemsPerPage = 50);

        T GetById<T>(string commentId);

        Task LikeComment(string commentId, string userId, ReactionType reaction);

        Task UpdateLike(string commentId, string userId, ReactionType reaction);

        Task AddMediaFilesToComment(IEnumerable<IFormFile> mediaFiles, string userId, string postId, string rootPath, Comment comment);
    }
}
