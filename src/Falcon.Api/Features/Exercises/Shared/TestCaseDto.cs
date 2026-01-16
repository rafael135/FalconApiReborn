namespace Falcon.Api.Features.Exercises.Shared;

/// <summary>
/// Informações de um caso de teste (entrada e saída esperada).
/// </summary>
/// <param name="InputId">ID do input associado.</param>
/// <param name="OutputId">ID do output associado.</param>
/// <param name="InputContent">Conteúdo da entrada.</param>
/// <param name="ExpectedOutput">Saída esperada do caso de teste.</param>
public record TestCaseDto(Guid InputId, Guid OutputId, string InputContent, string ExpectedOutput);
