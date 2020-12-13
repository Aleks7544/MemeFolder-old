namespace MemeFolder.Data.Models
{
    using MemeFolder.Data.Models.Enums;

    public class Like
    {
        public string PostId { get; set; }

        public Post Post { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public ReactionType Reaction { get; set; }
    }
}
