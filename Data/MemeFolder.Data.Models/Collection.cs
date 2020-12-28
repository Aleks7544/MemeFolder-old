namespace MemeFolder.Data.Models
{
    using System;
    using System.Collections.Generic;

    using MemeFolder.Data.Common.Models;
    using MemeFolder.Data.Models.Enums;

    public class Collection : BaseDeletableModel<string>
    {
        public Collection()
        {
            this.Id = Guid.NewGuid().ToString();
            this.MediaFiles = new List<MediaFile>();
            this.Posts = new List<Post>();
            this.Followers = new HashSet<ApplicationUser>();
            this.Tags = new HashSet<Tag>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public Visibility Visibility { get; set; }

        public string CreatorId { get; set; }

        public ApplicationUser Creator { get; set; }

        public virtual ICollection<MediaFile> MediaFiles { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public virtual ICollection<ApplicationUser> Followers { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }
}
