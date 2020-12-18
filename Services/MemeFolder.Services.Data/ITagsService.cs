namespace MemeFolder.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MemeFolder.Data.Models;
    using MemeFolder.Web.ViewModels.Tags;

    public interface ITagsService
    {
        Task<Tag> CreateTagAsync(CreateTagInputModel input);

        Task AddPostToTagCollection(Tag tag, Post post);

        Task AddMediaFileToTagCollection(Tag tag, MediaFile mediaFile);

        Task RemovePostFromTagCollection(string tagId, Post post);

        Task RemoveMediaFileFromTagCollection(string tagId, MediaFile mediaFile);

        T GetById<T>(string id);
    }
}
