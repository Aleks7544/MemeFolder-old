namespace MemeFolder.Services.Data
{
    using System.Threading.Tasks;

    using MemeFolder.Data.Models;
    using MemeFolder.Data.Models.Enums;

    public interface IUsersService
    {
        Task CreateRelationshipAsync(string firstUserId, string secondUserId, RelationshipStatus status);

        Task UpdateRelationshipAsync(string firstUserId, string secondUserId, RelationshipStatus status);

        Task FollowUser(string followerId, string followedUserId);

        Task UnfollowUser(string firstUserId, string secondUserId);

        Task AddCollectionToUserCollection(string userId, Collection collection);

        Task RemoveCollectionFromUserCollection(string userId, Collection collection);

        T GetRelationshipById<T>(string firstUserId, string secondUserId);

        T GetUserById<T>(string userId);
    }
}
