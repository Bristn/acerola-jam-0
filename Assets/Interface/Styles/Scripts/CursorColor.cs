using System.Collections.Generic;
using UnityEngine;

namespace Interface.Elements
{
    public class CursorColor : CustomStyle<CursorColor.Color>
    {
        public static CursorColor Instance { get; private set; } = new();

        public enum Color
        {
            [InspectorName("Primary")] PRIMARY,
            [InspectorName("Secondary")] SECONDARY,
            [InspectorName("Accent")] ACCENT,
            [InspectorName("Background")] BACKGROUND,
            [InspectorName("Background - Light")] BACKGROUND_LIGHT,
            [InspectorName("Foreground")] FOREGROUND,
        }

        private static Dictionary<Color, string> classes = new()
        {
            { Color.PRIMARY, "cursor-primary" },
            { Color.SECONDARY, "cursor-secondary" },
            { Color.ACCENT, "cursor-accent" },
            { Color.BACKGROUND, "cursor-background" },
            { Color.BACKGROUND_LIGHT, "cursor-background-light" },
            { Color.FOREGROUND, "cursor-foreground" },
        };

        public override Dictionary<Color, string> GetStyleClasses() => classes;
    }
}