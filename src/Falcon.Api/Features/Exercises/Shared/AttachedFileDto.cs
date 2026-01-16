namespace Falcon.Api.Features.Exercises.Shared;

/// <summary>
/// Attached file information.
/// </summary>
public record AttachedFileDto(Guid Id, string Name, string Type, long Size, DateTime CreatedAt);
