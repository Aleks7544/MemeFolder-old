namespace MemeFolder.Data.Models
{
    using System;
    using System.Collections.Generic;

    using MemeFolder.Data.Common.Models;

    public class Collection : BaseDeletableModel<string>
    {
        public Collection()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Files = new List<MediaFile>();
            this.Posts = new List<Post>();
            this.Followers = new HashSet<ApplicationUser>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CreatorId { get; set; }

        public ApplicationUser Creator { get; set; }

        public virtual ICollection<MediaFile> Files { get; set; }

        public virtual ICollection<Post> Posts { get; set; }

        public virtual ICollection<ApplicationUser> Followers { get; set; }
    }
}
