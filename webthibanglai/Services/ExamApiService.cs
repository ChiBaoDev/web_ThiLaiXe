using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using webthibanglai.Models;

namespace webthibanglai.Services;

public interface IExamApiService
{
    Task<ExamViewModel> GetSampleExamsAsync(string? accessToken, CancellationToken cancellationToken = default);
    Task<StartExamSessionResponseViewModel?> StartSampleExamAsync(long sampleExamId, string? accessToken, CancellationToken cancellationToken = default);
    Task<ExamSessionPageViewModel?> GetSessionAsync(long sessionId, string? accessToken, CancellationToken cancellationToken = default);
    Task<ExamSessionQuestionViewModel?> GetQuestionAsync(long sessionId, int number, string? accessToken, CancellationToken cancellationToken = default);
    Task<bool> SubmitAnswerAsync(long sessionId, long questionId, long answerId, string? accessToken, CancellationToken cancellationToken = default);
    Task<ExamSessionResultViewModel?> SubmitSessionAsync(long sessionId, bool autoSubmit, string? accessToken, CancellationToken cancellationToken = default);
    Task<ExamSessionResultViewModel?> GetResultAsync(long sessionId, string? accessToken, CancellationToken cancellationToken = default);
    Task<List<ExamSessionReviewItemViewModel>> GetReviewAsync(long sessionId, string? accessToken, CancellationToken cancellationToken = default);
}

