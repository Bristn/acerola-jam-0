using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class GridElement : VisualElement
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlEnumAttributeDescription<RoundChildren> roundChildren = new() { name = "round-children", defaultValue = RoundChildren.ALL };
            private readonly UxmlEnumAttributeDescription<FitType> fit = new() { name = "fit", defaultValue = FitType.WIDTH };
            private readonly UxmlEnumAttributeDescription<FillType> fill = new() { name = "fill", defaultValue = FillType.NONE };
            private readonly UxmlIntAttributeDescription rows = new() { name = "rows", defaultValue = 1 };
            private readonly UxmlIntAttributeDescription columns = new() { name = "columns", defaultValue = 1 };
            private readonly UxmlFloatAttributeDescription cellWidth = new() { name = "cell-width", defaultValue = 100 };
            private readonly UxmlFloatAttributeDescription cellHeight = new() { name = "cell-height", defaultValue = 100 };
            private readonly UxmlFloatAttributeDescription spacing = new() { name = "spacing", defaultValue = 100 };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement element, IUxmlAttributes attributes, CreationContext context)
            {
                base.Init(element, attributes, context);

                GridElement customElement = element as GridElement;
                customElement.RoundChildren = this.roundChildren.GetValueFromBag(attributes, context);
                customElement.Fit = this.fit.GetValueFromBag(attributes, context);
                customElement.Rows = this.rows.GetValueFromBag(attributes, context);
                customElement.Columns = this.columns.GetValueFromBag(attributes, context);
                customElement.CellWidth = this.cellWidth.GetValueFromBag(attributes, context);
                customElement.CellHeight = this.cellHeight.GetValueFromBag(attributes, context);
                customElement.Spacing = this.spacing.GetValueFromBag(attributes, context);
                customElement.Fill = this.fill.GetValueFromBag(attributes, context);
                customElement.UpdateCallbacks();
            }
        }
    }
}