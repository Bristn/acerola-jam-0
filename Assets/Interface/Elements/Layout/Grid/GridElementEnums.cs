using UnityEngine;

namespace Interface.Elements
{
    /// <summary>
    /// Describes the how the grid cells should be distributed
    /// </summary>
    public enum FitType
    {
        [InspectorName("Automatic uniform")] UNIFORM,
        [InspectorName("Automatic prio width")] WIDTH,
        [InspectorName("Automatic prio height")] HEIGHT,
        [InspectorName("Set row count")] FIXED_ROWS,
        [InspectorName("Set column count")] FIXED_COLUMNS,
    }

    /// <summary>
    /// Describes how the grid celss should fill the parent container
    /// </summary>
    public enum FillType
    {
        [InspectorName("None")] NONE,
        [InspectorName("Horizontal")] HORIZONTAL,
        [InspectorName("Vertical")] VERTICAL,
        [InspectorName("Both")] BOTH,
    }

    /// <summary>
    /// Describes the behaviour if the maximum count of selections are already present and 
    // a non selected entry is clicked
    /// </summary>
    public enum SelectionOverflowType
    {
        [InspectorName("None (Prevent selection)")] NONE,
        [InspectorName("Deselect oldest")] DESELECT_FIRST,
    }

    /// <summary>
    /// Sepcifies if this GridElement manages the rounded corners of it's children.
    /// If all children are selected, the firs and last child will have rounded corners
    /// with a bigger radius compared to the children in the middle of the collection
    /// </summary>
    public enum RoundChildren
    {
        [InspectorName("Don't round children")] NONE,
        [InspectorName("Round all chidlren")] ALL,
        [InspectorName("Ignore first child")] IGNORE_FIRST,
        [InspectorName("Ignore last child")] IGNORE_LAST,
        [InspectorName("Ignore first & last child")] IGNORE_ENDS
    }
}