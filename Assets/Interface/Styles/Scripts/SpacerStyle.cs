using System.Collections.Generic;
using UnityEngine;

namespace Interface.Elements
{
    public class SpacerStyle : CustomStyle<SpacerStyle.Style>
    {
        public static SpacerStyle Instance { get; private set; } = new();

        public enum Style
        {
            [InspectorName("Horizontal - Normal")] HORIZONTAL_NORMAL,
            [InspectorName("Horizontal - Half")] HORIZONTAL_HALF,
            [InspectorName("Vertical - Normal")] VERTICAL_NORMAL,
            [InspectorName("Vertical - Half")] VERTICAL_HALF,
        }

        private static Dictionary<Style, string> classes = new()
        {
            { Style.HORIZONTAL_NORMAL, "horizontal-normal" },
            { Style.HORIZONTAL_HALF, "horizontal-half" },
            { Style.VERTICAL_NORMAL, "vertical-normal" },
            { Style.VERTICAL_HALF, "vertical-half" },
        };

        public override Dictionary<Style, string> GetStyleClasses() => classes;
    }
}