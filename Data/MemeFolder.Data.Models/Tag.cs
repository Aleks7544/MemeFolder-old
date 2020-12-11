namespace MemeFolder.Data.Models
{
    using System;
    using System.Collections.Generic;

    using MemeFolder.Data.Common.Models;

    public class Tag : BaseDeletableModel<string>
    {
        public Tag()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Files = new HashSet<MediaFile>();
            this.Posts = new HashSet<Post>();
        }

        public string Name { get; set; }

        public int Color { get; set; }

        public string VectorImagePath { get; set; }

        public virtual ICollection<MediaFile> Files { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
