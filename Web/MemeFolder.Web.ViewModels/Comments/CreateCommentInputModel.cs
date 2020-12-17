namespace MemeFolder.Web.ViewModels.Comments
{
    using System.Collections.Generic;

    using MemeFolder.Web.ViewModels.ValidationAttributes;
    using Microsoft.AspNetCore.Http;

    public class CreateCommentInputModel
    {
        [TextOrFileRequired]
        public string Text { get; set; }

        public IEnumerable<IFormFile> MediaFiles { get; set; }
    }
}
