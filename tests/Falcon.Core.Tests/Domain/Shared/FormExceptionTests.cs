using Falcon.Core.Domain.Shared.Exceptions;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Shared;

public class FormExceptionTests
{
    [Fact]
    public void Constructor_Should_CreateException_WithErrorDictionary()
    {
        // Arrange
        var errors = new Dictionary<string, string>
        {
            { "email", "E-mail já utilizado" },
            { "ra", "RA já cadastrado" }
        };

        // Act
        var exception = new FormException(errors);

        // Assert
        exception.Should().NotBeNull();
        exception.Errors.Should().BeEquivalentTo(errors);
        exception.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void FormException_Should_ContainFieldLevelErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string>
        {
            { "password", "Senha deve ter no mínimo 8 caracteres" },
            { "name", "Nome é obrigatório" }
        };

        // Act
        var exception = new FormException(errors);

        // Assert
        exception.Errors["password"].Should().Be("Senha deve ter no mínimo 8 caracteres");
        exception.Errors["name"].Should().Be("Nome é obrigatório");
    }

    [Fact]
    public void FormException_Should_BeOfTypeDomainException()
    {
        // Arrange
        var errors = new Dictionary<string, string> { { "field", "error" } };

        // Act
        var exception = new FormException(errors);

        // Assert
        exception.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void FormException_Should_AllowEmptyErrorsDictionary()
    {
        // Arrange
        var errors = new Dictionary<string, string>();

        // Act
        var exception = new FormException(errors);

        // Assert
        exception.Errors.Should().BeEmpty();
    }
}
