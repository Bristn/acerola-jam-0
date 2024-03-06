using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class LinearProgressBarElement : CustomElement
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlEnumAttributeDescription<BackgroundColor.Color> color = new() { name = "color", defaultValue = BackgroundColor.Color.PRIMARY };
            private readonly UxmlEnumAttributeDescription<RoundedStyle.Style> rounded = new() { name = "rounded", defaultValue = RoundedStyle.Style.NONE };
            private readonly UxmlBoolAttributeDescription showText = new() { name = "show-text", defaultValue = true };
            private readonly UxmlStringAttributeDescription textFormat = new() { name = "text-format", defaultValue = "{i}" };
            private readonly UxmlIntAttributeDescription textDigits = new() { name = "text-digits", defaultValue = 2 };
            private readonly UxmlEnumAttributeDescription<ProgressType> type = new() { name = "type", defaultValue = ProgressType.HORIZONTAL_LEFT_RIGHT };
            private readonly UxmlFloatAttributeDescription min = new() { name = "min", defaultValue = 0 };
            private readonly UxmlFloatAttributeDescription max = new() { name = "max", defaultValue = 1 };
            private readonly UxmlFloatAttributeDescription value = new() { name = "value", defaultValue = 0.5f };

            public override void Init(VisualElement element, IUxmlAttributes attributes, CreationContext context)
            {
                base.Init(element, attributes, context);

                LinearProgressBarElement customElement = element as LinearProgressBarElement;
                customElement.BuildVisuals(CustomElementLookup.GetInstance().LinearProgressBarElement);

                customElement.Color = color.GetValueFromBag(attributes, context);
                customElement.Rounded = rounded.GetValueFromBag(attributes, context);
                customElement.ShowText = showText.GetValueFromBag(attributes, context);
                customElement.TextFormat = textFormat.GetValueFromBag(attributes, context);
                customElement.TextDigits = textDigits.GetValueFromBag(attributes, context);
                customElement.Type = type.GetValueFromBag(attributes, context);
                customElement.Min = min.GetValueFromBag(attributes, context);
                customElement.Max = max.GetValueFromBag(attributes, context);
                customElement.Value = value.GetValueFromBag(attributes, context);
            }
        }
    }
}