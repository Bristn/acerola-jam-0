using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class LinearElement : VisualElement
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlEnumAttributeDescription<RoundChildren> roundChildren = new() { name = "round-children", defaultValue = RoundChildren.ALL };
            private readonly UxmlFloatAttributeDescription spacing = new() { name = "spacing", defaultValue = 0 };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement element, IUxmlAttributes attributes, CreationContext context)
            {
                base.Init(element, attributes, context);

                LinearElement customElement = element as LinearElement;
                customElement.RoundChildren = this.roundChildren.GetValueFromBag(attributes, context);
                customElement.Spacing = this.spacing.GetValueFromBag(attributes, context);
                customElement.UpdateCallbacks();
            }
        }
    }
}