using System.Collections.Generic;

public class CustomPseudoStates
{
    /// <summary>
    /// Stores all different pseudo states 
    /// </summary>
    public enum States
    {
        TEST,
        BUTTON_HOVER,
        CARD_HOVER,
    }

    public static readonly char Delimiter = '_';

    /// <summary>
    /// Maps the state enum to the class suffixes. When adding a pseudo state to
    /// an object, all already present style will be iterated. For every style class
    /// present, a new entry will be created with the added suffix for this state.
    /// </summary>
    public static Dictionary<States, string> ClassSuffix = new()
    {
        { States.TEST, Delimiter + "test" },
        { States.BUTTON_HOVER, Delimiter + "button-hover" },
        { States.CARD_HOVER, Delimiter + "card-hover" },
    };
}