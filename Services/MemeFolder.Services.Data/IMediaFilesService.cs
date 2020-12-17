namespace MemeFolder.Services.Data
{
    using System.Threading.Tasks;

    using MemeFolder.Data.Models;
    using MemeFolder.Web.ViewModels.MediaFiles;

    public interface IMediaFilesService
    {
        Task AddPostToMediaFile(MediaFile mediaFile, Post post);

        Task AddCommentToMediaFile(MediaFile mediaFile, Comment comment);

        Task<MediaFile> CreateMediaFile(CreateMediaFileInputModel input, string userId);
    }
}
