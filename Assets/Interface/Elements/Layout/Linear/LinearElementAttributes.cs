using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class LinearElement : VisualElement
    {
        /* --- Values --- */

        private RoundChildren roundChildren = RoundChildren.ALL;
        private float spacing = 0;
        private bool hasGeometryEvent;
        private bool hasAttachEvent;

        /* --- Properties  --- */

        public RoundChildren RoundChildren
        {
            get => this.roundChildren;
            set => this.roundChildren = value;
        }

        public float Spacing
        {
            get => this.spacing;
            set => this.spacing = value;
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
