using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Users;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Admin.GetStatistics;

/// <summary>
/// Handler for getting system statistics.
/// </summary>
public class GetStatisticsHandler : IRequestHandler<GetStatisticsQuery, GetStatisticsResult>
{
    private readonly FalconDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<GetStatisticsHandler> _logger;

    public GetStatisticsHandler(
        FalconDbContext context,
        UserManager<User> userManager,
        ILogger<GetStatisticsHandler> logger
    )
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<GetStatisticsResult> Handle(
        GetStatisticsQuery request,
        CancellationToken cancellationToken
    )
    {
        // User statistics
        var totalStudents = (await _userManager.GetUsersInRoleAsync("Student")).Count;
        var totalTeachers = (await _userManager.GetUsersInRoleAsync("Teacher")).Count;
        var totalAdmins = (await _userManager.GetUsersInRoleAsync("Admin")).Count;
        var totalUsers = totalStudents + totalTeachers + totalAdmins;

        var userStats = new UserStatistics(totalStudents, totalTeachers, totalAdmins, totalUsers);

        // Group statistics
        var totalGroups = await _context.Groups.CountAsync(cancellationToken);

        // Competition statistics
        var totalPending = await _context.Competitions.CountAsync(
            c => c.Status == CompetitionStatus.Pending,
            cancellationToken
        );
        var totalOngoing = await _context.Competitions.CountAsync(
            c => c.Status == CompetitionStatus.Ongoing,
            cancellationToken
        );
        var totalFinished = await _context.Competitions.CountAsync(
            c => c.Status == CompetitionStatus.Finished,
            cancellationToken
        );
        var totalCompetitions = totalPending + totalOngoing + totalFinished;

        var competitionStats = new CompetitionStatistics(
            totalPending,
            totalOngoing,
            totalFinished,
            totalCompetitions
        );

        // Exercise statistics
        var exercisesByType = await _context
            .Exercises.Include(e => e.ExerciseType)
            .GroupBy(e => e.ExerciseType.Label)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Type, x => x.Count, cancellationToken);

        var totalExercises = exercisesByType.Values.Sum();

        var exerciseStats = new ExerciseStatistics(exercisesByType, totalExercises);

        // Submission statistics
        var totalSubmissions = await _context.GroupExerciseAttempts.CountAsync(cancellationToken);
        var acceptedSubmissions = await _context.GroupExerciseAttempts.CountAsync(
            a => a.Accepted,
            cancellationToken
        );
        var acceptanceRate =
            totalSubmissions > 0 ? (double)acceptedSubmissions / totalSubmissions * 100 : 0;

        var submissionStats = new SubmissionStatistics(
            totalSubmissions,
            acceptedSubmissions,
            acceptanceRate
        );

        _logger.LogInformation(
            "System statistics: {Users} users, {Groups} groups, {Competitions} competitions, {Exercises} exercises, {Submissions} submissions",
            totalUsers,
            totalGroups,
            totalCompetitions,
            totalExercises,
            totalSubmissions
        );

        return new GetStatisticsResult(
            userStats,
            totalGroups,
            competitionStats,
            exerciseStats,
            submissionStats
        );
    }
}
