using Application.DTOs;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Infrastructure.ApiClients.BoohmaClient;
using Microsoft.Extensions.Logging;
using Moq;

namespace RPSSL.Tests.UnitTests;

[TestFixture]
public class ChoicesServiceTests
{
    private Mock<IBoohmaApiClient> _boohmaApiClientMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<IChoicesService>> _loggerMock;
    private IChoicesService _service;

    [SetUp]
    public void SetUp()
    {
        _boohmaApiClientMock = new Mock<IBoohmaApiClient>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<IChoicesService>>();

        _service = new ChoicesService(
            _boohmaApiClientMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Test]
    public async Task GetValidRandomChoice_ReturnsValidEnum()
    {
        // Arrange
        var fakeRandom = 7;
        var expectedChoice = (Choices)(fakeRandom % 5 + 1);

        _boohmaApiClientMock
            .Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RandomNumberResponse { RandomNumber = fakeRandom });

        // Act
        var result = await _service.GetValidRandomChoice(CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedChoice));
    }

    [Test]
    public async Task GetRandomChoice_ReturnsMappedChoice()
    {
        // Arrange
        var choiceEnum = Choices.Scissors;
        var responseDto = new ChoiceResponse { Name = "Scissors" };

        _boohmaApiClientMock
            .Setup(x => x.GetRandomNumber(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RandomNumberResponse { RandomNumber = 2 });

        _mapperMock
            .Setup(m => m.Map<ChoiceResponse>(choiceEnum))
            .Returns(responseDto);
        
        // Act
        var result = await _service.GetRandomChoice(CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(responseDto));
    }
}
