using System.Threading.Tasks;
using Buildings.Towers;
using Cameras;
using Enemies;
using Interface.Elements;
using NaughtyAttributes;
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
        if (GameoverBehaviour.Instance.IsShown)
        {
            return;
        }

        this.hint = hint;
        switch (hint)
        {
            case DialogHint.INTRO:
                this.dialog.DialogText = "We've crashed on an alien planet. Only three of the crew members have survived. We better build some defenses against the hostile creatures.";
                this.dialog.ButtonText = "Got it";
                break;

            case DialogHint.PLACED_ONE_TOWER:
                this.dialog.DialogText = "We should use all of our remaining building resources now, as there will not be enough time once the enemies attack. (Luckily, the enemies are only starting their attack after clicking the start wave button at the top)";
                this.dialog.ButtonText = "Got it";
                break;

            case DialogHint.BEAT_FIRST_WAVE: // After first wave is completed
                this.dialog.DialogText = "The rescue team will arrive in seven minutes. We have to make due with the limited ammo supply until they arrive.";
                this.dialog.ButtonText = "Got it";
                break;

            case DialogHint.PLAYER_MOVEMENT:
                this.dialog.DialogText = "Use the WASD keys to move around the map. Collect & return the ammo dropped by enemies to the base.";
                this.dialog.ButtonText = "Finish tutorial";
                break;

            case DialogHint.PLAYER_LOST_LIFE:
                this.dialog.DialogText = "The current ammo collector is missing in action. One of the remaining survivors has to take their place.";
                this.dialog.ButtonText = "Got it";
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
                InterfaceBehaviour.Instance.SetElementVisible(Element.GENERIC_HINT, true);
                this.DialogVisible = false;
                break;

            case DialogHint.PLACED_ONE_TOWER:
                InterfaceBehaviour.Instance.SetElementVisible(Element.TOWER_CARDS, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.BUILDING_RESOURCES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.AMMO_RESOURCES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.PLAYER_LIFES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.START_WAVE, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.GENERIC_HINT, true);
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
                InterfaceBehaviour.Instance.SetElementVisible(Element.GENERIC_HINT, true);
                Helpers.StartEnemyWaveSpawner();
                Helpers.EnablePlayerMovement();
                Helpers.StartTimer();
                this.DialogVisible = false;
                break;

            case DialogHint.PLAYER_LOST_LIFE:
                InterfaceBehaviour.Instance.SetElementVisible(Element.PLAYER_LIFES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.AMMO_RESOURCES, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.TIMER, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.INENVTORY_BAR, true);
                InterfaceBehaviour.Instance.SetElementVisible(Element.GENERIC_HINT, true);
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