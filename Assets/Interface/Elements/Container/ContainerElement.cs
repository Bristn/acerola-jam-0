using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class ContainerElement : CustomElement, IRoundedCornersTarget
    {
        public new class UxmlFactory : UxmlFactory<ContainerElement, UxmlTraits> { }

        public event Action AfterLayoutCallback;

        /* --- Methods --- */

        public ContainerElement() { }

        public override void UpdateReferences() { }

        public override VisualElement RoundedCornersTarget => this;
    }
}
