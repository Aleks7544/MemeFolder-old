namespace MemeFolder.Web.ViewModels.MediaFiles
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class CreateMediaFileInputModel
    {
        [Required]
        public string Extension { get; set; }

        [Required]
        public IFormFile MediaFile { get; set; }

        [Required]
        public string RootPath { get; set; }
    }
}
