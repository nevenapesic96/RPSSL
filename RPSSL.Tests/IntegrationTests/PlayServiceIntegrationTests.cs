using Application.Common;
using Application.DTOs;
using Application.Services;
using AutoMapper;
using Domain;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace RPSSL.Tests.IntegrationTests;

[TestFixture]
public class PlayServiceIntegrationTests
{
    private PlayService _playService;
    private Mock<IChoicesService> _choicesServiceMock;
    private Mock<IPlayRepository> _playRepositoryMock;
    private IMapper _mapper;
    private AppSettings _appSettings;
    private ILogger<PlayService> _logger;

    [SetUp]
    public void Setup()
    {
        _choicesServiceMock = new Mock<IChoicesService>();
        _playRepositoryMock = new Mock<IPlayRepository>();
        _logger = NullLogger<PlayService>.Instance;

        _appSettings = new AppSettings { LatestResultsCount = 5 };

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<PlayResult, ResultResponse>();
        }, NullLoggerFactory.Instance);
        _mapper = config.CreateMapper();

        _playService = new PlayService(
            _choicesServiceMock.Object,
            _playRepositoryMock.Object,
            _appSettings,
            _mapper,
            _logger);
    }

    [Test]
    public async Task Play_ShouldReturnWin_WhenPlayerBeatsComputer()
    {
        // Arrange
        const Choices playerChoice = Choices.Rock;
        const Choices computerChoice = Choices.Scissors;
        const string username = "testuser";

        _choicesServiceMock.Setup(x => x.GetValidRandomChoice(It.IsAny<CancellationToken>()))
            .ReturnsAsync(computerChoice);

        _playRepositoryMock.Setup(x => x.SaveGame(username, GameResult.Win.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new PlayRequest { PlayerChoice = (int)playerChoice, Username = username };

        // Act
        var result = await _playService.Play(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccessful, Is.True);
        Assert.AreEqual(GameResult.Win, result.Result!.Results);
        Assert.That(result.Result.Computer, Is.EqualTo((int)computerChoice));
        Assert.That(result.Result.Player, Is.EqualTo((int)playerChoice));

        _playRepositoryMock.Verify(x => x.SaveGame(username, GameResult.Win.ToString(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ResetResults_ShouldDeleteAll_WhenUsernameIsNull()
    {
        // Arrange
        _playRepositoryMock.Setup(x => x.DeleteAll(It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _playService.ResetResults(null, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        _playRepositoryMock.Verify(x => x.DeleteAll(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task ResetResults_ShouldReturnNotFound_WhenUserHasNoResults()
    {
        var username = "user1";
        _playRepositoryMock.Setup(x => x.GetResultsForUsername(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlayResult>());

        // Act
        var result = await _playService.ResetResults(username, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.IsSuccessful);
        Assert.AreEqual(ApplicationErrorType.NotFound, result.Error.Type);
    }

    [Test]
    public async Task GetLatestResults_ShouldReturnMappedResults()
    {
        var results = new List<PlayResult>
        {
            new() { Username = "user1", Result = "Win" },
            new() { Username = "user2", Result = "Lose" }
        };

        _playRepositoryMock.Setup(x => x.GetLastResults(_appSettings.LatestResultsCount, It.IsAny<CancellationToken>()))
            .ReturnsAsync(results);

        var resultResponses = await _playService.GetLatestResults(CancellationToken.None);

        Assert.That(resultResponses, Is.Not.Null);
        Assert.That(resultResponses.Count(), Is.EqualTo(results.Count));
    }
}