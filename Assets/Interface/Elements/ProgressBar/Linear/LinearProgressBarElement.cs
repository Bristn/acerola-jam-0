using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class LinearProgressBarElement : CustomElement
    {
        public new class UxmlFactory : UxmlFactory<LinearProgressBarElement, UxmlTraits> { }

        private static readonly List<StylePropertyName> widthProperty = new List<StylePropertyName>() { new("width") };
        private static readonly List<StylePropertyName> heightroperty = new List<StylePropertyName>() { new("height") };

        public enum FillDirection
        {
            HORIZONTAL,
            VERTICAL,
        }

        /* --- References --- */

        private Label label;
        private VisualElement activeBackground;

        /* --- Vlaues --- */

        private TweenerCore<float, float, FloatOptions> tween = null;

        /* --- Methods --- */

        public LinearProgressBarElement()
        {
            this.BuildVisuals(CustomElementLookup.GetInstance().LinearProgressBarElement);
        }

        public override void UpdateReferences()
        {
            this.label = (Label)this.Query("progress-label");
            this.activeBackground = (VisualElement)this.Query("background-active");
        }

        private float GetCurrentValue()
        {
            return this.value;
        }

        private void SetCurrentValue(float value)
        {
            this.value = value;

            // Set the text value
            if (showText)
            {
                float rounded = Mathf.Clamp(this.value, this.min, this.max);
                string text = this.textFormat.Replace("{i}", rounded.ToString("n" + this.textDigits));
                this.label?.SetText(text);
            }

            // Sets the background size
            float relativeValue = this.value.Remap(this.min, this.max, 0, 100);
            relativeValue = Mathf.Clamp(relativeValue, 0, 100);
            switch (this.type)
            {
                case ProgressType.HORIZONTAL_LEFT_RIGHT:
                case ProgressType.HORIZONTAL_RIGHT_LEFT:
                    this.activeBackground.style.width = Length.Percent(relativeValue);
                    break;

                case ProgressType.VERTICAL_UP_DOWN:
                case ProgressType.VERTICAL_DOWN_UP:
                    this.activeBackground.style.height = Length.Percent(relativeValue);
                    break;
            }
        }

        /// <summary>
        /// Sets the progress of the progressbar over the given duration using the ease type
        /// </summary>
        /// <param name="value">The progress value</param>
        /// <param name="time">The duration of the animation</param>
        /// <param name="ease">The easing type</param>
        public void SetValue(float value, float time = 0, Ease ease = Ease.Linear)
        {
            this.tween?.Kill();
            if (time == 0)
            {
                this.SetCurrentValue(value);
            }
            else
            {
                this.tween = DOTween.To(this.GetCurrentValue, this.SetCurrentValue, value, time).SetEase(ease);
            }
        }
    }
}