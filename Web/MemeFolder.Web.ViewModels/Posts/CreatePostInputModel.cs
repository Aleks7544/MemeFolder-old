namespace MemeFolder.Web.ViewModels.Posts
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using MemeFolder.Data.Models;
    using MemeFolder.Data.Models.Enums;
    using MemeFolder.Web.ViewModels.Tags;
    using MemeFolder.Web.ViewModels.ValidationAttributes;

    public class CreatePostInputModel
    {
        [Required]
        [EnumDataType(typeof(Visibility), ErrorMessage = "Invalid visibility type!")]
        public Visibility Visibility { get; set; }

        [TextOrFileRequired]
        public string Text { get; set; }

        public IEnumerable<MediaFile> MediaFiles { get; set; }

        public IEnumerable<CreateTagInputModel> Tags { get; set; }
    }
}
