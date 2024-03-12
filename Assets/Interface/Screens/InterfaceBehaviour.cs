using System;
using System.Collections;
using System.Collections.Generic;
using Buildings.Base;
using Buildings.Towers;
using Common.Time;
using Enemies;
using Interface.Elements;
using NaughtyAttributes;
using Pickups;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InterfaceBehaviour : MonoBehaviour
{
    public static InterfaceBehaviour Instance { get; private set; }
    public static Action<string> ChangedGenericHint;

    public enum Element
    {
        GENERIC_HINT,
        PLAYER_LIFES,
        BUILDING_RESOURCES,
        AMMO_RESOURCES,
        INENVTORY_BAR,
        TOWER_CARDS,
        START_WAVE,
        TIMER,
    }

    /* --- References --- */

    [SerializeField][BoxGroup("References")] private UIDocument document;

    /* --- Values --- */

    private Label genericHintLabel;
    private Label ammoChangeLabel;
    private Label timerLabel;
    private ButtonElement startWaveButton;
    private StatElement playerLifes;
    private StatElement buildingResources;
    private StatElement ammoResources;
    private LinearProgressBarElement inventoryBar;
    private VisualElement cardParent;
    private List<TowerCardElement> cards = new();

    /* --- Values --- */

    private Coroutine ammoChangeRoutine;
    private Dictionary<Element, VisualElement> interfaceElements = new();

    private void Awake()
    {
        Instance = this;

        // References
        VisualElement root = this.document.rootVisualElement;
        this.genericHintLabel = (Label)root.Query("generic-hint-label");
        this.ammoChangeLabel = (Label)root.Query("ammo-change-label");
        this.playerLifes = (StatElement)root.Query("player-lifes");
        this.buildingResources = (StatElement)root.Query("building-resources");
        this.ammoResources = (StatElement)root.Query("ammo-resources");
        this.inventoryBar = (LinearProgressBarElement)root.Query("inventory-progress");
        this.cardParent = root.Query("tower-card-parent");
        this.startWaveButton = (ButtonElement)root.Query("start-wave-button");
        this.timerLabel = (Label)root.Query("timer-label");

        // Callbacks
        TowerPlacemementSystem.FinishedPlacement += (succes) => this.ReleasedTowerCard();
        BaseSystem.BuildingResourcesUpdated += this.UpdateBuildingResources;
        BaseSystem.AmmoResourcesUpdated += this.UpdateAmmoResources;
        BaseSystem.PlayerLifesUpdated += this.UpdatePlayerLifes;
        PickupSystem.PickedUpLoot += this.UpdateInventory;
        RemainingTimeSystem.RemainingTimeChanged += this.UpdateTimer;
        EnemySystem.EnemyReachedBase += () => this.ShowGameOver(GameoverBehaviour.GameoverType.REACHED_BASE);
        ChangedGenericHint += this.SetGenericHint;

        this.startWaveButton.Click += Helpers.StartEnemyWaveSpawner;

        // Initial values
        this.UpdateAmmoResources(0, BaseSystem.currentData.AmmoResoruces);
        this.UpdateBuildingResources(0, BaseSystem.currentData.BuildingResoruces);
        this.ammoChangeLabel.SetText(string.Empty);

        // Tower cards
        this.SetupTowerCards();

        // Default visibility
        this.interfaceElements.Add(Element.GENERIC_HINT, this.genericHintLabel);
        this.interfaceElements.Add(Element.PLAYER_LIFES, this.playerLifes);
        this.interfaceElements.Add(Element.BUILDING_RESOURCES, this.buildingResources);
        this.interfaceElements.Add(Element.AMMO_RESOURCES, this.ammoResources);
        this.interfaceElements.Add(Element.INENVTORY_BAR, this.inventoryBar.parent);
        this.interfaceElements.Add(Element.TOWER_CARDS, this.cardParent);
        this.interfaceElements.Add(Element.START_WAVE, this.startWaveButton);
        this.interfaceElements.Add(Element.TIMER, this.timerLabel.parent);
        this.HideAllElements();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TowerPlacemementSystem.FinishedPlacement -= (succes) => this.ReleasedTowerCard();
            BaseSystem.BuildingResourcesUpdated -= this.UpdateBuildingResources;
            BaseSystem.AmmoResourcesUpdated -= this.UpdateAmmoResources;
            BaseSystem.PlayerLifesUpdated -= this.UpdatePlayerLifes;
            PickupSystem.PickedUpLoot -= this.UpdateInventory;
            RemainingTimeSystem.RemainingTimeChanged -= this.UpdateTimer;
            EnemySystem.EnemyReachedBase -= () => this.ShowGameOver(GameoverBehaviour.GameoverType.REACHED_BASE);
            ChangedGenericHint -= this.SetGenericHint;

            var entityManager = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager;
            entityManager.DestroyEntity(entityManager.UniversalQuery);

            World.DefaultGameObjectInjectionWorld.Dispose();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            DefaultWorldInitialization.Initialize("Default World", false);
        }
    }

    private void UpdateTimer(float remainingSec)
    {
        if (this == null)
        {
            return;
        }

        int minutes = (int)(remainingSec / 60);
        int seconds = ((int)remainingSec) - minutes * 60;
        int milliseconds = (int)(remainingSec % 1 * 1000);
        this.timerLabel.SetText(minutes.ToString("00") + ":" + seconds.ToString("00") + "." + milliseconds.ToString("000"));

        if (remainingSec <= 0)
        {
            this.ShowGameOver(GameoverBehaviour.GameoverType.VICTORY);
        }
    }

    private void UpdatePlayerLifes(int old, int value)
    {
        if (this == null)
        {
            return;
        }

        this.UpdateInventory(0, 10);
        this.playerLifes.Value = value.ToString();
        if (value <= 0)
        {
            this.ShowGameOver(GameoverBehaviour.GameoverType.OUT_OF_LIFES);
        }
    }

    private void ShowGameOver(GameoverBehaviour.GameoverType type)
    {
        this.HideAllElements();
        GameoverBehaviour.Instance.ShowGameOver(type);
    }

    private void UpdateBuildingResources(int old, int value)
    {
        if (this == null)
        {
            return;
        }

        this.buildingResources.Value = value.ToString();
    }

    private void UpdateAmmoResources(int old, int value)
    {
        if (this == null)
        {
            return;
        }

        if (value > old)
        {
            if (this.ammoChangeRoutine != null)
            {
                this.StopCoroutine(this.ammoChangeRoutine);
            }

            this.ammoChangeRoutine = this.StartCoroutine(this.ShowAmmoChangeCorutine(value - old));
        }

        this.ammoResources.Value = value.ToString();
    }

    private IEnumerator ShowAmmoChangeCorutine(int added)
    {
        this.ammoChangeLabel.SetText("+" + added);
        yield return new WaitForSeconds(3f);
        this.ammoChangeLabel.SetText(string.Empty);
    }

    private void UpdateInventory(int value, int max)
    {
        if (this == null)
        {
            return;
        }

        this.inventoryBar.Max = max;
        this.inventoryBar.SetValue(value, 0.1f, DG.Tweening.Ease.InOutCubic);

        bool inventoryFull = value >= max;
        if (inventoryFull)
        {
            this.SetElementVisible(Element.GENERIC_HINT, true);
            ChangedGenericHint.Invoke("Drop off ammo at base");
        }
        else if (value == 0)
        {
            this.SetElementVisible(Element.GENERIC_HINT, true);
            ChangedGenericHint.Invoke(string.Empty);
        }

        this.inventoryBar.ShowText = inventoryFull;
    }

    private void SetGenericHint(string value)
    {
        this.genericHintLabel.SetText(value);
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
            card.RegisterCallback<PointerUpEvent>((pointerEvent) => this.ReleasedTowerCard());
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
                TowerType = card.TowerType,
            });

            ChangedGenericHint.Invoke("Press ESCAPE to cancel placement");
        }
    }

    private void ReleasedTowerCard()
    {
        foreach (TowerCardElement card in this.cards)
        {
            card.SetPseudoState(CustomPseudoStates.States.CARD_CLICK, false);
        }

        ChangedGenericHint.Invoke("Drag & drop to place towers");
    }

    public void HideAllElements()
    {
        foreach (Element type in System.Enum.GetValues(typeof(Element)))
        {
            if (this.interfaceElements.TryGetValue(type, out VisualElement element))
            {
                element.SetDisplayStyle(DisplayStyle.None);
            }
        }
    }

    public void SetElementVisible(Element type, bool visible)
    {
        if (this.interfaceElements.TryGetValue(type, out VisualElement element))
        {
            element.SetDisplayStyle(visible ? DisplayStyle.Flex : DisplayStyle.None);
        }
    }
}