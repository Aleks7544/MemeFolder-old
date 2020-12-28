namespace MemeFolder.Web.ViewModels.Comments
{
    using System.Collections.Generic;

    using MemeFolder.Data.Models;
    using MemeFolder.Web.ViewModels.ValidationAttributes;

    public class BaseCommentInputModel
    {
        [TextOrFileRequired]
        public string Text { get; set; }

        public ICollection<MediaFile> MediaFiles { get; set; }
    }
}
