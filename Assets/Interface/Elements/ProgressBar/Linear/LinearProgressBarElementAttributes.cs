using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class LinearProgressBarElement : CustomElement
    {
        public enum ProgressType
        {
            [InspectorName("Horizontal")] HORIZONTAL_LEFT_RIGHT,
            [InspectorName("Horizontal - Reversed")] HORIZONTAL_RIGHT_LEFT,
            [InspectorName("Vertical")] VERTICAL_UP_DOWN,
            [InspectorName("Vertical - Reversed")] VERTICAL_DOWN_UP,
        }

        /* --- Values --- */

        private BackgroundColor.Color color;
        private RoundedStyle.Style rounded;
        private bool showText;
        private string textFormat;
        private int textDigits;
        private ProgressType type;
        private float min;
        private float max;
        private float value;

        /* --- Properties --- */

        public BackgroundColor.Color Color
        {
            get => this.color;
            set
            {
                this.color = value;
                this.activeBackground?.SetCustomStyle(BackgroundColor.Instance, value);
            }
        }

        public RoundedStyle.Style Rounded
        {
            get => this.rounded;
            set
            {
                this.rounded = value;
                this.activeBackground?.SetCustomStyle(RoundedStyle.Instance, value);
            }
        }

        public bool ShowText
        {
            get => this.showText;
            set
            {
                this.showText = value;
                this.label?.SetDisplayStyle(this.showText ? DisplayStyle.Flex : DisplayStyle.None);
            }
        }

        public string TextFormat
        {
            get => this.textFormat;
            set
            {
                this.textFormat = value;
                this.SetCurrentValue(this.Value);
            }
        }

        public int TextDigits
        {
            get => this.textDigits;
            set
            {
                this.textDigits = value;
                this.SetCurrentValue(this.Value);
            }
        }

        public ProgressType Type
        {
            get => this.type;
            set
            {
                this.type = value;

                switch (this.type)
                {
                    case ProgressType.HORIZONTAL_LEFT_RIGHT:
                        this.activeBackground.style.left = 0;
                        this.activeBackground.style.right = StyleKeyword.Null;
                        this.activeBackground.style.height = Length.Percent(100);
                        break;

                    case ProgressType.HORIZONTAL_RIGHT_LEFT:
                        this.activeBackground.style.left = StyleKeyword.Null;
                        this.activeBackground.style.right = 0;
                        this.activeBackground.style.height = Length.Percent(100);
                        break;

                    case ProgressType.VERTICAL_UP_DOWN:
                        this.activeBackground.style.top = 0;
                        this.activeBackground.style.bottom = StyleKeyword.Null;
                        this.activeBackground.style.width = Length.Percent(100);
                        break;

                    case ProgressType.VERTICAL_DOWN_UP:
                        this.activeBackground.style.top = StyleKeyword.Null;
                        this.activeBackground.style.bottom = 0;
                        this.activeBackground.style.width = Length.Percent(100);
                        break;
                }
            }
        }

        public float Min
        {
            get => this.min;
            set
            {
                this.min = value;
                this.SetCurrentValue(this.value);
            }
        }

        public float Max
        {
            get => this.max;
            set
            {
                this.max = value;
                this.SetCurrentValue(this.value);
            }
        }

        public float Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.SetCurrentValue(this.value);
            }
        }
    }
}