public class ExamApiService : IExamApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ExamApiService> _logger;

    public ExamApiService(IHttpClientFactory httpClientFactory, ILogger<ExamApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ExamViewModel> GetSampleExamsAsync(string? accessToken, CancellationToken cancellationToken = default)
    {
        var model = new ExamViewModel
        {
            IsAuthenticated = !string.IsNullOrWhiteSpace(accessToken)
        };

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            model.ErrorMessage = "Vui lòng đăng nhập để xem danh sách đề thi mẫu.";
            return model;
        }

        try
        {
            var client = CreateAuthorizedClient(accessToken);
            var response = await client.GetAsync("/api/v1/sample-exams", cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get sample exams failed. StatusCode={StatusCode}, Response={Response}", response.StatusCode, responseBody);
                model.ErrorMessage = ExtractErrorMessage(responseBody) ?? "Không tải được danh sách đề thi mẫu.";
                return model;
            }

            var apiResponse = Deserialize<ApiEnvelope<PagedResult<SampleExamItem>>>(responseBody);
            model.SampleExams = apiResponse?.Data?.Items ?? new List<SampleExamItem>();

            if (model.SampleExams.Count == 0)
            {
                model.ErrorMessage = apiResponse?.Message ?? "Hiện chưa có đề thi mẫu nào.";
            }

            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting sample exams.");
            model.ErrorMessage = "Đã xảy ra lỗi khi tải danh sách đề thi mẫu.";
            return model;
        }
    }

    public async Task<StartExamSessionResponseViewModel?> StartSampleExamAsync(long sampleExamId, string? accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var client = CreateAuthorizedClient(accessToken);
        var response = await client.PostAsync($"/api/v1/exams/sample/{sampleExamId}/start", new StringContent(string.Empty, Encoding.UTF8, "application/json"), cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Start sample exam failed. SampleExamId={SampleExamId}, StatusCode={StatusCode}, Response={Response}", sampleExamId, response.StatusCode, responseBody);
            return null;
        }

        var apiResponse = Deserialize<ApiEnvelope<StartExamSessionResponseViewModel>>(responseBody);
        return apiResponse?.Data;
    }

    public async Task<ExamSessionPageViewModel?> GetSessionAsync(long sessionId, string? accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var client = CreateAuthorizedClient(accessToken);
        var response = await client.GetAsync($"/api/v1/exams/sessions/{sessionId}", cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Get session failed. SessionId={SessionId}, StatusCode={StatusCode}, Response={Response}", sessionId, response.StatusCode, responseBody);
            return null;
        }

        var apiResponse = Deserialize<ApiEnvelope<ExamSessionPageViewModel>>(responseBody);
        return apiResponse?.Data;
    }

    public async Task<ExamSessionQuestionViewModel?> GetQuestionAsync(long sessionId, int number, string? accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var client = CreateAuthorizedClient(accessToken);
        var response = await client.GetAsync($"/api/v1/exams/sessions/{sessionId}/questions/{number}", cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Get question failed. SessionId={SessionId}, Number={Number}, StatusCode={StatusCode}, Response={Response}", sessionId, number, response.StatusCode, responseBody);
            return null;
        }

        var apiResponse = Deserialize<ApiEnvelope<ExamSessionQuestionViewModel>>(responseBody);
        return apiResponse?.Data;
    }

    public async Task<bool> SubmitAnswerAsync(long sessionId, long questionId, long answerId, string? accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return false;
        }

        var client = CreateAuthorizedClient(accessToken);
        var payload = JsonSerializer.Serialize(new
        {
            questionId,
            answerId
        }, JsonOptions());

        var response = await client.PostAsync(
            $"/api/v1/exams/sessions/{sessionId}/answers",
            new StringContent(payload, Encoding.UTF8, "application/json"),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Submit answer failed. SessionId={SessionId}, QuestionId={QuestionId}, AnswerId={AnswerId}, StatusCode={StatusCode}, Response={Response}", sessionId, questionId, answerId, response.StatusCode, responseBody);
            return false;
        }

        return true;
    }

    public async Task<ExamSessionResultViewModel?> SubmitSessionAsync(long sessionId, bool autoSubmit, string? accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var client = CreateAuthorizedClient(accessToken);
        var endpoint = autoSubmit
            ? $"/api/v1/exams/sessions/{sessionId}/auto-submit"
            : $"/api/v1/exams/sessions/{sessionId}/submit";

        var response = await client.PostAsync(endpoint, new StringContent(string.Empty, Encoding.UTF8, "application/json"), cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Submit session failed. SessionId={SessionId}, AutoSubmit={AutoSubmit}, StatusCode={StatusCode}, Response={Response}", sessionId, autoSubmit, response.StatusCode, responseBody);
            return null;
        }

        var apiResponse = Deserialize<ApiEnvelope<ExamSessionResultViewModel>>(responseBody);
        return apiResponse?.Data;
    }

    public async Task<ExamSessionResultViewModel?> GetResultAsync(long sessionId, string? accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var client = CreateAuthorizedClient(accessToken);
        var response = await client.GetAsync($"/api/v1/exams/sessions/{sessionId}/result", cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Get result failed. SessionId={SessionId}, StatusCode={StatusCode}, Response={Response}", sessionId, response.StatusCode, responseBody);
            return null;
        }

        var apiResponse = Deserialize<ApiEnvelope<ExamSessionResultViewModel>>(responseBody);
        return apiResponse?.Data;
    }

    public async Task<List<ExamSessionReviewItemViewModel>> GetReviewAsync(long sessionId, string? accessToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return new List<ExamSessionReviewItemViewModel>();
        }

        var client = CreateAuthorizedClient(accessToken);
        var response = await client.GetAsync($"/api/v1/exams/sessions/{sessionId}/review", cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Get review failed. SessionId={SessionId}, StatusCode={StatusCode}, Response={Response}", sessionId, response.StatusCode, responseBody);
            return new List<ExamSessionReviewItemViewModel>();
        }

        var apiResponse = Deserialize<ApiEnvelope<ExamSessionReviewEnvelope>>(responseBody);
        return apiResponse?.Data?.Items ?? new List<ExamSessionReviewItemViewModel>();
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

    private static string? ExtractErrorMessage(string responseBody)
    {
        try
        {
            var apiResponse = Deserialize<ApiEnvelope<object>>(responseBody);
            if (!string.IsNullOrWhiteSpace(apiResponse?.Message))
            {
                return apiResponse.Message;
            }

            if (apiResponse?.Errors != null && apiResponse.Errors.Count > 0)
            {
                return string.Join(" ", apiResponse.Errors
                    .Select(x => !string.IsNullOrWhiteSpace(x.Detail) ? x.Detail : x.Code)
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private class ExamSessionReviewEnvelope
    {
        public long SessionId { get; set; }
        public List<ExamSessionReviewItemViewModel> Items { get; set; } = new();
    }
}
