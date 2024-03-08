using System.Collections.Generic;
using Buildings.Base;
using Buildings.Towers;
using Interface.Elements;
using NaughtyAttributes;
using Pickups;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class InterfaceBehaviour : MonoBehaviour
{
    /* --- References --- */

    [SerializeField][BoxGroup("References")] private UIDocument document;

    /* --- Values --- */

    private Label buildingResourcesLabel;
    private Label ammoResourcesLabel;
    private LinearProgressBarElement inventoryBar;
    private List<TowerCardElement> cards = new();

    private void Awake()
    {
        // References
        VisualElement root = this.document.rootVisualElement;
        this.buildingResourcesLabel = (Label)root.Query("building-resources-label");
        this.ammoResourcesLabel = (Label)root.Query("ammo-resources-label");
        this.inventoryBar = (LinearProgressBarElement)root.Query("inventory-progress");

        // Callbacks
        TowerPlacemementSystem.SuccessfullyPlacedTower += this.ResetCardClick;
        BaseSystem.BuildingResourcesUpdated += this.UpdateBuildingResources;
        BaseSystem.AmmoResourcesUpdated += this.UpdateAmmoResources;
        PickupSystem.PickedUpLoot += this.UpdateInventory;

        // Initial values
        this.UpdateAmmoResources(BaseSystem.currentData.AmmoResoruces);
        this.UpdateBuildingResources(BaseSystem.currentData.BuildingResoruces);

        // Tower cards
        this.SetupTowerCards();
    }

    private void UpdateBuildingResources(int value)
    {
        this.buildingResourcesLabel.SetText(value.ToString());
    }

    private void UpdateAmmoResources(int value)
    {
        this.ammoResourcesLabel.SetText(value.ToString());
    }

    private void UpdateInventory(int value, int max)
    {
        this.inventoryBar.Max = max;
        this.inventoryBar.SetValue(value, 0.1f, DG.Tweening.Ease.InOutCubic);
    }

    private void SetupTowerCards()
    {
        VisualElement root = this.document.rootVisualElement;

        // Get references
        this.cards.Add((TowerCardElement)root.Query("tower-default"));
        this.cards.Add((TowerCardElement)root.Query("tower-sniper"));
        this.cards.Add((TowerCardElement)root.Query("tower-shotgun"));
        this.cards.Add((TowerCardElement)root.Query("tower-rifle"));

        // Interactivity
        foreach (TowerCardElement card in this.cards)
        {
            card.RegisterCallback<PointerDownEvent>((pointerEvent) => this.ClickCard(card));
            card.RegisterCallback<PointerUpEvent>((pointerEvent) => this.ResetCardClick());
        }
    }

    private void ClickCard(TowerCardElement card)
    {
        card.SetPseudoState(CustomPseudoStates.States.CARD_CLICK, true);

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery placementData = entityManager.CreateEntityQuery(new ComponentType[] { typeof(TowerPlacemementData) });

        if (placementData.TryGetSingletonEntity<TowerPlacemementData>(out Entity entity))
        {
            entityManager.SetComponentData(entity, new TowerPlacemementData()
            {
                ShowPlacement = true,
                TowerType = card.TowerType
            });
        }
    }

    private void ResetCardClick()
    {
        foreach (TowerCardElement card in this.cards)
        {
            card.SetPseudoState(CustomPseudoStates.States.CARD_CLICK, false);
        }
    }
}