using System.Collections.Immutable;
using Domain.Enums;

namespace Domain;

public static class PlayRules
{
    /// <summary>
    /// Collection containing rules of the game
    /// </summary>
    public static readonly ImmutableDictionary<Choices, ImmutableArray<Choices>> Rules =
        ImmutableDictionary.CreateRange(new[]
        {
            new KeyValuePair<Choices, ImmutableArray<Choices>>(Choices.Rock, [Choices.Lizard, Choices.Scissors]),
            new KeyValuePair<Choices, ImmutableArray<Choices>>(Choices.Lizard, [Choices.Paper, Choices.Spock]),
            new KeyValuePair<Choices, ImmutableArray<Choices>>(Choices.Spock, [Choices.Rock, Choices.Scissors]),
            new KeyValuePair<Choices, ImmutableArray<Choices>>(Choices.Scissors, [Choices.Lizard, Choices.Paper]),
            new KeyValuePair<Choices, ImmutableArray<Choices>>(Choices.Paper, [Choices.Rock, Choices.Spock]),
        });
}