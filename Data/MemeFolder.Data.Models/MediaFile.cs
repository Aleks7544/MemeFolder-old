namespace MemeFolder.Data.Models
{
    using System;
    using System.Collections.Generic;

    using MemeFolder.Data.Common.Models;
    using MemeFolder.Data.Models.Enums;

    public class MediaFile : BaseModel<string>
    {
        public MediaFile()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Posts = new HashSet<Post>();
            this.Comments = new HashSet<Comment>();
        }

        public string Extension { get; set; }

        public string FilePath { get; set; }

        public string UploaderId { get; set; }

        public ApplicationUser Uploader { get; set; }

        // Posts which contain the given media file
        public virtual ICollection<Post> Posts { get; set; }

        // Comments which contain the given media file
        public virtual ICollection<Comment> Comments { get; set; }

        // Collections which contain the given media file
        public virtual ICollection<Collection> Collections { get; set; }

        // A single given media file can be used on multiple Posts, Collections and/or Comments, thus reducing unnecessarily wasted storage. The two collections above exist in order to reduce the number of duplicate files that would otherwise be created if the given media file was to be used more than a single time.
    }
}
