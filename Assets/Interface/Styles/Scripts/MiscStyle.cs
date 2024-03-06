using System.Collections.Generic;
using UnityEngine;

namespace Interface.Elements
{
    public class MiscStyle : CustomStyle<MiscStyle.Style>
    {
        public static MiscStyle Instance { get; private set; } = new();

        public enum Style
        {
            [InspectorName("Outline")] OUTLINE,
        }

        private static Dictionary<Style, string> classes = new()
        {
            { Style.OUTLINE, "outline-normal" },
        };

        public override Dictionary<Style, string> GetStyleClasses() => classes;
    }
}