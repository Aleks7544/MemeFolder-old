namespace MemeFolder.Data.Models
{
    using System;

    using MemeFolder.Data.Models.Enums;

    public class Like
    {
        public string PostId { get; set; }

        public Post Post { get; set; }

        public string CommentId { get; set; }

        public Comment Comment { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public ReactionType Reaction { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
