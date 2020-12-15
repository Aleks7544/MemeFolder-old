namespace MemeFolder.Services.Data
{
    using System.Threading.Tasks;

    using MemeFolder.Data.Models;

    public interface IMediaFilesService
    {
        Task AddPostToMediaFile(MediaFile mediaFile, Post post);
    }
}
