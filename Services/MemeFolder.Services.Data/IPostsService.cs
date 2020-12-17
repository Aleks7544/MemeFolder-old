﻿namespace MemeFolder.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MemeFolder.Data.Models;
    using MemeFolder.Data.Models.Enums;
    using MemeFolder.Web.ViewModels.Posts;
    using Microsoft.AspNetCore.Http;
    using Web.ViewModels.Tags;

    public interface IPostsService
    {
        Task CreatePostAsync(CreatePostInputModel input, string userId);

        Task RepostPostAsync(RepostPostInputModel input, string userId);

        Task EditPostAsync(string id, EditPostInputModel input);

        IEnumerable<T> GetAllPopularPost<T>(int page, int itemsPerPage = 25);

        IEnumerable<T> GetAllNew<T>(int page, int itemsPerPage = 25);

        T GetById<T>(string id);

        Task LikePost(string id, string userId, ReactionType reaction);

        Task PostComment(CreateCommentInput input, string id, string userId);

        Task AddFollower(string id, string userId);

        Task AddMediaFiles(IEnumerable<IFormFile> mediaFiles, string userId, string rootPath, Post post);

        void AddTags(IEnumerable<CreateTagInputModel> tags, Post post);
    }
}
