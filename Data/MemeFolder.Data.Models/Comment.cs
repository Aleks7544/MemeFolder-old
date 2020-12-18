namespace MemeFolder.Data.Models
{
    using System;
    using System.Collections.Generic;

    using MemeFolder.Data.Common.Models;

    public class Comment : BaseDeletableModel<string>
    {
        public Comment()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Media = new List<MediaFile>();
            this.Likes = new List<Like>();
        }

        public string PostId { get; set; }

        public Post Post { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string Text { get; set; }

        public virtual ICollection<MediaFile> Media { get; set; }

        public virtual ICollection<Like> Likes { get; set; }
    }
}
