namespace webthibanglai.Services
{
    public interface IAIService
    {
        Task<string> GetReplyAsync(string message, string context = "general");
    }
}
