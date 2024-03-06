using System.Collections.Generic;
using UnityEngine;

namespace Interface.Elements
{
    public class FontStyle : CustomStyle<FontStyle.Style>
    {
        public static FontStyle Instance { get; private set; } = new();

        public enum Style
        {
            [InspectorName("Normal")] NORMAL,
            [InspectorName("Icon solid")] ICON_SOLID,
            [InspectorName("Icon regular")] ICON_REGULAR,
            [InspectorName("Icon brands")] ICON_BRANDS,
        }

        private static Dictionary<Style, string> classes = new()
        {
            { Style.NORMAL, "text-normal" },
            { Style.ICON_SOLID, "text-icon-solid" },
            { Style.ICON_REGULAR, "text-icon-regular" },
            { Style.ICON_BRANDS, "text-icon-brands" },
        };

        public override Dictionary<Style, string> GetStyleClasses() => classes;
    }
}