using System.Collections.Generic;
using UnityEngine;

namespace Interface.Elements
{
    public class RoundedStyle : CustomStyle<RoundedStyle.Style>
    {
        public static RoundedStyle Instance { get; private set; } = new();

        public enum Style
        {
            [InspectorName("None")] NONE,
            [InspectorName("Small")] SMALL,
            [InspectorName("Normal")] NORMAL,
        }

        private static Dictionary<Style, string> classes = new()
        {
            { Style.NONE, "rounded-none" },
            { Style.SMALL, "rounded-small" },
            { Style.NORMAL, "rounded" },
        };

        public override Dictionary<Style, string> GetStyleClasses() => classes;
    }
}