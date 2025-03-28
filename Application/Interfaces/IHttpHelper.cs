namespace Application.Interfaces
{
    public interface IHttpHelper
    {
        public string GetHostPathPrefix();
        public Guid GetCurrentUserId();
    }
}
