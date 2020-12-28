namespace MemeFolder.Services.Data
{
    using System.Threading.Tasks;
    using MemeFolder.Data.Models;
    using MemeFolder.Web.ViewModels.Collections;

    public interface ICollectionsService
    {
        Task CreateCollectionAsync(CollectionInputModel input, string userId, string rootPath);

        Task UpdateCollectionAsync(CollectionInputModel input, string collectionId, string userId, string rootPath);

        Task DeleteCollectionAsync(string collectionId);

        Task AddMediaFileToCollectionAsync(string collectionId, MediaFile mediaFile);

        Task RemoveMediaFileFromCollectionAsync(string collectionId, MediaFile mediaFile);

        Task AddPostToCollectionAsync(string collectionId, Post post);

        Task RemovePostFromCollectionAsync(string collectionId, Post post);

        Task AddTagToCollectionAsync(string collectionId, Tag tag);

        Task RemoveTagFromCollectionAsync(string collectionId, Tag tag);

        Task AddFollowerToCollectionAsync(string collectionId, ApplicationUser user);

        Task RemoveFollowerFromCollectionAsync(string collectionId, ApplicationUser user);

        T GetById<T>(string collectionId);
    }
}
