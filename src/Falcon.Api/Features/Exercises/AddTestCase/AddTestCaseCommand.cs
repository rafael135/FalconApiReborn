using MediatR;

namespace Falcon.Api.Features.Exercises.AddTestCase;

public record AddTestCaseCommand(
    Guid ExerciseId,
    string InputContent,
    string ExpectedOutput
) : IRequest<AddTestCaseResult>;
