using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace webthibanglai.Services;

public interface IPracticeApiService
{
    Task<PracticeQuestionsResponse?> GetQuestionsByTopicAsync(string topicCode, string? accessToken, CancellationToken cancellationToken = default);
    Task<bool> RecordWrongAnswerAsync(long questionId, long selectedAnswerId, string? accessToken, CancellationToken cancellationToken = default);
}

public class PracticeApiService : IPracticeApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PracticeApiService> _logger;

    public PracticeApiService(IHttpClientFactory httpClientFactory, ILogger<PracticeApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<PracticeQuestionsResponse?> GetQuestionsByTopicAsync(string topicCode, string? accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            
            // Xây dựng URL với pagination và includeCorrectAnswer
            var url = "/api/v1/questions/with-answers?page=1&pageSize=250&includeCorrectAnswer=true&includeExplanation=true";
            
            // Thêm topicCode nếu có
            if (!string.IsNullOrWhiteSpace(topicCode))
            {
                url += $"&topicCode={Uri.EscapeDataString(topicCode)}";
            }
            
            var response = await client.GetAsync(url, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get questions by topic failed. TopicCode={TopicCode}, StatusCode={StatusCode}, Response={Response}",
                    topicCode, response.StatusCode, responseBody);
                return null;
            }

            var apiResponse = Deserialize<ApiEnvelope<PracticeQuestionsResponse>>(responseBody);
            return apiResponse?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting questions by topic. TopicCode={TopicCode}", topicCode);
            return null;
        }
    }

    public async Task<bool> RecordWrongAnswerAsync(long questionId, long selectedAnswerId, string? accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return false;
        }

        try
        {
            var client = CreateAuthorizedClient(accessToken);
            var payload = JsonSerializer.Serialize(new
            {
                questionId,
                selectedAnswerId
            }, JsonOptions());

            var response = await client.PostAsync(
                "/api/v1/wrong-questions/practice-sessions",
                new StringContent(payload, Encoding.UTF8, "application/json"),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Record wrong answer failed. QuestionId={QuestionId}, SelectedAnswerId={SelectedAnswerId}, StatusCode={StatusCode}, Response={Response}", 
                    questionId, selectedAnswerId, response.StatusCode, responseBody);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while recording wrong answer. QuestionId={QuestionId}", questionId);
            return false;
        }
    }

    private HttpClient CreateAuthorizedClient(string accessToken)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private static T? Deserialize<T>(string responseBody)
    {
        return JsonSerializer.Deserialize<T>(responseBody, JsonOptions());
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}

// Response models
public class PracticeQuestionsResponse
{
    public List<PracticeQuestion> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}

public class PracticeQuestion
{
    public long Id { get; set; }
    public long TopicId { get; set; }
    public string TopicCode { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string QuestionType { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public bool IsCritical { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public string? ImageUrl { get; set; }
    public List<PracticeAnswer> Answers { get; set; } = new();
}

public class PracticeAnswer
{
    public long AnswerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool? IsCorrect { get; set; }
}

public class ApiEnvelope<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<ApiError>? Errors { get; set; }
}

public class ApiError
{
    public string Code { get; set; } = string.Empty;
    public string? Field { get; set; }
    public string? Detail { get; set; }
}
