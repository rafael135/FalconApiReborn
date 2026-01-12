using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Exercises.RemoveTestCase;

/// <summary>
/// Handler that removes a test case (input and corresponding output) from an exercise.
/// </summary>
/// <remarks>
/// Throws <see cref="Falcon.Core.Domain.Shared.Exceptions.NotFoundException"/> if the test case input is not found.
/// </remarks>
public class RemoveTestCaseHandler : IRequestHandler<RemoveTestCaseCommand, RemoveTestCaseResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<RemoveTestCaseHandler> _logger;

    public RemoveTestCaseHandler(FalconDbContext dbContext, ILogger<RemoveTestCaseHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<RemoveTestCaseResult> Handle(RemoveTestCaseCommand request, CancellationToken cancellationToken)
    {
        var input = await _dbContext.ExerciseInputs
            .FirstOrDefaultAsync(i => i.Id == request.TestCaseId, cancellationToken);

        if (input == null)
            throw new NotFoundException("TestCase", request.TestCaseId);

        var output = await _dbContext.ExerciseOutputs
            .FirstOrDefaultAsync(o => o.ExerciseInputId == request.TestCaseId, cancellationToken);

        if (output != null)
            _dbContext.ExerciseOutputs.Remove(output);

        _dbContext.ExerciseInputs.Remove(input);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Test case {TestCaseId} removed from exercise {ExerciseId}", request.TestCaseId, request.ExerciseId);

        return new RemoveTestCaseResult(true, "Caso de teste removido com sucesso");
    }
}
