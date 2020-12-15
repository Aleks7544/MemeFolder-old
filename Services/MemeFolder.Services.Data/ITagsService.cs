namespace MemeFolder.Services.Data
{
    using System.Threading.Tasks;

    using MemeFolder.Data.Models;
    using MemeFolder.Web.ViewModels.Tags;

    public interface ITagsService
    {
        Task<Tag> CreateTagAsync(CreateTagInputModel input, string userId);

        Task AddTagToPost(Tag tag, Post post);

        Task AddMediaFileToTag(Tag tag, MediaFile mediaFile);
    }
}
