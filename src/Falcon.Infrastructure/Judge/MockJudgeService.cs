using Falcon.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Falcon.Infrastructure.Judge;

/// <summary>
/// Mock implementation of IJudgeService for testing purposes.
/// </summary>
public class MockJudgeService : IJudgeService
{
    private readonly ILogger<MockJudgeService> _logger;
    private readonly Random _random = new();

    public MockJudgeService(ILogger<MockJudgeService> logger)
    {
        _logger = logger;
    }

    public Task<string> CreateExerciseAsync(string title, string description, List<TestCase> testCases)
    {
        var judgeUuid = Guid.NewGuid().ToString();
        
        _logger.LogInformation("Mock: Created exercise '{Title}' in Judge with UUID {JudgeUuid}", 
            title, judgeUuid);
        
        return Task.FromResult(judgeUuid);
    }

    public Task<JudgeSubmissionResult> SubmitCodeAsync(string code, string language, string exerciseUuid)
    {
        // Simulate random submission results
        var outcomes = new[]
        {
            JudgeSubmissionResponse.Accepted,
            JudgeSubmissionResponse.Accepted,
            JudgeSubmissionResponse.Accepted,
            JudgeSubmissionResponse.WrongAnswer,
            JudgeSubmissionResponse.TimeLimitExceeded,
            JudgeSubmissionResponse.RuntimeError
        };

        var status = outcomes[_random.Next(outcomes.Length)];
        var executionTime = TimeSpan.FromMilliseconds(_random.Next(100, 2000));
        var memoryUsed = _random.Next(1000, 10000);
        var submissionId = Guid.NewGuid().ToString();

        _logger.LogInformation(
            "Mock: Submitted code to exercise {ExerciseUuid}. Result: {Status}, Time: {ExecutionTime}ms", 
            exerciseUuid, status, executionTime.TotalMilliseconds);

        return Task.FromResult(new JudgeSubmissionResult(
            submissionId,
            status,
            executionTime,
            memoryUsed
        ));
    }

    public Task<JudgeSubmissionStatus> GetSubmissionStatusAsync(string submissionId)
    {
        _logger.LogInformation("Mock: Getting status for submission {SubmissionId}", submissionId);

        return Task.FromResult(new JudgeSubmissionStatus(
            submissionId,
            JudgeSubmissionResponse.Accepted,
            null
        ));
    }
}
