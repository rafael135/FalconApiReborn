using MediatR;

namespace Falcon.Api.Features.Exercises.AddTestCase;

/// <summary>
/// Comando para adicionar um caso de teste a um exercício.
/// </summary>
/// <param name="ExerciseId">ID do exercício ao qual o caso de teste será adicionado.</param>
/// <param name="InputContent">Conteúdo de entrada do caso de teste.</param>
/// <param name="ExpectedOutput">Saída esperada para o caso de teste.</param>
public record AddTestCaseCommand(Guid ExerciseId, string InputContent, string ExpectedOutput)
    : IRequest<AddTestCaseResult>;
