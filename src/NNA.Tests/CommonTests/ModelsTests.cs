using FluentAssertions;
using NNA.Domain.DTOs;
using Xunit;

namespace NNA.Tests.CommonTests;

public sealed class ModelsTests {
    [Fact]
    public void AllDtoShouldInheritBaseDtoClass() {
        //Arrange
        var allDtos = typeof(BaseDto).Assembly.Types().Where(t => t.Name.EndsWith("Dto") && t.Name != nameof(BaseDto))
            .ToList();

        //Assert
        allDtos.ForEach(dto => dto.Should().BeDerivedFrom<BaseDto>());
    }
}