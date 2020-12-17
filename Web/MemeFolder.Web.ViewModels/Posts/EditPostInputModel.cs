namespace MemeFolder.Web.ViewModels.Posts
{
    using System.Collections.Generic;

    using MemeFolder.Web.ViewModels.Tags;
    using Microsoft.AspNetCore.Http;

    public class EditPostInputModel : BasePostInputModel
    {
        public IEnumerable<CreateTagInputModel> NewTags { get; set; }

        public IEnumerable<IFormFile> NewMediaFiles { get; set; }

        public IEnumerable<string> RemovedTags { get; set; }

        public IEnumerable<string> RemovedMediaFiles { get; set; }
    }
}
