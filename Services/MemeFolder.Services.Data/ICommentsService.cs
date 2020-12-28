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
        Task CreateCommentAsync(BaseCommentInputModel input, string postId, string userId, string rootPath);

        Task EditCommentAsync(string text, string commentId, string rootPath);

        Task DeleteCommentAsync(string commentId);

        IEnumerable<T> GetAllPopularComments<T>(string postId, int page, int itemsPerPage = 50);

        IEnumerable<T> GetAllNew<T>(string postId, int page, int itemsPerPage = 50);

        T GetByIdWithTracking<T>(string commentId);

        Task LikeComment(string commentId, string userId, ReactionType reaction);

        Task UpdateLike(string commentId, string userId, ReactionType reaction);
    }
}
