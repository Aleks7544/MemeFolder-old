namespace MemeFolder.Web.ViewModels.Posts
{
    using System.Collections.Generic;

    using MemeFolder.Web.ViewModels.Tags;
    using Microsoft.AspNetCore.Http;

    public class CreatePostInputModel : BasePostInputModel
    {
        public IEnumerable<IFormFile> MediaFiles { get; set; }

        public IEnumerable<CreateTagInputModel> Tags { get; set; }
    }
}
