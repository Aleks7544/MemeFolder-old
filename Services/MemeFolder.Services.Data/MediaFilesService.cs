namespace MemeFolder.Services.Data
{
    using System.Threading.Tasks;

    using MemeFolder.Data.Common.Repositories;
    using MemeFolder.Data.Models;

    public class MediaFilesService : IMediaFilesService
    {
        private readonly IRepository<MediaFile> mediaFilesRepository;

        public MediaFilesService(IRepository<MediaFile> mediaFilesRepository)
        {
            this.mediaFilesRepository = mediaFilesRepository;
        }

        public async Task AddPostToMediaFile(MediaFile mediaFile, Post post)
        {
            mediaFile.Posts.Add(post);

            await this.mediaFilesRepository.SaveChangesAsync();
        }
    }
}
