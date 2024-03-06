using System.Collections.Generic;
using UnityEngine;

namespace Interface.Elements
{
    public class OutlineColor : CustomStyle<OutlineColor.Color>
    {
        public static OutlineColor Instance { get; private set; } = new();

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
            { Color.PRIMARY, "outline-primary" },
            { Color.SECONDARY, "outline-secondary" },
            { Color.ACCENT, "outline-accent" },
            { Color.BACKGROUND, "outline-background" },
            { Color.BACKGROUND_LIGHT, "outline-background-light" },
            { Color.FOREGROUND, "outline-foreground" },
        };

        public override Dictionary<Color, string> GetStyleClasses() => classes;
    }

}