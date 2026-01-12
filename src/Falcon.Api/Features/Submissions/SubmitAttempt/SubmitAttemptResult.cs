using Falcon.Api.Features.Competitions.Shared;
using Falcon.Api.Features.Submissions.Shared;

namespace Falcon.Api.Features.Submissions.SubmitAttempt;

/// <summary>
/// Resultado inicial de uma submissão: tentativa criada e posição no ranking.
/// </summary>
/// <param name="Attempt">Detalhes da tentativa.</param>
/// <param name="Ranking">Entrada do ranking relevante.</param>
public record SubmitAttemptResult(AttemptDto Attempt, RankingEntryDto Ranking); 
