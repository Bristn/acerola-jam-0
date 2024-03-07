using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class TowerCardElement : CustomElement
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription towerName = new() { name = "tower-name", defaultValue = "Label" };
            private readonly UxmlStringAttributeDescription towerDescription = new() { name = "tower-description", defaultValue = "Label" };
            private readonly UxmlIntAttributeDescription towerCost = new() { name = "tower-cost", defaultValue = 100 };
            private readonly UxmlIntAttributeDescription towerType = new() { name = "tower-type", defaultValue = 0 };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement element, IUxmlAttributes attributes, CreationContext context)
            {
                base.Init(element, attributes, context);

                TowerCardElement customElement = element as TowerCardElement;
                customElement.BuildVisuals(CustomElementLookup.GetInstance().TowerCardElement);
                customElement.TowerName = towerName.GetValueFromBag(attributes, context);
                customElement.TowerDescription = towerDescription.GetValueFromBag(attributes, context);
                customElement.TowerCost = towerCost.GetValueFromBag(attributes, context);
                customElement.TowerType = towerType.GetValueFromBag(attributes, context);
            }
        }
    }
}