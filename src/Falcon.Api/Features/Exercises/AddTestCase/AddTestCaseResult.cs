using Falcon.Api.Features.Exercises.Shared;

namespace Falcon.Api.Features.Exercises.AddTestCase;

/// <summary>
/// Result returned after creating a test case.
/// </summary>
public record AddTestCaseResult(TestCaseDto TestCase); 
