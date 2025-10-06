using Domain.Enums;
using FluentValidation;

namespace Application.DTOs;

/// <summary>
/// Object that contains all details about user choice
/// </summary>
public record PlayRequest
{
    /// <summary>
    /// Identifier of choice that user selected
    /// </summary>
    public int PlayerChoice { get; init; }

    /// <summary>
    /// Name of the user playing the game
    /// </summary>
    public string Username { get; set; }
}

public class PlayRequestValidator : AbstractValidator<PlayRequest>
{
    public PlayRequestValidator()
    {
        RuleFor(x => x.PlayerChoice)
            .Must(v => Enum.IsDefined(typeof(Choices), v))
            .WithMessage("Move not valid. It has to be one of the possible choices.");
        RuleFor(x => x.Username).NotEmpty();
    }
}