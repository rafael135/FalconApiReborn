using Falcon.Api.Features.Exercises.Shared;

namespace Falcon.Api.Features.Exercises.AddTestCase;

/// <summary>
/// Resultado retornado após a criação de um caso de teste.
/// </summary>
public record AddTestCaseResult(TestCaseDto TestCase); 
