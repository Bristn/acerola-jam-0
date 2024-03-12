using Enemies;
using Interface.Elements;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

public class GameoverBehaviour : MonoBehaviour
{
    public static GameoverBehaviour Instance { get; private set; }

    public enum GameoverType
    {
        REACHED_BASE,
        OUT_OF_LIFES,
        VICTORY,
    }

    /* --- References --- */

    [SerializeField][BoxGroup("References")] private UIDocument document;

    /* --- Values --- */

    private VisualElement root;
    private Label titleLabel;
    private Label reasonLabel;
    private VisualElement actionBackground;

    /* --- Properties --- */

    public bool IsShown { get; private set; }

    private void Awake()
    {
        Instance = this;

        this.root = this.document.rootVisualElement;
        this.titleLabel = (Label)this.root.Query("title-label");
        this.reasonLabel = (Label)this.root.Query("reason-label");
        this.actionBackground = this.root.Query("action-background");

        this.root.SetDisplayStyle(DisplayStyle.None);
        this.IsShown = false;
    }

    public void ShowGameOver(GameoverType type)
    {
        if (this.root == null)
        {
            return;
        }

        switch (type)
        {
            case GameoverType.REACHED_BASE:
                this.titleLabel.SetText("Game over");
                this.reasonLabel.SetText("The enemies have overrun the base!");
                this.actionBackground.SetCustomStyle(BackgroundColor.Instance, BackgroundColor.Color.SECONDARY);
                break;

            case GameoverType.OUT_OF_LIFES:
                this.titleLabel.SetText("Game over");
                this.reasonLabel.SetText("There is no one left to defend the base!");
                this.actionBackground.SetCustomStyle(BackgroundColor.Instance, BackgroundColor.Color.SECONDARY);
                break;

            case GameoverType.VICTORY:
                this.titleLabel.SetText("Victory");
                this.reasonLabel.SetText("You've suceessfully defended the base!");
                this.actionBackground.SetCustomStyle(BackgroundColor.Instance, BackgroundColor.Color.PRIMARY);
                break;
        }

        this.IsShown = true;
        Helpers.SetGamePaused(true);
        Helpers.RecenterCamera();
        this.root.SetDisplayStyle(DisplayStyle.Flex);
    }
}