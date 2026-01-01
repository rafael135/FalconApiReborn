using Falcon.Api.Features.Exercises.Shared;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Exercises.AddTestCase;

public class AddTestCaseHandler : IRequestHandler<AddTestCaseCommand, AddTestCaseResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<AddTestCaseHandler> _logger;

    public AddTestCaseHandler(FalconDbContext dbContext, ILogger<AddTestCaseHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<AddTestCaseResult> Handle(AddTestCaseCommand request, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(request.InputContent))
            errors.Add(nameof(request.InputContent), "Entrada é obrigatória");
        if (string.IsNullOrWhiteSpace(request.ExpectedOutput))
            errors.Add(nameof(request.ExpectedOutput), "Saída esperada é obrigatória");
        if (errors.Any())
            throw new FormException(errors);

        var exercise = await _dbContext.Exercises
            .FirstOrDefaultAsync(e => e.Id == request.ExerciseId, cancellationToken);

        if (exercise == null)
            throw new NotFoundException("Exercise", request.ExerciseId);

        var input = new ExerciseInput(request.InputContent);
        input.SetExercise(exercise);
        
        await _dbContext.ExerciseInputs.AddAsync(input, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var output = new ExerciseOutput(request.ExpectedOutput, exercise);
        await _dbContext.ExerciseOutputs.AddAsync(output, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Test case added to exercise {ExerciseId}", request.ExerciseId);

        return new AddTestCaseResult(new TestCaseDto(input.Id, output.Id, input.InputContent, output.OutputContent));
    }
}
