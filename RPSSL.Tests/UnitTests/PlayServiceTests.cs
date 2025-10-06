using Application.Common;
using Application.DTOs;
using Application.Services;
using AutoMapper;
using Domain;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace RPSSL.Tests.UnitTests;

[TestFixture]
public class PlayServiceTests
{
    private Mock<IChoicesService> _choicesServiceMock;
    private Mock<IPlayRepository> _playRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<PlayService>> _loggerMock;
    private AppSettings _appSettings;

    private PlayService _service;

    [SetUp]
    public void SetUp()
    {
        _choicesServiceMock = new Mock<IChoicesService>();
        _playRepositoryMock = new Mock<IPlayRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<PlayService>>();

        _appSettings = new AppSettings
        {
            LatestResultsCount = 10
        };

        _service = new PlayService(
            _choicesServiceMock.Object,
            _playRepositoryMock.Object,
            _appSettings,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Test]
    public async Task Play_ReturnsSuccess_WhenGameSaved()
    {
        // Arrange
        var request = new PlayRequest
        {
            Username = "testUser",
            PlayerChoice = (int)Choices.Rock
        };

        const Choices computerChoice = Choices.Scissors;
        _choicesServiceMock
            .Setup(c => c.GetValidRandomChoice(It.IsAny<CancellationToken>()))
            .ReturnsAsync(computerChoice);

        _playRepositoryMock
            .Setup(r => r.SaveGame(request.Username, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.Play(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccessful, Is.True);
        Assert.AreEqual(GameResult.Win, result.Result!.Results);
        Assert.Multiple(() =>
        {
            Assert.That(result.Result!.Player, Is.EqualTo(request.PlayerChoice));
            Assert.That(result.Result!.Computer, Is.EqualTo((int)computerChoice));
        });
    }
    
    [Test]
    public async Task Play_ReturnsFailure_WhenGameSaveFails()
    {
        // Arrange
        var request = new PlayRequest
        {
            Username = "testUser",
            PlayerChoice = (int)Choices.Paper
        };

        const Choices computerChoice = Choices.Spock;
        _choicesServiceMock
            .Setup(c => c.GetValidRandomChoice(It.IsAny<CancellationToken>()))
            .ReturnsAsync(computerChoice);

        _playRepositoryMock
            .Setup(r => r.SaveGame(request.Username, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.Play(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccessful, Is.False);
        Assert.AreEqual(GameResult.Win, result.Result!.Results);
    }
    
    [Test]
    public async Task Play_ReturnsTie_WhenPlayerAndComputerHaveSameChoice()
    {
        // Arrange
        const Choices choice = Choices.Rock;
        var request = new PlayRequest
        {
            Username = "testUser",
            PlayerChoice = (int)choice
        };

        _choicesServiceMock
            .Setup(c => c.GetValidRandomChoice(It.IsAny<CancellationToken>()))
            .ReturnsAsync(choice);

        _playRepositoryMock
            .Setup(r => r.SaveGame(request.Username, "Tie", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.Play(request, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccessful, Is.True);
            Assert.That(result.Result, Is.Not.Null);
        });
        Assert.AreEqual(GameResult.Tie, result.Result.Results);
        Assert.Multiple(() =>
        {
            Assert.That(result.Result.Player, Is.EqualTo(request.PlayerChoice));
            Assert.That(result.Result.Computer, Is.EqualTo(request.PlayerChoice));
        });
    }


    [Test]
    public async Task ResetResults_AllUsers_ReturnsSuccess_WhenDeleteSucceeds()
    {
        _playRepositoryMock
            .Setup(r => r.DeleteAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.ResetResults(null, CancellationToken.None);

        Assert.IsTrue(result.IsSuccessful);
        Assert.IsNull(result.Error);
    }

    [Test]
    public async Task ResetResults_AllUsers_ReturnsError_WhenDeleteFails()
    {
        _playRepositoryMock
            .Setup(r => r.DeleteAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.ResetResults(null, CancellationToken.None);

        Assert.IsFalse(result.IsSuccessful);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual(ApplicationErrorType.UnprocessableEntity, result.Error.Type);
    }

    [Test]
    public async Task ResetResults_SpecificUser_NotFound_ReturnsNotFound()
    {
        const string username = "unknownUser";

        _playRepositoryMock
            .Setup(r => r.GetResultsForUsername(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlayResult>());

        var result = await _service.ResetResults(username, CancellationToken.None);

        Assert.IsFalse(result.IsSuccessful);
        Assert.AreEqual(ApplicationErrorType.NotFound, result.Error!.Type);
    }

    [Test]
    public async Task ResetResults_SpecificUser_DeleteFails_ReturnsUnprocessableEntity()
    {
        const string username = "testUser";

        _playRepositoryMock
            .Setup(r => r.GetResultsForUsername(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlayResult> { new() { Username = "user1", Result = "Win" } });

        _playRepositoryMock
            .Setup(r => r.DeleteForUser(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.ResetResults(username, CancellationToken.None);

        Assert.IsFalse(result.IsSuccessful);
        Assert.AreEqual(ApplicationErrorType.UnprocessableEntity, result.Error!.Type);
    }

    [Test]
    public async Task ResetResults_SpecificUser_Success()
    {
        const string username = "testUser";

        _playRepositoryMock
            .Setup(r => r.GetResultsForUsername(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlayResult> { new() { Username = "user1", Result = "Win" } });

        _playRepositoryMock
            .Setup(r => r.DeleteForUser(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.ResetResults(username, CancellationToken.None);

        Assert.IsTrue(result.IsSuccessful);
    }

    [Test]
    public async Task GetLatestResults_ReturnsMappedResults()
    {
        // Arrange
        var latestCount = _appSettings.LatestResultsCount;

        var playResults = new List<PlayResult>
        {
            new() { Username = "user1", Result = "Win" },
            new() { Username = "user2", Result = "Lose" }
        };

        var mappedResults = new List<ResultResponse>
        {
            new() { Username = "user1", Result = GameResult.Win },
            new() { Username = "user2", Result = GameResult.Lose }
        };

        _playRepositoryMock
            .Setup(r => r.GetLastResults(latestCount, It.IsAny<CancellationToken>()))
            .ReturnsAsync(playResults);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<ResultResponse>>(playResults))
            .Returns(mappedResults);

        // Act
        var result = await _service.GetLatestResults(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(mappedResults));
    }
}