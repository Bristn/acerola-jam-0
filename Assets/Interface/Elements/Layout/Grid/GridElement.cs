using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class GridElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<GridElement, UxmlTraits> { }

        public event Action AfterLayoutCallback;

        /* --- Methods --- */

        public GridElement()
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

        /// <summary>
        /// Detemines the automatic values for rows, columns & cell size.
        /// Adjusts the position of all children to the constraints
        /// https://www.youtube.com/watch?v=CGsEJToeXmA&t=631s
        /// </summary>
        public void UpdateLayout()
        {
            if (this.fit == FitType.UNIFORM || this.fit == FitType.HEIGHT || this.fit == FitType.WIDTH)
            {
                this.fill = FillType.BOTH;

                float squareRoot = Mathf.Sqrt(this.hierarchy.childCount);
                this.columns = Mathf.CeilToInt(squareRoot);
                this.rows = Mathf.CeilToInt(squareRoot);
            }

            if (this.fit == FitType.WIDTH || this.fit == FitType.FIXED_COLUMNS)
            {
                this.rows = Mathf.CeilToInt(this.hierarchy.childCount / (float)this.columns);
            }
            else if (this.fit == FitType.HEIGHT || this.fit == FitType.FIXED_ROWS)
            {
                this.columns = Mathf.CeilToInt(this.hierarchy.childCount / (float)this.rows);
            }

            float containerWidth = this.resolvedStyle.width;
            float containerHeight = this.resolvedStyle.height;

            float cellWidth = (containerWidth / this.columns) - (spacing.x / this.columns * (this.columns - 1));
            float cellHeight = (containerHeight / this.rows) - (spacing.y / this.rows * (this.rows - 1));

            this.cellSize.x = this.fill == FillType.BOTH || this.fill == FillType.HORIZONTAL ? cellWidth : this.cellSize.x;
            this.cellSize.y = this.fill == FillType.BOTH || this.fill == FillType.VERTICAL ? cellHeight : this.cellSize.y;

            // Layout the children
            int columnIndex = 0;
            int rowIndex = 0;
            for (int i = 0; i < this.hierarchy.childCount; i++)
            {
                rowIndex = i / this.columns;
                columnIndex = i % this.columns;

                float xPosition = (columnIndex * this.cellSize.x) + (spacing.x * columnIndex);
                float yPosition = (rowIndex * this.cellSize.y) + (spacing.y * rowIndex);

                VisualElement child = this.hierarchy.Children().ElementAt(i);
                child.style.position = new StyleEnum<Position>(Position.Absolute);
                child.style.left = xPosition;
                child.style.width = this.cellSize.x;
                child.style.top = yPosition;
                child.style.height = this.cellSize.y;

                // If possible, update the roudned corners of the CustomElement child
                bool isFirst = i == 0;
                bool isLast = i == this.hierarchy.childCount - 1;
                bool ignoreFirst = this.RoundChildren == RoundChildren.IGNORE_FIRST || this.RoundChildren == RoundChildren.IGNORE_ENDS;
                bool ignoreLast = this.RoundChildren == RoundChildren.IGNORE_LAST || this.RoundChildren == RoundChildren.IGNORE_ENDS;

                bool ignoreChild = this.RoundChildren == RoundChildren.NONE || (ignoreFirst && isFirst) || (ignoreLast && isLast);
                if (!ignoreChild && child is CustomElement element)
                {
                    GridElementRoundCorners.UpdateCorners(element, i, this.hierarchy.childCount, this.rows, this.columns);
                }
            }

            this.AfterLayoutCallback?.Invoke();
        }
    }
}
