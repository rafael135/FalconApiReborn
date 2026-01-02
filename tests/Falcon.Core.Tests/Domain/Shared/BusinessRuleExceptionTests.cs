using Falcon.Core.Domain.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Falcon.Core.Tests.Domain.Shared;

public class BusinessRuleExceptionTests
{
    [Fact]
    public void Constructor_Should_CreateException_WithBusinessRuleMessage()
    {
        // Arrange
        var mockRule = new Mock<IBusinessRule>();
        mockRule.Setup(r => r.Message).Returns("Test business rule message");
        mockRule.Setup(r => r.IsBroken()).Returns(true);

        // Act
        var exception = new BusinessRuleException(mockRule.Object);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain("Test business rule message");
        exception.BrokenRule.Should().Be(mockRule.Object);
    }

    [Fact]
    public void BusinessRuleException_Should_BeOfTypeDomainException()
    {
        // Arrange
        var mockRule = new Mock<IBusinessRule>();
        mockRule.Setup(r => r.Message).Returns("Test message");

        // Act
        var exception = new BusinessRuleException(mockRule.Object);

        // Assert
        exception.Should().BeAssignableTo<DomainException>();
    }
}
