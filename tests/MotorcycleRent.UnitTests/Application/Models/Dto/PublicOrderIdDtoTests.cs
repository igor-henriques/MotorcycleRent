namespace MotorcycleRent.UnitTests.Application.Models.Dto;

public sealed class PublicOrderIdDtoTests
{
    [Fact]
    public void TryParse_ValidInput_ReturnsTrueAndOutsDto()
    {
        // Arrange
        var input = "12345";

        // Act
        var result = PublicOrderIdDto.TryParse(input, out var dto);

        // Assert
        Assert.True(result);
        Assert.NotNull(dto);
        Assert.Equal(input, dto.PublicOrderId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void TryParse_InvalidInput_ReturnsFalseAndNullDto(string input)
    {
        // Act
        var result = PublicOrderIdDto.TryParse(input, out var dto);

        // Assert
        Assert.False(result);
        Assert.Null(dto);
    }

    [Fact]
    public void ImplicitOperator_StringToDto_ConvertsCorrectly()
    {
        // Arrange
        var input = "12345";

        // Act
        PublicOrderIdDto dto = input;

        // Assert
        Assert.Equal(input, dto.PublicOrderId);
    }

    [Fact]
    public void ImplicitOperator_DtoToString_ConvertsCorrectly()
    {
        // Arrange
        var publicOrderId = "12345";
        var dto = new PublicOrderIdDto { PublicOrderId = publicOrderId };

        // Act
        string output = dto;

        // Assert
        Assert.Equal(publicOrderId, output);
    }
}
