namespace MemeFolder.Services.Data
{
    using System.Threading.Tasks;
    
    using MemeFolder.Web.ViewModels.MediaFiles;

    public interface IMediaFilesService
    {
        Task CreateMediaFile(CreateMediaFileInputModel input, string userId);

        Task SetMediaFileAsDeleted(string mediaFileId);

        Task CheckIfMediaFileCanBeDeletedFromStorage(string mediaFileId);

        T GetByIdWithTracking<T>(string mediaFileId);
    }
}
