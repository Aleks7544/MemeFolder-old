namespace MemeFolder.Web.ViewModels.Collections
{
    using System.ComponentModel.DataAnnotations;

    using MemeFolder.Data.Models.Enums;

    public class CollectionInputModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public Visibility Visibility { get; set; }
    }
}
