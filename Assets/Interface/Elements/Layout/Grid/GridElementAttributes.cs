using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class GridElement : VisualElement
    {
        /* --- Values --- */

        private RoundChildren roundChildren = RoundChildren.ALL;
        private FitType fit = FitType.WIDTH;
        private int rows;
        private int columns;
        private Vector2 cellSize = Vector2.zero;
        private Vector2 spacing = Vector2.zero;
        private FillType fill;
        private bool hasGeometryEvent;
        private bool hasAttachEvent;

        /* --- Properties  --- */

        public RoundChildren RoundChildren
        {
            get => this.roundChildren;
            set => this.roundChildren = value;
        }

        public FitType Fit
        {
            get => this.fit;
            set => this.fit = value;
        }

        public int Rows
        {
            get => this.rows;
            set => this.rows = value;
        }

        public int Columns
        {
            get => this.columns;
            set => this.columns = value;
        }

        public float CellWidth
        {
            get => this.cellSize.x;
            set => this.cellSize.x = value;
        }

        public float CellHeight
        {
            get => this.cellSize.y;
            set => this.cellSize.y = value;
        }

        public float Spacing
        {
            get => this.spacing.x;
            set => this.spacing = new Vector2(value, value);
        }

        public FillType Fill
        {
            get => this.fill;
            set => this.fill = value;
        }

        // Selection

        /// <summary>
        /// Global switch to disable selection for this grid
        /// </summary>
        public bool AllowSelection { get; set; }

        /// <summary>
        /// Determines if a entry can be deselected by clicking it again
        /// </summary>
        public bool AllowDeselection { get; set; }

        /// <summary>
        /// How many entries can be selected at a given time
        /// </summary>
        public int MaxSelectedElements { get; set; }

        /// <summary>
        /// The behaviour to apply if the maximum amount of selections is reached
        /// </summary>
        public SelectionOverflowType SelectionOverflowType { get; set; }
    }
}
