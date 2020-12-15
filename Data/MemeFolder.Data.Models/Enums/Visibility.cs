namespace MemeFolder.Data.Models.Enums
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public enum Visibility
    {
        [Display(Name = "Me Only")]
        MeOnly = 1,
        [Display(Name = "Friends Only")]
        FriendsOnly = 2,
        [Display(Name = "Followers And Friends")]
        FollowersAndFriends = 3,
        [Display(Name = "Public")]
        Public = 4,
    }
}
