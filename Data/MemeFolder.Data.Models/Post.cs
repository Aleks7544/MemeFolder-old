namespace MemeFolder.Data.Models
{
    using System;
    using System.Collections.Generic;

    using MemeFolder.Data.Common.Models;
    using MemeFolder.Data.Models.Enums;

    public class Post : BaseDeletableModel<string>
    {
        public Post()
        {
            this.Id = Guid.NewGuid().ToString();
            this.MediaFiles = new List<MediaFile>();
            this.Likes = new HashSet<Like>();
            this.Comments = new List<Comment>();
            this.Tags = new HashSet<Tag>();
        }

        public string PosterId { get; set; }

        public ApplicationUser Poster { get; set; }

        public Visibility Visibility { get; set; }

        public string Text { get; set; }

        public string RepostedPostId { get; set; }

        public virtual ICollection<MediaFile> MediaFiles { get; set; }

        public virtual ICollection<Like> Likes { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }
}
