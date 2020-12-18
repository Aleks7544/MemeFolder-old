namespace MemeFolder.Services.Data
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MemeFolder.Data.Common.Repositories;
    using MemeFolder.Data.Models;
    using MemeFolder.Services.Mapping;
    using MemeFolder.Web.ViewModels.Collections;
    using MemeFolder.Web.ViewModels.MediaFiles;

    public class CollectionsService : ICollectionsService
    {
        private readonly IDeletableEntityRepository<Collection> collectionsRepository;

        private readonly IUsersService usersService;
        private readonly IPostsService postsService;
        private readonly IMediaFilesService mediaFilesService;


        public CollectionsService(IDeletableEntityRepository<Collection> collectionsRepository, IUsersService usersService, IPostsService postsService, IMediaFilesService mediaFilesService)
        {
            this.collectionsRepository = collectionsRepository;

            this.usersService = usersService;
            this.postsService = postsService;
            this.mediaFilesService = mediaFilesService;
        }

        public async Task CreateCollectionAsync(BaseCollectionInputModel input, string userId, string rootPath)
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

            await this.usersService.AddCollectionToUserCollection(userId, collection);

            await this.collectionsRepository.SaveChangesAsync();
        }

        public async Task UpdateCollectionAsync(EditCollectionInputModel input, string collectionId, string userId, string rootPath)
        {
            Collection collection = this.GetById<Collection>(collectionId);

            collection.Name = input.Name;
            collection.Description = input.Description;
            collection.Visibility = input.Visibility;

            foreach (var inputNewPost in input.NewPosts)
            {
                collection.Posts.Add(this.postsService.GetById<Post>(inputNewPost));
            }

            foreach (var inputMediaFile in input.NewMediaFiles)
            {
                CreateMediaFileInputModel mediaFileInputModel = new CreateMediaFileInputModel
                {
                    Extension = Path.GetExtension(inputMediaFile.FileName).TrimStart('.'),
                    RootPath = rootPath,
                    MediaFile = inputMediaFile,
                };

                MediaFile mediaFile = this.mediaFilesService.CreateMediaFile(mediaFileInputModel, userId).GetAwaiter().GetResult();

                collection.MediaFiles.Add(mediaFile);

                await this.mediaFilesService.AddCollectionToMediaFile(mediaFile, collection);
            }

            foreach (var inputRemovedMediaFile in input.RemovedMediaFiles)
            {
                collection.MediaFiles.Remove(this.mediaFilesService.GetById<MediaFile>(inputRemovedMediaFile));

                await this.mediaFilesService.RemoveCollectionFromMediaFile(inputRemovedMediaFile, collection);
            }

            foreach (var inputRemovedPost in input.RemovedPosts)
            {
                collection.Posts.Remove(this.postsService.GetById<Post>(inputRemovedPost));
            }

            await this.collectionsRepository.SaveChangesAsync();
        }

        public async Task DeleteCollectionAsync(string collectionId)
        {
            Collection collection = this.GetById<Collection>(collectionId);

            foreach (var collectionMediaFile in collection.MediaFiles)
            {
                await this.mediaFilesService.RemoveCollectionFromMediaFile(collectionMediaFile.Id, collection);
            }

            foreach (var collectionFollower in collection.Followers)
            {
                await this.usersService.RemoveCollectionFromUserCollection(collectionFollower.Id, collection);
            }

            this.collectionsRepository.Delete(collection);

            await this.collectionsRepository.SaveChangesAsync();
        }

        public async Task FollowCollectionAsync(string collectionId, string userId)
        {
            Collection collection = this.GetById<Collection>(collectionId);

            ApplicationUser user = this.usersService.GetUserById<ApplicationUser>(userId);

            collection.Followers.Add(user);
            await this.usersService.AddCollectionToUserCollection(userId, collection);

            await this.collectionsRepository.SaveChangesAsync();
        }

        public async Task UnfollowCollectionAsync(string collectionId, string userId)
        {
            Collection collection = this.GetById<Collection>(collectionId);

            ApplicationUser user = this.usersService.GetUserById<ApplicationUser>(userId);

            collection.Followers.Remove(user);
            await this.usersService.RemoveCollectionFromUserCollection(userId, collection);

            await this.collectionsRepository.SaveChangesAsync();
        }

        public T GetById<T>(string collectionId) =>
            this.collectionsRepository
                .All()
                .Where(p => p.Id == collectionId)
                .To<T>()
                .FirstOrDefault();
    }
}
