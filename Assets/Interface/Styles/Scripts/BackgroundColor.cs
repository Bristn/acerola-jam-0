using System.Collections.Generic;
using UnityEngine;

namespace Interface.Elements
{
    public class BackgroundColor : CustomStyle<BackgroundColor.Color>
    {
        public static BackgroundColor Instance { get; private set; } = new();

        public enum Color
        {
            [InspectorName("Primary")] PRIMARY,
            [InspectorName("Secondary")] SECONDARY,
            [InspectorName("Accent")] ACCENT,
            [InspectorName("Dark 900")] DARK_900,
            [InspectorName("Dark 800")] DARK_800,
            [InspectorName("Dark 700")] DARK_700,
            [InspectorName("Foreground")] FOREGROUND,
            [InspectorName("Transparent")] TRANSPARENT,
        }

        private static Dictionary<Color, string> classes = new()
        {
            { Color.PRIMARY, "background-primary" },
            { Color.SECONDARY, "background-secondary" },
            { Color.ACCENT, "background-accent" },
            { Color.DARK_900, "background-dark-900" },
            { Color.DARK_800, "background-dark-800" },
            { Color.DARK_700, "background-dark-700" },
            { Color.FOREGROUND, "background-foreground" },
            { Color.TRANSPARENT, "background-transparent" },
        };

        public override Dictionary<Color, string> GetStyleClasses() => classes;
    }

}