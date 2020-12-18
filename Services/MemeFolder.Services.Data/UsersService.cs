namespace MemeFolder.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using MemeFolder.Data.Common.Repositories;
    using MemeFolder.Data.Models;
    using MemeFolder.Data.Models.Enums;
    using MemeFolder.Services.Mapping;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Relationship> relationshipsRepository;

        public UsersService(IDeletableEntityRepository<ApplicationUser> usersRepository, IRepository<Relationship> relationshipsRepository)
        {
            this.usersRepository = usersRepository;
            this.relationshipsRepository = relationshipsRepository;
        }

        public async Task CreateRelationshipAsync(string firstUserId, string secondUserId, RelationshipStatus status)
        {
            Relationship relationship = new Relationship
            {
                FirstUserId = firstUserId,
                SecondUserId = secondUserId,
                Status = status,
            };

            await this.relationshipsRepository.AddAsync(relationship);

            ApplicationUser firstUser = this.GetUserById<ApplicationUser>(firstUserId);
            ApplicationUser secondUser = this.GetUserById<ApplicationUser>(secondUserId);

            firstUser.Relationships.Add(relationship);
            secondUser.Relationships.Add(relationship);

            await this.relationshipsRepository.SaveChangesAsync();
            await this.usersRepository.SaveChangesAsync();
        }

        public async Task UpdateRelationshipAsync(string firstUserId, string secondUserId, RelationshipStatus newStatus)
        {
            ApplicationUser firstUser = this.GetUserById<ApplicationUser>(firstUserId);
            ApplicationUser secondUser = this.GetUserById<ApplicationUser>(secondUserId);

            Relationship relationship = this.GetRelationshipById<Relationship>(firstUserId, secondUserId);

            if (newStatus == RelationshipStatus.None)
            {
                firstUser.Relationships.Remove(relationship);
                secondUser.Relationships.Remove(relationship);
            }
            else
            {
                Relationship requesterRelationship = firstUser.Relationships.FirstOrDefault(x => x == relationship);
                Relationship requestedUserRelationship = secondUser.Relationships.FirstOrDefault(x => x == relationship);

                requesterRelationship.Status = newStatus;
                requestedUserRelationship.Status = newStatus;
            }

            await this.relationshipsRepository.SaveChangesAsync();
            await this.usersRepository.SaveChangesAsync();
        }

        public async Task FollowUser(string followerId, string followedUserId)
        {
            ApplicationUser follower = this.GetUserById<ApplicationUser>(followerId);
            ApplicationUser followedUser = this.GetUserById<ApplicationUser>(followedUserId);

            follower.Following.Add(followedUser);
            followedUser.Followers.Add(follower);

            await this.usersRepository.SaveChangesAsync();
        }

        public async Task UnfollowUser(string firstUserId, string secondUserId)
        {
            ApplicationUser follower = this.GetUserById<ApplicationUser>(firstUserId);
            ApplicationUser followedUser = this.GetUserById<ApplicationUser>(secondUserId);

            follower.Following.Remove(followedUser);
            followedUser.Followers.Remove(follower);

            await this.usersRepository.SaveChangesAsync();
        }

        public async Task AddCollectionToUserCollection(string userId, Collection collection)
        {
            ApplicationUser user = this.GetUserById<ApplicationUser>(userId);

            user.Collections.Add(collection);

            await this.usersRepository.SaveChangesAsync();
        }

        public async Task RemoveCollectionFromUserCollection(string userId, Collection collection)
        {
            ApplicationUser user = this.GetUserById<ApplicationUser>(userId);

            user.Collections.Remove(collection);

            await this.usersRepository.SaveChangesAsync();
        }

        public T GetRelationshipById<T>(string firstUserId, string secondUserId) =>
            this.relationshipsRepository
                .All()
                .Where(r => r.FirstUserId == firstUserId && r.SecondUserId == secondUserId)
                .To<T>()
                .FirstOrDefault();

        public T GetUserById<T>(string userId) =>
            this.usersRepository
                .All()
                .Where(u => u.Id == userId)
                .To<T>()
                .FirstOrDefault();
    }
}
