namespace MemeFolder.Web.ViewModels.Tags
{
    using ValidationAttributes;

    public class CreateTagInputModel
    {
        [TagIdOrNameAndColorRequired]
        public string Id { get; set; }

        public string Name { get; set; }

        public int Color { get; set; }
    }
}
