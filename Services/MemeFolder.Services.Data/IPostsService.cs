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
        Task<Post> CreatePostAsync(CreatePostInputModel input, string userId, string rootPath);

        Task RepostPostAsync(CreatePostInputModel input, string userId, string postId, string rootPath);

        Task EditPostAsync(string id, EditPostInputModel input, string rootPath);

        Task DeletePostAsync(string postId);

        IEnumerable<T> GetAllPopularPost<T>(int page, int itemsPerPage = 25);

        IEnumerable<T> GetAllNew<T>(int page, int itemsPerPage = 25);

        IEnumerable<T> GetAllPopularPostWithTags<T>(int page, ICollection<Tag> tags, int itemsPerPage = 25);

        IEnumerable<T> GetAllNewWithTags<T>(int page, ICollection<Tag> tags, int itemsPerPage = 25);

        T GetByIdWithTracking<T>(string postId);

        Task LikePost(string postId, string userId, ReactionType reaction);

        Task UpdateLike(string postId, string userId, ReactionType reaction);

        Task PostComment(Comment comment, string postId, string userId, string rootPath);

        Task RemoveMediaFilesFromPost(ICollection<MediaFile> mediaFiles, Post post);

        Task AddMediaFilesToPost(ICollection<MediaFile> mediaFiles, Post post);

        Task RemoveTagsFromPost(ICollection<Tag> tags, Post post);

        Task AddTagsToPost(ICollection<Tag> tags, Post post);
    }
}
