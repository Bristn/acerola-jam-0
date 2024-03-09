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

    private StatElement playerLifes;
    private StatElement buildingResources;
    private StatElement ammoResources;
    private LinearProgressBarElement inventoryBar;
    private List<TowerCardElement> cards = new();

    private void Awake()
    {
        // References
        VisualElement root = this.document.rootVisualElement;
        this.playerLifes = (StatElement)root.Query("player-lifes");
        this.buildingResources = (StatElement)root.Query("building-resources");
        this.ammoResources = (StatElement)root.Query("ammo-resources");
        this.inventoryBar = (LinearProgressBarElement)root.Query("inventory-progress");

        // Callbacks
        TowerPlacemementSystem.FinishedPlacement += this.ResetCardClick;
        BaseSystem.BuildingResourcesUpdated += this.UpdateBuildingResources;
        BaseSystem.AmmoResourcesUpdated += this.UpdateAmmoResources;
        PickupSystem.PickedUpLoot += this.UpdateInventory;

        // Initial values
        this.UpdateAmmoResources(BaseSystem.currentData.AmmoResoruces);
        this.UpdateBuildingResources(BaseSystem.currentData.BuildingResoruces);

        // Tower cards
        this.SetupTowerCards();
    }

    private void UpdatePlayerLifes(int value)
    {
        this.playerLifes.Value = value.ToString();
    }

    private void UpdateBuildingResources(int value)
    {
        this.buildingResources.Value = value.ToString();
    }

    private void UpdateAmmoResources(int value)
    {
        this.ammoResources.Value = value.ToString();
    }

    private void UpdateInventory(int value, int max)
    {
        this.inventoryBar.Max = max;
        this.inventoryBar.SetValue(value, 0.1f, DG.Tweening.Ease.InOutCubic);
        this.inventoryBar.ShowText = value >= max;
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