namespace MemeFolder.Web.ViewModels.Posts
{
    using System.Collections.Generic;

    using MemeFolder.Data.Models;
    using MemeFolder.Web.ViewModels.Tags;

    public class CreatePostInputModel : BasePostInputModel
    {
        public ICollection<MediaFile> MediaFiles { get; set; }

        public ICollection<Tag> Tags { get; set; }
    }
}
