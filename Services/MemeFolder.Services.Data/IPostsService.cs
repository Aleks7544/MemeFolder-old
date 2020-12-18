namespace MemeFolder.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MemeFolder.Data.Models;
    using MemeFolder.Data.Models.Enums;
    using MemeFolder.Web.ViewModels.Comments;
    using MemeFolder.Web.ViewModels.Posts;
    using MemeFolder.Web.ViewModels.Tags;
    using Microsoft.AspNetCore.Http;

    public interface IPostsService
    {
        Task<string> CreatePostAsync(CreatePostInputModel input, string userId, string rootPath);

        Task<string> RepostPostAsync(CreatePostInputModel input, string userId, string postId, string rootPath);

        Task<string> EditPostAsync(string id, EditPostInputModel input, string rootPath);

        Task DeletePostAsync(string postId);

        IEnumerable<T> GetAllPopularPost<T>(int page, int itemsPerPage = 25);

        IEnumerable<T> GetAllNew<T>(int page, int itemsPerPage = 25);

        IEnumerable<T> GetAllPopularPostWithTags<T>(int page, IEnumerable<string> tagsIds, int itemsPerPage = 25);

        IEnumerable<T> GetAllNewWithTags<T>(int page, IEnumerable<string> tagsIds, int itemsPerPage = 25);

        T GetById<T>(string postId);

        Task LikePost(string postId, string userId, ReactionType reaction);

        Task UpdateLike(string postId, string userId, ReactionType reaction);

        Task PostComment(CreateCommentInputModel input, string postId, string userId, string rootPath);

        Task AddMediaFilesToPost(IEnumerable<IFormFile> mediaFiles, string userId, string rootPath, Post post);

        void AddTagsToPost(IEnumerable<CreateTagInputModel> tags, Post post);
    }
}
