namespace MemeFolder.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MemeFolder.Data.Common.Repositories;
    using MemeFolder.Data.Models;
    using MemeFolder.Services.Mapping;
    using MemeFolder.Web.ViewModels.Tags;

    public class TagsService : ITagsService
    {
        private readonly IRepository<Tag> tagsRepository;

        public TagsService(IRepository<Tag> tagsRepository)
        {
            this.tagsRepository = tagsRepository;
        }

        public async Task AddPostToTagCollection(Tag tag, Post post)
        {
            tag.Posts.Add(post);

            await this.tagsRepository.SaveChangesAsync();
        }

        public async Task AddMediaFileToTagCollection(Tag tag, MediaFile mediaFile)
        {
            tag.MediaFiles.Add(mediaFile);

            await this.tagsRepository.SaveChangesAsync();
        }

        public async Task<Tag> CreateTagAsync(CreateTagInputModel input)
        {
            Tag tag = new Tag
            {
                Name = input.Name,
                Color = input.Color,
                CreatedOn = DateTime.UtcNow,
            };

            await this.tagsRepository.AddAsync(tag);
            await this.tagsRepository.SaveChangesAsync();

            return tag;
        }

        public async Task RemovePostFromTagCollection(string tagId, Post post)
        {
            Tag tag = this.GetById<Tag>(tagId);

            tag.Posts.Remove(post);

            if (!tag.MediaFiles.Any() && !tag.Posts.Any())
            {
                this.tagsRepository.Delete(tag);
            }

            await this.tagsRepository.SaveChangesAsync();
        }

        public async Task RemoveMediaFileFromTagCollection(string tagId, MediaFile mediaFile)
        {
            Tag tag = this.GetById<Tag>(tagId);

            tag.MediaFiles.Remove(mediaFile);

            if (!tag.MediaFiles.Any() && !tag.Posts.Any())
            {
                this.tagsRepository.Delete(tag);
            }

            await this.tagsRepository.SaveChangesAsync();
        }

        public T GetById<T>(string id) =>
            this.tagsRepository
                .AllAsNoTracking()
                .Where(p => p.Id == id)
                .To<T>()
                .FirstOrDefault();
    }
}
