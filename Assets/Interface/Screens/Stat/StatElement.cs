using System;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class StatElement : CustomElement, IRoundedCornersTarget
    {
        public new class UxmlFactory : UxmlFactory<StatElement, UxmlTraits> { }

        /* --- References --- */

        private VisualElement background;
        private Label valueLabel;
        private IconElement statIcon;

        /* --- Values --- */

        /* --- Properties --- */

        public override VisualElement RoundedCornersTarget => this.background;

        /* --- Methods --- */

        public StatElement()
        {
            this.BuildVisuals(CustomElementLookup.GetInstance().StatElement);
        }

        public override void UpdateReferences()
        {
            this.background = this.Query("background");
            this.valueLabel = (Label)this.Query("value-label");
            this.statIcon = (IconElement)this.Query("stat-icon");
        }
    }
}