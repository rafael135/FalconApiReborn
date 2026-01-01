using MediatR;

namespace Falcon.Api.Features.Admin.GetStatistics;

/// <summary>
/// Query to get system statistics.
/// </summary>
public record GetStatisticsQuery() : IRequest<GetStatisticsResult>;
