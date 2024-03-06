using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class ButtonElement : CustomElement
    {
        public new class UxmlFactory : UxmlFactory<ButtonElement, UxmlTraits> { }

        public enum LabelType
        {
            [InspectorName("Text only")] TEXT_ONLY,
            [InspectorName("Icon only")] ICON_ONLY,
            [InspectorName("Both")] BOTH,
        }


        /* --- References --- */

        private Label label;
        private IconElement iconElementStart;
        private IconElement iconElementCenter;
        private IconElement iconElementEnd;
        private ContainerElement button;

        /* --- Values --- */

        public event Action Click;

        /* --- Properties --- */

        public override VisualElement RoundedCornersTarget => this.button;

        /* --- Methods --- */

        public ButtonElement()
        {
            this.BuildVisuals(CustomElementLookup.GetInstance().ButtonElement);
        }

        public ButtonElement(Data data)
        {
            this.BuildVisuals(CustomElementLookup.GetInstance().ButtonElement);

            this.Text = data.Text;
            this.Color = data.Color;
            this.Rounded = data.Rounded;
            this.Align = data.Align;
            this.Interactable = data.Interactable;
            this.IconStart = data.IconStart;
            this.IconCenter = data.IconCenter;
            this.IconEnd = data.IconEnd;
        }

        public Data GetData()
        {
            return new()
            {
                Text = this.text,
                Color = this.Color,
                Rounded = this.Rounded,
                Align = this.Align,
                Interactable = this.interactable,
                IconStart = this.IconStart,
                IconCenter = this.IconCenter,
                IconEnd = this.iconEnd
            };
        }

        public override void UpdateReferences()
        {
            this.label = (Label)this.Query("text");
            this.iconElementStart = (IconElement)this.Query("icon-start");
            this.iconElementCenter = (IconElement)this.Query("icon-center");
            this.iconElementEnd = (IconElement)this.Query("icon-end");
            this.button = (ContainerElement)this.Query("button");

            this.RegisterCallback<ClickEvent>((clickEvent) =>
            {
                if (!this.Interactable)
                {
                    return;
                }

                this.Click?.Invoke();
            });

            this.RegisterCallback<PointerEnterEvent>((pointerEvent) =>
            {
                if (!this.Interactable)
                {
                    return;
                }

                this.button.SetPseudoState(CustomPseudoStates.States.BUTTON_HOVER, true);
            });

            this.RegisterCallback<PointerLeaveEvent>((pointerEvent) =>
            {
                if (!this.Interactable)
                {
                    return;
                }

                this.button.SetPseudoState(CustomPseudoStates.States.BUTTON_HOVER, false);
            });
        }
    }
}