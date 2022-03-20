namespace TheBlogProject.Services
{
    public interface ISlugService
    {
        string urlFriendly(string title);

        bool IsUnique(string slug);
    }
}
