namespace MemeFolder.Services.Data
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MemeFolder.Data.Common.Repositories;
    using MemeFolder.Data.Models;
    using MemeFolder.Services.Mapping;
    using MemeFolder.Web.ViewModels.MediaFiles;

    public class MediaFilesService : IMediaFilesService
    {
        private readonly IRepository<MediaFile> mediaFilesRepository;
        private readonly IRepository<Tag> tagsRepository;

        private readonly string[] allowedExtensions = new[] { "jpg", "png", "jpeg", "gif", "mp3", "wav", "ogg", "mp4" };

        public MediaFilesService(IRepository<MediaFile> mediaFilesRepository, IRepository<Tag> tagsRepository)
        {
            this.mediaFilesRepository = mediaFilesRepository;
            this.tagsRepository = tagsRepository;
        }

        public async Task CreateMediaFile(CreateMediaFileInputModel input, string userId)
        {
            string extension = Path.GetExtension(input.MediaFile.FileName).TrimStart('.');

            if (!this.allowedExtensions.Any(x => extension.EndsWith(x)))
            {
                throw new Exception($"Invalid or unsupported file extension {extension}");
            }

            string directory = $"{input.RootPath}/mediaFiles/{userId}";

            Directory.CreateDirectory(directory);

            MediaFile mediaFile = new MediaFile
            {
                Extension = input.Extension,
                UploaderId = userId,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false,
            };

            mediaFile.FilePath = $"{directory}/{mediaFile.Id}.{input.Extension}";

            await using Stream fileStream = new FileStream(mediaFile.FilePath, FileMode.Create);
            await input.MediaFile.CopyToAsync(fileStream);

            await this.mediaFilesRepository.AddAsync(mediaFile);
            await this.mediaFilesRepository.SaveChangesAsync();
        }

        public async Task SetMediaFileAsDeleted(string mediaFileId)
        {
            MediaFile mediaFile = this.GetByIdWithTracking<MediaFile>(mediaFileId);

            mediaFile.IsDeleted = true;

            await this.mediaFilesRepository.SaveChangesAsync();

            await this.CheckIfMediaFileCanBeDeletedFromStorage(mediaFileId);
        }

        public async Task CheckIfMediaFileCanBeDeletedFromStorage(string mediaFileId)
        {
            MediaFile mediaFile = this.GetByIdWithTracking<MediaFile>(mediaFileId);

            if (mediaFile.IsDeleted && !mediaFile.Posts.Any() && !mediaFile.Comments.Any() && !mediaFile.Collections.Any())
            {
                File.Delete(mediaFile.FilePath);

                foreach (var mediaFileTag in mediaFile.Tags)
                {
                    Tag tag = this.tagsRepository.All().FirstOrDefault(x => x.Equals(mediaFileTag));

                    tag.MediaFiles.Remove(mediaFile);

                    mediaFile.Tags.Remove(mediaFileTag);

                    await this.tagsRepository.SaveChangesAsync();
                }

                this.mediaFilesRepository.Delete(mediaFile);
            }

            await this.mediaFilesRepository.SaveChangesAsync();
            await this.tagsRepository.SaveChangesAsync();
        }

        public T GetByIdWithTracking<T>(string mediaFileId) =>
            this.mediaFilesRepository
                .All()
                .Where(m => m.Id == mediaFileId)
                .To<T>()
                .FirstOrDefault();
    }
}
