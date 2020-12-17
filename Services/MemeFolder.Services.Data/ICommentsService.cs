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
        Task<Comment> CreateCommentAsync(CreateCommentInputModel input, string postId, string userId);

        Task EditCommentAsync(CreateCommentInputModel input, string postId, string userId);

        Task DeleteCommentAsync(string id);

        IEnumerable<T> GetAllPopularComments<T>(int page, int itemsPerPage = 100);

        IEnumerable<T> GetAllNew<T>(int page, int itemsPerPage = 100);

        T GetById<T>(string id);

        Task LikeComment(string id, string userId, ReactionType reaction);

        Task AddMediaFilesToComment(IEnumerable<IFormFile> mediaFiles, string userId, string postId, string rootPath, Comment comment);
    }
}
