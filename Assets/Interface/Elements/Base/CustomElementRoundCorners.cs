using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public static class CustomElementRoundCorners
    {
        public enum CornerType
        {
            [InspectorName("None")] NONE,
            [InspectorName("Small")] SMALL,
            [InspectorName("Normal")] NORMAL
        }

        private static readonly Dictionary<CornerType, float> cornerRadius = new() {
            { CornerType.NONE, 0 },
            { CornerType.SMALL, 5 },
            { CornerType.NORMAL, 15 },
        };

        /// <summary>
        /// Setst the corner radius of the current element to one of the pre-defined values
        /// </summary>
        /// <param name="element">The element to change the corners of</param>
        /// <param name="topLeft">The type of the top left corner</param>
        /// <param name="topRight">The type of the top right corner</param>
        /// <param name="botRight">The type of the bottom right</param>
        /// <param name="botLeft">The type of the bottom left</param>
        public static void SetRoundCorners(this VisualElement element, CornerType topLeft, CornerType topRight, CornerType botRight, CornerType botLeft)
        {
            if (element == null)
            {
                return;
            }

            cornerRadius.TryGetValue(topLeft, out float topLeftCorner);
            cornerRadius.TryGetValue(topRight, out float topRightCorner);
            cornerRadius.TryGetValue(botRight, out float botRightCorner);
            cornerRadius.TryGetValue(botLeft, out float botLeftCorner);
            element.style.borderTopLeftRadius = topLeftCorner;
            element.style.borderTopRightRadius = topRightCorner;
            element.style.borderBottomRightRadius = botRightCorner;
            element.style.borderBottomLeftRadius = botLeftCorner;
        }
    }
}
