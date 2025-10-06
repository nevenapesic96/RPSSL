using Application.DTOs;
using Application.Services;
using AutoMapper;
using Domain.Enums;
using Infrastructure.ApiClients.BoohmaClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RPSSL.Tests.IntegrationTests;

[TestFixture]
public class ChoicesServiceIntegrationTests
{
    private IMapper _mapper;
    private ILogger<IChoicesService> _logger;
    private IBoohmaApiClient _boohmaApiClient;

    [SetUp]
    public void Setup()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Choices, ChoiceResponse>();
        }, NullLoggerFactory.Instance);
        _mapper = config.CreateMapper();

        _logger = new NullLogger<ChoicesService>();

        _boohmaApiClient = new FakeBoohmaApiClient();
    }

    [Test]
    public async Task GetValidRandomChoice_ShouldReturnValidChoice()
    {
        //Arrange
        var service = new ChoicesService(_boohmaApiClient, _mapper, _logger);

        //Act
        var result = await service.GetValidRandomChoice(CancellationToken.None);

        //Assert
        Assert.That(Enum.IsDefined(typeof(Choices), result), Is.True);
    }

    [Test]
    public async Task GetRandomChoice_ShouldReturnMappedChoiceResponse()
    {
        //Arrange
        var service = new ChoicesService(_boohmaApiClient, _mapper, _logger);

        //Act
        var result = await service.GetRandomChoice(CancellationToken.None);

        //Assert
        Assert.That(result, Is.Not.Null);
    }
}

public class FakeBoohmaApiClient : IBoohmaApiClient
{
    public Task<RandomNumberResponse> GetRandomNumber(CancellationToken cancellationToken)
    {
        return Task.FromResult(new RandomNumberResponse { RandomNumber = 3 });
    }
}

