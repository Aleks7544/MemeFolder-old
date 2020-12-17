namespace MemeFolder.Data.Models
{
    using System.Collections.Generic;

    public class Comment
    {
        public Comment()
        {
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
