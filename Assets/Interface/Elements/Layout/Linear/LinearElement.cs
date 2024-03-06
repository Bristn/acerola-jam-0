using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    /// <summary>
    /// Custom element to realize linear layouts (vertical or horizontal). Does not size the children,
    /// but allows to set spacing (margin) & manage the rounded corners of chidlren
    /// </summary>
    public partial class LinearElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<LinearElement, UxmlTraits> { }

        public event Action AfterLayoutCallback;

        /* --- Methods --- */

        public LinearElement()
        {
            this.UpdateCallbacks();
        }

        public new void Add(VisualElement child)
        {
            base.Add(child);
            this.UpdateLayoutIfPossible();
        }

        public new void Remove(VisualElement child)
        {
            base.Remove(child);
            this.UpdateLayoutIfPossible();
        }

        /// <summary>
        /// Reinitializes the required event callbacks. 
        /// The layout can only be applied, if the children have been attached and the container geometry is defined
        /// </summary>
        public void UpdateCallbacks()
        {
            this.hasGeometryEvent = false;
            this.hasAttachEvent = false;

            this.UnregisterCallback<AttachToPanelEvent>(this.OnAttachEvent);
            this.UnregisterCallback<GeometryChangedEvent>(this.OnGeometryEvent);

            this.RegisterCallback<AttachToPanelEvent>(this.OnAttachEvent);
            this.RegisterCallback<GeometryChangedEvent>(this.OnGeometryEvent);
        }

        private void OnAttachEvent(AttachToPanelEvent attachEvent)
        {
            this.hasAttachEvent = true;
            this.UpdateLayoutIfPossible();
        }

        private void OnGeometryEvent(GeometryChangedEvent geometryEvent)
        {
            this.hasGeometryEvent = true;
            this.UpdateLayoutIfPossible();
        }

        private void UpdateLayoutIfPossible()
        {
            if (this.hasAttachEvent && this.hasGeometryEvent)
            {
                this.UpdateLayout();
            }
        }

        public void UpdateLayout()
        {
            bool isVertical = this.resolvedStyle.flexDirection == FlexDirection.Column || this.resolvedStyle.flexDirection == FlexDirection.ColumnReverse;
            bool isHorizontal = this.resolvedStyle.flexDirection == FlexDirection.Row || this.resolvedStyle.flexDirection == FlexDirection.RowReverse;
            bool isReversed = this.resolvedStyle.flexDirection == FlexDirection.ColumnReverse || this.resolvedStyle.flexDirection == FlexDirection.RowReverse;
            Vector4 margin = isHorizontal ? new Vector4(0, this.spacing, 0, 0) : new Vector4(0, 0, this.spacing, 0);

            int rows = (isHorizontal ? 1 : 0) * (isReversed ? -1 : 1);
            int columns = (isVertical ? 1 : 0) * (isReversed ? -1 : 1);

            // Layout the children
            for (int i = 0; i < this.hierarchy.childCount; i++)
            {
                VisualElement child = this.hierarchy.Children().ElementAt(i);

                // If possible, update the roudned corners of the CustomElement child
                bool isFirst = i == 0;
                bool isLast = i == this.hierarchy.childCount - 1;
                bool ignoreFirst = this.RoundChildren == RoundChildren.IGNORE_FIRST || this.RoundChildren == RoundChildren.IGNORE_ENDS;
                bool ignoreLast = this.RoundChildren == RoundChildren.IGNORE_LAST || this.RoundChildren == RoundChildren.IGNORE_ENDS;

                bool ignoreChild = this.RoundChildren == RoundChildren.NONE || (ignoreFirst && isFirst) || (ignoreLast && isLast);
                if (!ignoreChild)
                {
                    if (child is CustomElement element)
                    {
                        GridElementRoundCorners.UpdateCorners(element, i, this.hierarchy.childCount, rows, columns);
                    }
                    else
                    {
                        GridElementRoundCorners.UpdateCorners(child, i, this.hierarchy.childCount, rows, columns);
                    }
                }

                if (!isLast && !isFirst)
                {
                    child.SetMargin(margin);
                }
                else if (isFirst && !isReversed)
                {
                    child.SetMargin(margin);
                }
                else if (isLast && isReversed)
                {
                    child.SetMargin(margin);
                }
            }

            this.AfterLayoutCallback?.Invoke();
        }
    }
}
