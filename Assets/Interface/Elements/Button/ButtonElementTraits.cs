using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class ButtonElement : CustomElement
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlEnumAttributeDescription<BackgroundColor.Color> color = new() { name = "color", defaultValue = BackgroundColor.Color.PRIMARY };
            private readonly UxmlEnumAttributeDescription<TextAnchor> align = new() { name = "align", defaultValue = TextAnchor.MiddleLeft };
            private readonly UxmlStringAttributeDescription text = new() { name = "text", defaultValue = "Label" };
            private readonly UxmlStringAttributeDescription iconStart = new() { name = "icon-start", defaultValue = "" };
            private readonly UxmlStringAttributeDescription iconCenter = new() { name = "icon-center", defaultValue = "" };
            private readonly UxmlStringAttributeDescription iconEnd = new() { name = "icon-end", defaultValue = "" };
            private readonly UxmlEnumAttributeDescription<RoundedStyle.Style> rounded = new() { name = "rounded", defaultValue = RoundedStyle.Style.NORMAL };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement element, IUxmlAttributes attributes, CreationContext context)
            {
                base.Init(element, attributes, context);

                ButtonElement customElement = element as ButtonElement;
                customElement.BuildVisuals(CustomElementLookup.GetInstance().ButtonElement);
                customElement.Text = text.GetValueFromBag(attributes, context);
                customElement.Color = color.GetValueFromBag(attributes, context);
                customElement.Rounded = rounded.GetValueFromBag(attributes, context);
                customElement.Align = align.GetValueFromBag(attributes, context);
                customElement.IconStart = iconStart.GetValueFromBag(attributes, context);
                customElement.IconCenter = iconCenter.GetValueFromBag(attributes, context);
                customElement.IconEnd = iconEnd.GetValueFromBag(attributes, context);
            }
        }
    }
}