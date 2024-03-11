using System.Threading.Tasks;
using Buildings.Towers;
using Cameras;
using Enemies;
using Interface.Elements;
using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;
using static InterfaceBehaviour;

public class DialogBehaviour : MonoBehaviour
{
    public enum DialogHint
    {
        INTRO,
        PLACED_ONE_TOWER,
        START_WAVE,
        BEAT_FIRST_WAVE,
        PLAYER_MOVEMENT,

        PLAYER_LOST_LIFE,
    }

    /* --- References --- */

    [SerializeField][BoxGroup("References")] private UIDocument document;

    /* --- Values --- */

    private DialogElement dialog;
    [SerializeField][BoxGroup("Info")][ReadOnly] private DialogHint hint;

    /* --- Properties --- */

    private bool DialogVisible { set => this.dialog?.SetDisplayStyle(value ? DisplayStyle.Flex : DisplayStyle.None); }

    private void Awake()
    {
        Helpers.SpawnNewPlayer();
        Helpers.SetGamePaused(false);

        // References
        VisualElement root = this.document.rootVisualElement;
        this.dialog = (DialogElement)root.Query("dialog-element");
        this.dialog.ClickButton += this.DialogButtonPressed;

        // Callbacks
        TowerPlacemementSystem.FinishedPlacement += this.TowerPlacementFinished;
        EnemyWaveSystem.BeatFirstWave += () => this.SetDialogHint(DialogHint.BEAT_FIRST_WAVE);
        CameraRecenterSystem.ReachedCenter += () => this.SetDialogHint(DialogHint.PLAYER_LOST_LIFE);

        // Start tutorial
        Task task = new(async () =>
        {
            await Task.Delay(1000);
            this.SetDialogHint(DialogHint.INTRO);
        });

        task.RunSynchronously();
    }

    public void SetDialogHint(DialogHint hint)
    {
        this.hint = hint;
        switch (hint)
        {
            case DialogHint.INTRO:
                this.dialog.DialogText = "TODO: Explain lifes & tower placement";
                this.dialog.ButtonText = "Continue";
                break;

            case DialogHint.PLACED_ONE_TOWER:
                this.dialog.DialogText = "TODO: Place remaining towers (Not enough time later) Button to start waves";
                this.dialog.ButtonText = "Continue";
                break;

            case DialogHint.START_WAVE: // Before starting the first wave
                this.dialog.DialogText = "TODO: ";
                this.dialog.ButtonText = "Continue";
                break;

            case DialogHint.BEAT_FIRST_WAVE: // After first wave is completed
                this.dialog.DialogText = "TODO: Ammo low, need to defend for x time";
                this.dialog.ButtonText = "Continue";
                break;

            case DialogHint.PLAYER_MOVEMENT:
                this.dialog.DialogText = "TODO: Use WASD to collect & return ammo from fallen enemies";
                this.dialog.ButtonText = "Continue";
                break;

            case DialogHint.PLAYER_LOST_LIFE:
                this.dialog.DialogText = "TODO: One less life, another person needs to take over ammo collection";
                this.dialog.ButtonText = "Continue";
                Helpers.SetGamePaused(true);
                break;
        }

        InterfaceBehaviour.Instance.HideAllElements();
        this.DialogVisible = true;
    }

    private void DialogButtonPressed()
    {
        switch (this.hint)
        {
            case DialogHint.INTRO:

                InterfaceBehaviour.Instance.SetElementVisible(Element.TOWER_CARDS, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.BUILDING_RESOURCES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.PLAYER_LIFES, true);
                this.DialogVisible = false;
                break;

            case DialogHint.PLACED_ONE_TOWER:
                InterfaceBehaviour.Instance.SetElementVisible(Element.TOWER_CARDS, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.BUILDING_RESOURCES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.AMMO_RESOURCES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.PLAYER_LIFES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.START_WAVE, true);
                this.DialogVisible = false;
                break;

            case DialogHint.BEAT_FIRST_WAVE:
                this.SetDialogHint(DialogHint.PLAYER_MOVEMENT);
                break;

            case DialogHint.PLAYER_MOVEMENT:
                InterfaceBehaviour.Instance.SetElementVisible(Element.PLAYER_LIFES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.AMMO_RESOURCES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.TIMER, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.INENVTORY_BAR, true);
                Helpers.StartEnemySpawner();
                Helpers.EnablePlayerMovement();
                Helpers.StartTimer();
                this.DialogVisible = false;
                break;

            case DialogHint.PLAYER_LOST_LIFE:
                InterfaceBehaviour.Instance.SetElementVisible(Element.PLAYER_LIFES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.AMMO_RESOURCES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.TIMER, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.INENVTORY_BAR, true);
                this.DialogVisible = false;
                Helpers.SetGamePaused(false);
                Helpers.SpawnNewPlayer();
                break;
        }
    }

    private void TowerPlacementFinished(bool success)
    {
        if (!success)
        {
            return;
        }

        TowerPlacemementSystem.FinishedPlacement -= this.TowerPlacementFinished;
        this.SetDialogHint(DialogHint.PLACED_ONE_TOWER);
    }
}