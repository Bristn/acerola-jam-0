using UnityEngine;
using UnityEngine.UIElements;
using static Interface.Elements.CustomElementRoundCorners;

namespace Interface.Elements
{
    public static class GridElementRoundCorners
    {
        /// <summary>
        /// Update the rounded corners of the given element based on the grid configuration and the neighboring elements
        /// </summary>
        /// <param name="element">The element to update the corners of</param>
        /// <param name="index">The current element hierachy index</param>
        /// <param name="entries">A count of all entries in the grid</param>
        /// <param name="rows">A count of used rows</param>
        /// <param name="columns">A count of used columns</param>
        public static void UpdateCorners(CustomElement element, int index, int entries, int rows, int columns)
        {
            UpdateCorners(element.RoundedCornersTarget, index, entries, rows, columns);
        }

        /// <summary>
        /// Update the rounded corners of the given element based on the grid configuration and the neighboring elements
        /// Values of -1 for columns or rows indicates that the linear layout is reversed
        /// </summary>
        /// <param name="element">The element to update the corners of</param>
        /// <param name="index">The current element hierachy index</param>
        /// <param name="entries">A count of all entries in the grid</param>
        /// <param name="rows">A count of used rows</param>
        /// <param name="columns">A count of used columns</param>
        public static void UpdateCorners(VisualElement element, int index, int entries, int rows, int columns)
        {
            if (rows == 1)
            {
                Debug.Log("GridElementRoundCorners: Update horizontal corners");
                UpdateCornersOfHorizontalLayout(element, index, entries);
                return;
            }

            if (rows == -1)
            {
                Debug.Log("GridElementRoundCorners: Update reversed horizontal corners");
                UpdateCornersOfHorizontalLayout(element, entries - index - 1, entries);
                return;
            }

            if (columns == 1)
            {
                Debug.Log("GridElementRoundCorners: Update vertical corners");
                UpdateCornersOfVerticalLayout(element, index, entries);
                return;
            }

            if (columns == -1)
            {
                Debug.Log("GridElementRoundCorners: Update reversed vertical corners");
                UpdateCornersOfVerticalLayout(element, entries - index - 1, entries);
                return;
            }

            Debug.Log("GridElementRoundCorners: Update grid corners");

            // This is a actual grid layout with more than one row x column
            int indexAbove = index - columns;
            int indexBelow = index + columns;
            bool hasAbove = indexAbove >= 0;
            bool hasBelow = indexBelow <= entries - 1;
            bool isLeftMost = index % columns == 0;
            bool isRightMost = (index + 1) % columns == 0 || index == entries - 1;

            bool roundTopLeft = !hasAbove && isLeftMost;
            bool roundTopRight = !hasAbove && isRightMost;
            bool roundBottomRight = !hasBelow && isRightMost;
            bool roundBottomLeft = !hasBelow && isLeftMost;

            element.SetRoundCorners(
                roundTopLeft ? CornerType.NORMAL : CornerType.SMALL,
                roundTopRight ? CornerType.NORMAL : CornerType.SMALL,
                roundBottomRight ? CornerType.NORMAL : CornerType.SMALL,
                roundBottomLeft ? CornerType.NORMAL : CornerType.SMALL
            );
        }

        /// <summary>
        /// Update the rounded corners as if the grid layout were a horizontal layout.
        /// </summary>
        /// <param name="element">The element to update the corners of</param>
        /// <param name="index">The current element hierachy index</param>
        /// <param name="entries">A count of all entries in the grid</param>
        private static void UpdateCornersOfHorizontalLayout(VisualElement element, int index, int entries)
        {
            bool roundLeft = index == 0;
            bool roundRight = index == entries - 1;

            CornerType left = roundLeft ? CornerType.NORMAL : CornerType.SMALL;
            CornerType right = roundRight ? CornerType.NORMAL : CornerType.SMALL;

            element.SetRoundCorners(left, right, right, left);
        }

        /// <summary>
        /// Update the rounded corners as if the grid layout were a vertical layout.
        /// </summary>
        /// <param name="element">The element to update the corners of</param>
        /// <param name="index">The current element hierachy index</param>
        /// <param name="entries">A count of all entries in the grid</param>
        private static void UpdateCornersOfVerticalLayout(VisualElement element, int index, int entries)
        {
            bool roundTop = index == 0;
            bool roundBottom = index == entries - 1;

            CornerType top = roundTop ? CornerType.NORMAL : CornerType.SMALL;
            CornerType bottom = roundBottom ? CornerType.NORMAL : CornerType.SMALL;

            element.SetRoundCorners(top, top, bottom, bottom);
        }
    }
}
