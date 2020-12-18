namespace MemeFolder.Web.ViewModels.Collections
{
    using System.Collections.Generic;

    using MemeFolder.Data.Models;
    using Microsoft.AspNetCore.Http;

    public class EditCollectionInputModel : BaseCollectionInputModel
    {
        public IEnumerable<IFormFile> NewMediaFiles { get; set; }

        public IEnumerable<string> NewPosts { get; set; }

        public IEnumerable<string> RemovedMediaFiles { get; set; }

        public IEnumerable<string> RemovedPosts { get; set; }
    }
}
