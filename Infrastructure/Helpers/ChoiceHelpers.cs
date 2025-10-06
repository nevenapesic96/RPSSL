using Domain.Enums;

namespace Infrastructure.Helpers;

/// <summary>
/// Group of methods used to ease use of choice enum
/// </summary>
public static class ChoiceHelpers
{
    /// <summary>
    /// Method that returns all possible choices
    /// </summary>
    /// <returns>List of choice enums</returns>
    public static IEnumerable<Choices> AllChoices()
    {
        return Enum.GetValues<Choices>();
    }
    
    /// <summary>
    /// Method that calculate the count of possible choice values
    /// </summary>
    /// <returns></returns>
    public static int AllChoicesCount()
    {
        return AllChoices().Count();
    }
}