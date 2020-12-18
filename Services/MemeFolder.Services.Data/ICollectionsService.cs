namespace MemeFolder.Services.Data
{
    using System.Threading.Tasks;

    using MemeFolder.Web.ViewModels.Collections;

    public interface ICollectionsService
    {
        Task CreateCollectionAsync(BaseCollectionInputModel input, string userId, string rootPath);

        Task UpdateCollectionAsync(EditCollectionInputModel input, string collectionId, string userId, string rootPath);

        Task DeleteCollectionAsync(string collectionId);

        Task FollowCollectionAsync(string collectionId, string userId);

        Task UnfollowCollectionAsync(string collectionId, string userId);

        T GetById<T>(string collectionId);
    }
}
