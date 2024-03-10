using System;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class DialogElement : CustomElement
    {
        public new class UxmlFactory : UxmlFactory<DialogElement, UxmlTraits> { }

        /* --- References --- */

        private Label dialogLabel;
        private ButtonElement dialogButton;

        /* --- Values --- */

        public event Action ClickButton;

        /* --- Properties --- */

        /* --- Methods --- */

        public DialogElement()
        {
            this.BuildVisuals(CustomElementLookup.GetInstance().DialogElement);
        }

        public override void UpdateReferences()
        {
            this.dialogLabel = (Label)this.Query("dialog-label");
            this.dialogButton = (ButtonElement)this.Query("dialog-button");

            this.dialogButton.Click += () => this.ClickButton.Invoke();
        }
    }
}