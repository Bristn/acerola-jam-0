using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class DialogElement : CustomElement
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription dialogText = new() { name = "dialog-text", defaultValue = "Dialog" };
            private readonly UxmlStringAttributeDescription buttonText = new() { name = "button-text", defaultValue = "Button" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement element, IUxmlAttributes attributes, CreationContext context)
            {
                base.Init(element, attributes, context);

                DialogElement customElement = element as DialogElement;
                customElement.BuildVisuals(CustomElementLookup.GetInstance().DialogElement);
                customElement.DialogText = dialogText.GetValueFromBag(attributes, context);
                customElement.ButtonText = buttonText.GetValueFromBag(attributes, context);
            }
        }
    }
}