using Falcon.Api.Features.Admin.GenerateTeacherToken;
using Falcon.Api.Features.Admin.GetCurrentToken;
using Falcon.Api.Features.Admin.GetStatistics;
using Falcon.Api.Features.Admin.GetUsers;
using Falcon.Api.Features.Auth.LoginUser;
using Falcon.Api.Features.Auth.RegisterUser;
using Falcon.Api.Features.Competitions.BlockGroup;
using Falcon.Api.Features.Competitions.CreateTemplate;
using Falcon.Api.Features.Competitions.FinishCompetition;
using Falcon.Api.Features.Competitions.GetCompetition;
using Falcon.Api.Features.Competitions.GetCompetitions;
using Falcon.Api.Features.Competitions.GetRanking;
using Falcon.Api.Features.Competitions.PromoteTemplate;
using Falcon.Api.Features.Competitions.RegisterGroup;
using Falcon.Api.Features.Competitions.StartCompetition;
using Falcon.Api.Features.Competitions.UnregisterGroup;
using Falcon.Api.Features.Exercises.AddTestCase;
using Falcon.Api.Features.Exercises.CreateExercise;
using Falcon.Api.Features.Exercises.GetExercise;
using Falcon.Api.Features.Exercises.GetExercises;
using Falcon.Api.Features.Exercises.RemoveTestCase;
using Falcon.Api.Features.Exercises.UpdateExercise;
using Falcon.Api.Features.Files.DeleteFile;
using Falcon.Api.Features.Files.DownloadFile;
using Falcon.Api.Features.Files.UploadFile;
using Falcon.Api.Features.Groups.AcceptInvite;
using Falcon.Api.Features.Groups.CreateGroup;
using Falcon.Api.Features.Groups.GetGroup;
using Falcon.Api.Features.Groups.InviteUser;
using Falcon.Api.Features.Groups.LeaveGroup;
using Falcon.Api.Features.Groups.RejectInvite;
using Falcon.Api.Features.Groups.RemoveMember;
using Falcon.Api.Features.Groups.UpdateGroup;
using Falcon.Api.Features.Logs.GetLogs;
using Falcon.Api.Features.Logs.GetUserLogs;
using Falcon.Api.Features.Submissions.GetAttempt;
using Falcon.Api.Features.Submissions.GetGroupAttempts;

namespace Falcon.Api.Extensions;

/// <summary>
/// Extension methods for mapping all API endpoints.
/// </summary>
public static class EndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        // Auth
        // TODO: app.MapRegisterUser();
        // TODO: app.MapLoginUser();

        // Groups
        // TODO: app.MapCreateGroup();
        // TODO: app.MapGetGroup();
        // TODO: app.MapUpdateGroup();
        // TODO: app.MapLeaveGroup();
        // TODO: app.MapInviteUser();
        // TODO: app.MapAcceptInvite();
        // TODO: app.MapRejectInvite();
        // TODO: app.MapRemoveMember();

        // Competitions
        // TODO: app.MapCreateTemplate();
        // TODO: app.MapPromoteTemplate();
        // TODO: app.MapGetCompetitions();
        // TODO: app.MapGetCompetition();
        // TODO: app.MapStartCompetition();
        // TODO: app.MapFinishCompetition();
        // TODO: app.MapRegisterGroup();
        // TODO: app.MapUnregisterGroup();
        // TODO: app.MapGetRanking();
        // TODO: app.MapBlockGroup();
        // TODO: app.MapAddExerciseToCompetition();
        // TODO: app.MapRemoveExerciseFromCompetition();

        // Exercises
        // TODO: app.MapCreateExercise();
        // TODO: app.MapGetExercise();
        // TODO: app.MapGetExercises();
        // TODO: app.MapUpdateExercise();
        // TODO: app.MapAddTestCase();
        // TODO: app.MapRemoveTestCase();
        // TODO: app.MapAttachFile();

        // Submissions
        // TODO: app.MapGetGroupAttempts();
        // TODO: app.MapGetAttempt();
        // TODO: app.MapGetAllAttempts();

        // Logs
        app.MapGetLogs();
        app.MapGetUserLogs();

        // Files
        app.MapUploadFile();
        app.MapDownloadFile();
        app.MapDeleteFile();

        // Admin
        app.MapGenerateTeacherToken();
        app.MapGetCurrentToken();
        app.MapGetUsers();
        app.MapGetStatistics();

        return app;
    }
}
