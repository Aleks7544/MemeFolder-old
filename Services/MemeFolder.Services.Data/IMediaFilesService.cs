namespace MemeFolder.Services.Data
{
    using System.Threading.Tasks;

    using MemeFolder.Data.Models;
    using MemeFolder.Web.ViewModels.MediaFiles;

    public interface IMediaFilesService
    {
        Task<MediaFile> CreateMediaFile(CreateMediaFileInputModel input, string userId);

        Task AddPostToMediaFile(MediaFile mediaFile, Post post);

        Task AddCommentToMediaFile(MediaFile mediaFile, Comment comment);

        Task RemoveCommentFromMediaFile(string mediaFileId, Comment comment);

        Task RemovePostFromMediaFile(string mediaFileId, Post post);

        T GetById<T>(string mediaFileId);
    }
}
