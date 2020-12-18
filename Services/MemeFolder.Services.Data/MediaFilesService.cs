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

        public MediaFilesService(IRepository<MediaFile> mediaFilesRepository)
        {
            this.mediaFilesRepository = mediaFilesRepository;
        }

        public async Task<MediaFile> CreateMediaFile(CreateMediaFileInputModel input, string userId)
        {
            string directory = $"{input.RootPath}/mediaFiles/{userId}";

            Directory.CreateDirectory(directory);

            MediaFile mediaFile = new MediaFile
            {
                Extension = input.Extension,
                UploaderId = userId,
                CreatedOn = DateTime.UtcNow,
            };
            mediaFile.FilePath = $"{directory}/{mediaFile.Id}.{input.Extension}";

            await using Stream fileStream = new FileStream(mediaFile.FilePath, FileMode.Create);
            await input.MediaFile.CopyToAsync(fileStream);

            await this.mediaFilesRepository.AddAsync(mediaFile);
            await this.mediaFilesRepository.SaveChangesAsync();

            return mediaFile;
        }

        public async Task AddPostToMediaFile(MediaFile mediaFile, Post post)
        {
            mediaFile.Posts.Add(post);

            await this.mediaFilesRepository.SaveChangesAsync();
        }

        public async Task AddCommentToMediaFile(MediaFile mediaFile, Comment comment)
        {
            mediaFile.Comments.Add(comment);

            await this.mediaFilesRepository.SaveChangesAsync();
        }

        public async Task RemoveCommentFromMediaFile(string mediaFileId, Comment comment)
        {
            MediaFile mediaFile = this.GetById<MediaFile>(mediaFileId);

            mediaFile.Comments.Remove(comment);

            if (!mediaFile.Posts.Any() && !mediaFile.Comments.Any())
            {
                File.Delete(mediaFile.FilePath);
            }

            await this.mediaFilesRepository.SaveChangesAsync();
        }

        public async Task RemovePostFromMediaFile(string mediaFileId, Post post)
        {
            MediaFile mediaFile = this.GetById<MediaFile>(mediaFileId);

            mediaFile.Posts.Remove(post);

            if (!mediaFile.Posts.Any() && !mediaFile.Comments.Any())
            {
                File.Delete(mediaFile.FilePath);
            }

            await this.mediaFilesRepository.SaveChangesAsync();
        }

        public T GetById<T>(string mediaFileId) =>
            this.mediaFilesRepository
                .AllAsNoTracking()
                .Where(m => m.Id == mediaFileId)
                .To<T>()
                .FirstOrDefault();
    }
}
