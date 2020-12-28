namespace MemeFolder.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MemeFolder.Data.Common.Repositories;
    using MemeFolder.Data.Models;
    using MemeFolder.Services.Mapping;
    using MemeFolder.Web.ViewModels.Collections;

    public class CollectionsService : ICollectionsService
    {
        private readonly IDeletableEntityRepository<Collection> collectionsRepository;
        private readonly IRepository<MediaFile> mediaFilesRepository;
        private readonly IRepository<Tag> tagsRepository;
        private readonly IDeletableEntityRepository<Post> postsRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;

        public CollectionsService(IDeletableEntityRepository<Collection> collectionsRepository, IRepository<MediaFile> mediaFilesRepository, IRepository<Tag> tagsRepository, IDeletableEntityRepository<Post> postsRepository, IDeletableEntityRepository<ApplicationUser> usersRepository)
        {
            this.collectionsRepository = collectionsRepository;
            this.mediaFilesRepository = mediaFilesRepository;
            this.tagsRepository = tagsRepository;
            this.postsRepository = postsRepository;
            this.usersRepository = usersRepository;
        }

        public async Task CreateCollectionAsync(CollectionInputModel input, string userId, string rootPath)
        {
            Collection collection = new Collection
            {
                CreatorId = userId,
                Description = input.Description,
                Visibility = input.Visibility,
                Name = input.Name,
                CreatedOn = DateTime.UtcNow,
            };

            await this.collectionsRepository.AddAsync(collection);
            await this.collectionsRepository.SaveChangesAsync();
        }

        public async Task UpdateCollectionAsync(CollectionInputModel input, string collectionId, string userId, string rootPath)
        {
            Collection collection = this.GetByIdWithTracking<Collection>(collectionId);

            collection.Name = input.Name;
            collection.Description = input.Description;
            collection.Visibility = input.Visibility;

            await this.collectionsRepository.SaveChangesAsync();
        }

        public async Task DeleteCollectionAsync(string collectionId)
        {
            Collection collection = this.GetByIdWithTracking<Collection>(collectionId);

            foreach (var collectionMediaFile in collection.MediaFiles)
            {
                collectionMediaFile.Collections.Remove(collection);
            }

            foreach (var collectionPost in collection.Posts)
            {
                collectionPost.Collections.Remove(collection);
            }

            foreach (var collectionFollower in collection.Followers)
            {
                collectionFollower.Collections.Remove(collection);
            }

            foreach (var collectionTag in collection.Tags)
            {
                collectionTag.Collections.Remove(collection);
            }

            this.collectionsRepository.Delete(collection);

            await this.collectionsRepository.SaveChangesAsync();
        }

        public async Task AddMediaFileToCollectionAsync(string collectionId, MediaFile mediaFile)
        {
            Collection collection = this.GetByIdWithTracking<Collection>(collectionId);

            collection.MediaFiles.Add(mediaFile);
            mediaFile.Collections.Add(collection);

            await this.collectionsRepository.SaveChangesAsync();
            await this.mediaFilesRepository.SaveChangesAsync();
        }

        public async Task RemoveMediaFileFromCollectionAsync(string collectionId, MediaFile mediaFile)
        {
            Collection collection = this.GetByIdWithTracking<Collection>(collectionId);

            collection.MediaFiles.Remove(mediaFile);
            mediaFile.Collections.Remove(collection);

            await this.collectionsRepository.SaveChangesAsync();
            await this.mediaFilesRepository.SaveChangesAsync();
        }

        public async Task AddPostToCollectionAsync(string collectionId, Post post)
        {
            Collection collection = this.GetByIdWithTracking<Collection>(collectionId);

            collection.Posts.Add(post);
            post.Collections.Add(collection);

            await this.collectionsRepository.SaveChangesAsync();
            await this.postsRepository.SaveChangesAsync();
        }

        public async Task RemovePostFromCollectionAsync(string collectionId, Post post)
        {
            Collection collection = this.GetByIdWithTracking<Collection>(collectionId);

            collection.Posts.Remove(post);
            post.Collections.Remove(collection);

            await this.collectionsRepository.SaveChangesAsync();
            await this.postsRepository.SaveChangesAsync();
        }

        public async Task AddTagToCollectionAsync(string collectionId, Tag tag)
        {
            Collection collection = this.GetByIdWithTracking<Collection>(collectionId);

            collection.Tags.Add(tag);
            tag.Collections.Add(collection);

            await this.collectionsRepository.SaveChangesAsync();
            await this.tagsRepository.SaveChangesAsync();
        }

        public async Task RemoveTagFromCollectionAsync(string collectionId, Tag tag)
        {
            Collection collection = this.GetByIdWithTracking<Collection>(collectionId);

            collection.Tags.Remove(tag);
            tag.Collections.Remove(collection);

            await this.collectionsRepository.SaveChangesAsync();
            await this.tagsRepository.SaveChangesAsync();
        }

        public async Task AddFollowerToCollectionAsync(string collectionId, ApplicationUser user)
        {
            Collection collection = this.GetByIdWithTracking<Collection>(collectionId);

            collection.Followers.Add(user);
            user.Collections.Add(collection);

            await this.collectionsRepository.SaveChangesAsync();
            await this.usersRepository.SaveChangesAsync();
        }

        public async Task RemoveFollowerFromCollectionAsync(string collectionId, ApplicationUser user)
        {
            Collection collection = this.GetByIdWithTracking<Collection>(collectionId);

            collection.Followers.Remove(user);
            user.Collections.Remove(collection);

            await this.collectionsRepository.SaveChangesAsync();
            await this.usersRepository.SaveChangesAsync();
        }

        public T GetByIdWithTracking<T>(string collectionId) =>
            this.collectionsRepository
                .All()
                .Where(p => p.Id == collectionId)
                .To<T>()
                .FirstOrDefault();
    }
}
