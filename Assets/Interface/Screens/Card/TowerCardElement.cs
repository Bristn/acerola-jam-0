using System;
using Buildings.Towers;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class TowerCardElement : CustomElement
    {
        public new class UxmlFactory : UxmlFactory<TowerCardElement, UxmlTraits> { }

        /* --- References --- */

        private Label nameLabel;
        private Label costLabel;
        private Label descriptionLabel;

        /* --- Values --- */

        public event Action Click;

        /* --- Properties --- */

        /* --- Methods --- */

        public TowerCardElement()
        {
            this.BuildVisuals(CustomElementLookup.GetInstance().TowerCardElement);
        }

        public override void UpdateReferences()
        {
            this.nameLabel = (Label)this.Query("tower-name-label");
            this.costLabel = (Label)this.Query("tower-cost-label");
            this.descriptionLabel = (Label)this.Query("tower-description-label");

            this.RegisterCallback<PointerEnterEvent>((pointerEvent) =>
            {
                this.SetPseudoState(CustomPseudoStates.States.CARD_HOVER, true);
            });

            this.RegisterCallback<PointerLeaveEvent>((pointerEvent) =>
            {
                this.SetPseudoState(CustomPseudoStates.States.CARD_HOVER, false);
            });

            this.RegisterCallback<PointerDownEvent>((pointerEvent) =>
            {
                this.SetPseudoState(CustomPseudoStates.States.CARD_CLICK, true);

                EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                EntityQuery placementData = entityManager.CreateEntityQuery(new ComponentType[] { typeof(TowerPlacemementData) });

                bool hasData = placementData.TryGetSingletonEntity<TowerPlacemementData>(out Entity colorTablesEntity);
                if (!hasData)
                {
                    Debug.Log("No PlacementData found!");
                    return;
                }

                entityManager.SetComponentData(colorTablesEntity, new TowerPlacemementData()
                {
                    ShowPlacement = true,
                    TowerType = this.TowerType
                });
            });

            this.RegisterCallback<PointerUpEvent>((pointerEvent) =>
            {
                this.SetPseudoState(CustomPseudoStates.States.CARD_CLICK, false);
            });
        }
    }
}