namespace MemeFolder.Services.Data
{
    using System;
    using System.Threading.Tasks;

    using MemeFolder.Data.Common.Repositories;
    using MemeFolder.Data.Models;
    using MemeFolder.Web.ViewModels.Tags;

    public class TagsService : ITagsService
    {
        private readonly IRepository<Tag> tagsRepository;

        public TagsService(IRepository<Tag> tagsRepository)
        {
            this.tagsRepository = tagsRepository;
        }

        public async Task AddTagToPost(Tag tag, Post post)
        {
            tag.Posts.Add(post);

            await this.tagsRepository.SaveChangesAsync();
        }

        public async Task AddMediaFileToTag(Tag tag, MediaFile mediaFile)
        {
            tag.Files.Add(mediaFile);

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
    }
}
