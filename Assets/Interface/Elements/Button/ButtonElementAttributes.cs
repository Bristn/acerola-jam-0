using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class ButtonElement : CustomElement
    {
        public class Data
        {
            public LabelType DisplayMode { get; set; }
            public string Text { get; set; }
            public string IconStart { get; set; }
            public string IconCenter { get; set; }
            public string IconEnd { get; set; }
            public BackgroundColor.Color Color { get; set; }
            public RoundedStyle.Style Rounded { get; set; }
            public TextAnchor Align { get; set; }
            public bool Interactable { get; set; } = true;
        }

        /* --- values --- */

        private string text;
        private string iconStart;
        private string iconCenter;
        private string iconEnd;
        private BackgroundColor.Color type;
        private RoundedStyle.Style rounded;
        private TextAnchor align;
        private bool interactable = true;

        /* --- Properties --- */

        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                this.label?.SetText(value);
            }
        }

        public string IconStart
        {
            get => this.iconStart;
            set
            {
                this.iconStart = value;
                this.iconElementStart.Icon = value;
                this.UpdateLayout(this.iconStart, this.iconCenter, this.iconEnd);
            }
        }

        public string IconCenter
        {
            get => this.iconCenter;
            set
            {
                this.iconCenter = value;
                this.iconElementCenter.Icon = value;
                this.UpdateLayout(this.iconStart, this.iconCenter, this.iconEnd);
            }
        }

        public string IconEnd
        {
            get => this.iconEnd;
            set
            {
                this.iconEnd = value;
                this.iconElementEnd.Icon = value;
                this.UpdateLayout(this.iconStart, this.iconCenter, this.iconEnd);
            }
        }

        public BackgroundColor.Color Color
        {
            get => this.type;
            set
            {
                this.type = value;
                this.button?.SetCustomStyle(BackgroundColor.Instance, value);
                this.label?.SetTextColorOnBackground(value);

                TextColor.OnBackgroundClasses.TryGetValue(value, out TextColor.Color textColor);
                this.iconElementStart.Color = textColor;
                this.iconElementCenter.Color = textColor;
                this.iconElementEnd.Color = textColor;
            }
        }

        public RoundedStyle.Style Rounded
        {
            get => this.rounded;
            set
            {
                this.rounded = value;
                this.button?.SetCustomStyle(RoundedStyle.Instance, this.rounded);
            }
        }

        public TextAnchor Align
        {
            get => this.align;
            set
            {
                this.align = value;
                if (this.label != null)
                {
                    this.label.style.unityTextAlign = value;
                }
            }
        }

        public bool Interactable
        {
            get => this.interactable;
            set
            {
                this.interactable = value;
                this.style.opacity = value ? 1 : 0.25f;

                if (!this.interactable)
                {
                    this.button.SetPseudoState(CustomPseudoStates.States.BUTTON_HOVER, false);
                }
            }
        }

        private void UpdateLayout(string startIcon, string centerIcon, string endIcon)
        {
            bool showStart = startIcon != null && startIcon.Length != 0;
            this.iconElementStart?.SetDisplayStyle(showStart ? DisplayStyle.Flex : DisplayStyle.None);
            this.button.style.paddingLeft = showStart ? 60 : 30;

            bool showCenter = centerIcon != null && centerIcon.Length != 0;
            this.iconElementCenter?.SetDisplayStyle(showCenter ? DisplayStyle.Flex : DisplayStyle.None);
            this.label.SetDisplayStyle(showCenter ? DisplayStyle.None : DisplayStyle.Flex);

            bool showEnd = endIcon != null && endIcon.Length != 0;
            this.iconElementEnd?.SetDisplayStyle(showEnd ? DisplayStyle.Flex : DisplayStyle.None);
            this.button.style.paddingRight = showEnd ? 60 : 30;
        }
    }
}