using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public class IconElement : CustomElement
    {
        public new class UxmlFactory : UxmlFactory<IconElement, UxmlTraits> { }

        /* --- References --- */

        private Label iconLabel;

        /* --- Values --- */

        private string icon;
        private TextColor.Color color;

        /* --- Properties --- */

        /// <summary>
        /// Set the display icon. Setting the suffix of the icon changes the font asset used:
        /// _S: Solid icons
        /// _R: Regular icons
        /// _B: Brand icons
        /// </summary>
        public string Icon
        {
            get => this.icon;
            set
            {
                this.icon = value;
                if (this.icon == null || this.icon.Length == 0)
                {
                    this.iconLabel?.SetText("");
                    return;
                }

                FontStyle.Style font = FontStyle.Style.ICON_SOLID;
                if (this.icon.EndsWith("_R"))
                {
                    font = FontStyle.Style.ICON_REGULAR;
                }
                else if (this.icon.EndsWith("_B"))
                {
                    font = FontStyle.Style.ICON_BRANDS;
                }

                int index = this.icon.IndexOf("_");
                string iconText = this.icon.Substring(0, index == -1 ? this.icon.Length : index);
                this.iconLabel?.SetText("\\u" + iconText);
                this.iconLabel?.SetCustomStyle(FontStyle.Instance, font);
            }
        }

        public TextColor.Color Color
        {
            get => this.color;
            set
            {
                this.color = value;
                this.iconLabel?.SetCustomStyle(TextColor.Instance, this.color);
            }
        }

        /* --- Methods --- */

        public IconElement()
        {
            this.BuildVisuals(CustomElementLookup.GetInstance().IconElement);
        }

        public override void UpdateReferences()
        {
            this.iconLabel = (Label)this.Query("icon");
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription icon = new() { name = "icon", defaultValue = "" };
            private readonly UxmlEnumAttributeDescription<TextColor.Color> color = new() { name = "color", defaultValue = TextColor.Color.ON_BACKGROUND };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement element, IUxmlAttributes attributes, CreationContext context)
            {
                base.Init(element, attributes, context);

                IconElement customElement = element as IconElement;
                customElement.BuildVisuals(CustomElementLookup.GetInstance().IconElement);
                customElement.Icon = icon.GetValueFromBag(attributes, context);
                customElement.Color = color.GetValueFromBag(attributes, context);
            }
        }
    }
}
