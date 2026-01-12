using Falcon.Api.Features.Competitions.Shared;
using Falcon.Api.Features.Submissions.Shared;
using Falcon.Core.Domain.Shared.Enums;
using MediatR;

namespace Falcon.Api.Features.Submissions.SubmitAttempt;

/// <summary>
/// Comando para submeter uma tentativa de exercício para avaliação assincrona.
/// </summary>
/// <param name="CompetitionId">Id da competição (opcional dependendo do contexto).</param>
/// <param name="ExerciseId">Id do exercício.</param>
/// <param name="Code">Código fonte enviado.</param>
/// <param name="Language">Linguagem de programação.</param>
public record SubmitAttemptCommand(
    Guid CompetitionId,
    Guid ExerciseId,
    string Code,
    LanguageType Language
) : IRequest<SubmitAttemptResult>; 
