using System.Net;
using System.Net.Http.Json;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Judge.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Falcon.Infrastructure.Judge;

/// <summary>
/// Service for interacting with the Judge API.
/// </summary>
public class JudgeService : IJudgeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenService _tokenService;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<JudgeService> _logger;
    private const string JudgeMemoryTokenKey = "JudgeJwtToken";

    public JudgeService(
        IHttpClientFactory httpClientFactory,
        ITokenService tokenService,
        IMemoryCache memoryCache,
        ILogger<JudgeService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _tokenService = tokenService;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<string?> AuthenticateJudgeAsync()
    {
        string generatedToken = _tokenService.GenerateJudgeToken();
        var httpClient = _httpClientFactory.CreateClient("JudgeAPI");

        try
        {
            var response = await httpClient.PostAsJsonAsync(
                "/auth/integrator-token",
                new { api_key = generatedToken });

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Judge authentication failed with status {StatusCode}", response.StatusCode);
                return null;
            }

            var authResponse = await response.Content.ReadFromJsonAsync<JudgeAuthenticationResponse>();
            return authResponse?.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to authenticate with Judge API");
            return null;
        }
    }

    public async Task<string?> FetchJudgeTokenAsync()
    {
        if (_memoryCache.TryGetValue(JudgeMemoryTokenKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
        {
            bool isValid = _tokenService.ValidateToken(cachedToken);
            if (isValid)
            {
                return cachedToken;
            }
        }

        string? newToken = await AuthenticateJudgeAsync();
        if (newToken != null)
        {
            _memoryCache.Set(JudgeMemoryTokenKey, newToken, TimeSpan.FromHours(1));
        }

        return newToken;
    }

    public async Task<string?> CreateExerciseAsync(string title, string description, List<TestCase> testCases)
    {
        string? token = await FetchJudgeTokenAsync();
        if (token == null)
        {
            return null;
        }

        var httpClient = _httpClientFactory.CreateClient("JudgeAPI");
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var request = new CreateJudgeExerciseRequest
        {
            Name = title,
            Description = description ?? string.Empty,
            DataEntry = testCases.Select(tc => tc.Input).ToList(),
            DataOutput = testCases.Select(tc => tc.ExpectedOutput).ToList(),
            EntryDescription = "",
            OutputDescription = ""
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync("/v0/problems", request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var exerciseResponse = await response.Content.ReadFromJsonAsync<JudgeExerciseResponse>();
                if (exerciseResponse != null)
                {
                    _logger.LogInformation("Created exercise in Judge with UUID {Uuid}", exerciseResponse.Id);
                    return exerciseResponse.Id;
                }
            }
            else
            {
                _logger.LogWarning("Failed to create exercise in Judge. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating exercise in Judge API");
        }

        return null;
    }

    public async Task<JudgeSubmissionResult> SubmitCodeAsync(string code, string language, string exerciseUuid)
    {
        string? token = await FetchJudgeTokenAsync();
        if (token == null)
        {
            throw new InvalidOperationException("Failed to authenticate with Judge API");
        }

        var httpClient = _httpClientFactory.CreateClient("JudgeAPI");
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var request = new JudgeSubmissionRequest
        {
            ProblemId = exerciseUuid,
            Content = code,
            LanguageType = language
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync("/v0/submissions", request);

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                _logger.LogWarning("Judge submission failed - unprocessable entity for exercise {ExerciseUuid}", exerciseUuid);
                return new JudgeSubmissionResult(
                    Guid.NewGuid().ToString(),
                    JudgeSubmissionResponse.CompilationError,
                    TimeSpan.Zero,
                    0);
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Judge submission failed with status {StatusCode}", response.StatusCode);
                return new JudgeSubmissionResult(
                    Guid.NewGuid().ToString(),
                    JudgeSubmissionResponse.RuntimeError,
                    TimeSpan.Zero,
                    0);
            }

            var submissionResponse = await response.Content.ReadFromJsonAsync<JudgeSubmissionResponseDto>();
            if (submissionResponse != null)
            {
                var status = ParseJudgeStatus(submissionResponse.Status);
                var executionTime = TimeSpan.FromMilliseconds(submissionResponse.ExecutionTime);

                _logger.LogInformation("Submission result: {Status}, Execution time: {Time}ms",
                    status, submissionResponse.ExecutionTime);

                return new JudgeSubmissionResult(
                    Guid.NewGuid().ToString(),
                    status,
                    executionTime,
                    0);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting code to Judge API");
        }

        return new JudgeSubmissionResult(
            Guid.NewGuid().ToString(),
            JudgeSubmissionResponse.RuntimeError,
            TimeSpan.Zero,
            0);
    }

    public async Task<JudgeExerciseInfo?> GetExerciseByUuidAsync(string judgeUuid)
    {
        string? token = await FetchJudgeTokenAsync();
        if (token == null)
        {
            return null;
        }

        var httpClient = _httpClientFactory.CreateClient("JudgeAPI");
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        try
        {
            var exerciseResponse = await httpClient.GetFromJsonAsync<JudgeExerciseResponse>($"/v0/problems/{judgeUuid}");
            if (exerciseResponse != null)
            {
                return new JudgeExerciseInfo(
                    exerciseResponse.Id,
                    exerciseResponse.Name,
                    exerciseResponse.Description,
                    exerciseResponse.DataEntry,
                    exerciseResponse.DataOutput);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exercise from Judge API");
        }

        return null;
    }

    public async Task<bool> UpdateExerciseAsync(string judgeUuid, string title, string description, List<TestCase> testCases)
    {
        string? token = await FetchJudgeTokenAsync();
        if (token == null)
        {
            return false;
        }

        var httpClient = _httpClientFactory.CreateClient("JudgeAPI");
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var request = new UpdateJudgeExerciseRequest
        {
            ProblemId = judgeUuid,
            Name = title,
            Description = description ?? string.Empty,
            DataEntry = testCases.Select(tc => tc.Input).ToArray(),
            DataOutput = testCases.Select(tc => tc.ExpectedOutput).ToArray()
        };

        try
        {
            var response = await httpClient.PutAsJsonAsync($"/v0/problems/{judgeUuid}", request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation("Updated exercise in Judge with UUID {Uuid}", judgeUuid);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to update exercise in Judge. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating exercise in Judge API");
        }

        return false;
    }

    private JudgeSubmissionResponse ParseJudgeStatus(string status)
    {
        return status.ToUpperInvariant() switch
        {
            "ACCEPTED" => JudgeSubmissionResponse.Accepted,
            "PRESENTATION ERROR" => JudgeSubmissionResponse.PresentationError,
            "WRONG ANSWER" => JudgeSubmissionResponse.WrongAnswer,
            "COMPILATION ERROR" => JudgeSubmissionResponse.CompilationError,
            "TIME LIMIT EXCEEDED" => JudgeSubmissionResponse.TimeLimitExceeded,
            "MEMORY LIMIT EXCEEDED" => JudgeSubmissionResponse.MemoryLimitExceeded,
            "RUNTIME ERROR" => JudgeSubmissionResponse.RuntimeError,
            "SECURITY ERROR" => JudgeSubmissionResponse.SecurityError,
            _ => JudgeSubmissionResponse.RuntimeError
        };
    }
}
