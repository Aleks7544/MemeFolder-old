namespace MemeFolder.Data.Models
{
    using System;
    using System.Collections.Generic;

    using MemeFolder.Data.Common.Models;

    public class Tag : BaseModel<string>
    {
        public Tag()
        {
            this.Id = Guid.NewGuid().ToString();
            this.MediaFiles = new HashSet<MediaFile>();
            this.Posts = new HashSet<Post>();
            this.Tags = new HashSet<Tag>();
        }

        public string Name { get; set; }

        public int Color { get; set; }

        public virtual ICollection<MediaFile> MediaFiles { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }
}
