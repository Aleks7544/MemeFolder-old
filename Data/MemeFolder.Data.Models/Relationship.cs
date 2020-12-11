namespace MemeFolder.Data.Models
{
    using MemeFolder.Data.Common.Models;
    using MemeFolder.Data.Models.Enums;

    public class Relationship : BaseModel<int>
    {
        public string FirstUserId { get; set; }

        public ApplicationUser FirstUser { get; set; }

        public string SecondUserId { get; set; }

        public ApplicationUser SecondUser { get; set; }

        public RelationshipStatus Status { get; set; }
    }
}
