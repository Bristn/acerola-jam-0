using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class StatElement : CustomElement
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription value = new() { name = "value", defaultValue = "0" };
            private readonly UxmlStringAttributeDescription icon = new() { name = "icon", defaultValue = "f183" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement element, IUxmlAttributes attributes, CreationContext context)
            {
                base.Init(element, attributes, context);

                StatElement customElement = element as StatElement;
                customElement.BuildVisuals(CustomElementLookup.GetInstance().StatElement);
                customElement.Value = value.GetValueFromBag(attributes, context);
                customElement.Icon = icon.GetValueFromBag(attributes, context);
            }
        }
    }
}