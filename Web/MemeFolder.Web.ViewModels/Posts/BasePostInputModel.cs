namespace MemeFolder.Web.ViewModels.Posts
{
    using System.ComponentModel.DataAnnotations;

    using MemeFolder.Data.Models.Enums;
    using MemeFolder.Web.ViewModels.ValidationAttributes;

    public class BasePostInputModel
    {
        [Required]
        [EnumDataType(typeof(Visibility), ErrorMessage = "Invalid visibility type!")]
        public Visibility Visibility { get; set; }

        [TextOrFileRequired]
        public string Text { get; set; }
    }
}
