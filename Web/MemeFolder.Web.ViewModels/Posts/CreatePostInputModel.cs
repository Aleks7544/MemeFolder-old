namespace MemeFolder.Web.ViewModels.Posts
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MemeFolder.Data.Models.Enums;
    using MemeFolder.Web.ViewModels.Tags;
    using MemeFolder.Web.ViewModels.ValidationAttributes;
    using Microsoft.AspNetCore.Http;

    public class CreatePostInputModel
    {
        [Required]
        [EnumDataType(typeof(Visibility), ErrorMessage = "Invalid visibility type!")]
        public Visibility Visibility { get; set; }

        [TextOrFileRequired]
        public string Text { get; set; }

        public IEnumerable<IFormFile> MediaFiles { get; set; }

        public IEnumerable<CreateTagInputModel> Tags { get; set; }
    }
}
