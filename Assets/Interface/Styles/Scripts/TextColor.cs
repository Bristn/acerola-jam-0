using System.Collections.Generic;
using UnityEngine;

namespace Interface.Elements
{
    public class TextColor : CustomStyle<TextColor.Color>
    {
        public static TextColor Instance { get; private set; } = new();

        public enum Color
        {
            [InspectorName("On Primary")] ON_PRIMARY,
            [InspectorName("On Background")] ON_BACKGROUND,
            [InspectorName("On Background Dark")] ON_BACKGROUND_DARK,

            [InspectorName("Primary")] PRIMARY,
            [InspectorName("Secondary")] SECONDARY,
            [InspectorName("Accent")] ACCENT,
            [InspectorName("Background")] BACKGROUND,
            [InspectorName("Background - Light")] BACKGROUND_LIGHT,
            [InspectorName("Foreground")] FOREGROUND,
        }

        private static Dictionary<Color, string> classes = new()
        {
            { Color.ON_PRIMARY, "text-on-primary" },
            { Color.ON_BACKGROUND, "text-on-background" },
            { Color.ON_BACKGROUND_DARK, "text-on-background-dark" },

            { Color.PRIMARY, "text-primary" },
            { Color.SECONDARY, "text-secondary" },
            { Color.ACCENT, "text-accent" },
            { Color.BACKGROUND, "text-background" },
            { Color.BACKGROUND_LIGHT, "text-background-light" },
            { Color.FOREGROUND, "text-foreground" },
        };

        public static Dictionary<BackgroundColor.Color, Color> OnBackgroundClasses = new()
        {
            { BackgroundColor.Color.PRIMARY, Color.ON_PRIMARY },
            { BackgroundColor.Color.SECONDARY, Color.ON_PRIMARY},
            { BackgroundColor.Color.FOREGROUND, Color.ON_PRIMARY },

            { BackgroundColor.Color.ACCENT, Color.ON_BACKGROUND },
            { BackgroundColor.Color.DARK_900, Color.ON_BACKGROUND },
            { BackgroundColor.Color.DARK_800, Color.ON_BACKGROUND },
            { BackgroundColor.Color.DARK_700, Color.ON_BACKGROUND },
        };

        public override Dictionary<Color, string> GetStyleClasses() => classes;
    }
}