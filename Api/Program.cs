using Api;
using Api.Configuration;
using Application.DTOs;
using Application.Mappers;
using Application.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.ApiClients.BoohmaClient;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.InitializeAndValidateAppSettings();

// Add services to the container.

builder.Services.AddScoped<IChoicesService, ChoicesService>();
builder.Services.AddScoped<IPlayService, PlayService>();
builder.Services.AddScoped<IPlayRepository, PlayRepository>();

builder.Services.AddHttpClient<IBoohmaApiClient, BoohmaApiApiClient>()
    .ConfigureHttpClient(client => client.BaseAddress = appSettings.BoohmaServiceSettings.BaseUrl);

builder.Services.AddAutoMapper(x =>
{
    x.AddProfile<ChoiceMappingProfile>();
    x.AddProfile<ResultsMappingProfile>();
});

builder.Services.AddSingleton(appSettings);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<PlayRequest>, PlayRequestValidator>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapEndpoints();

await app.RunAsync();

//gitignore fajl
//README
//global error page -> proveri da li moze da se pojednostavi
//docker