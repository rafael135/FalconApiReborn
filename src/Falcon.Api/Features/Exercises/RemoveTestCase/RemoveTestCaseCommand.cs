using MediatR;

namespace Falcon.Api.Features.Exercises.RemoveTestCase;

/// <summary>
/// Comando para remover um caso de teste de um exercício.
/// </summary>
/// <param name="ExerciseId">ID do exercício.</param>
/// <param name="TestCaseId">ID do caso de teste a ser removido.</param>
public record RemoveTestCaseCommand(Guid ExerciseId, Guid TestCaseId) : IRequest<RemoveTestCaseResult>; 
