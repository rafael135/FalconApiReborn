namespace Falcon.Api.Features.Auth.Shared;

public record UserDto(string Id, string Name, string Email, string Ra, string Role, GroupDto? Group);