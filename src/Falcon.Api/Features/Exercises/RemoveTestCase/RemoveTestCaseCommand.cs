using MediatR;

namespace Falcon.Api.Features.Exercises.RemoveTestCase;

public record RemoveTestCaseCommand(Guid ExerciseId, Guid TestCaseId) : IRequest<RemoveTestCaseResult>;
