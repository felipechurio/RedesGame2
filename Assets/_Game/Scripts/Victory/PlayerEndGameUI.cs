using Fusion;
using UnityEngine;

public class PlayerEndGameUI : NetworkBehaviour
{
    public static PlayerEndGameUI Local;

    [SerializeField] private GameObject victoryUI;
    [SerializeField] private GameObject defeatUI;
    [SerializeField] private GameObject quitButton;

    public override void Spawned()
    {
        if (Object.InputAuthority == Runner.LocalPlayer)
        {
            Local = this;

            victoryUI.SetActive(false);
            defeatUI.SetActive(false);
            quitButton.SetActive(false);
        }
        else
        {
            victoryUI.SetActive(false);
            defeatUI.SetActive(false);
            quitButton.SetActive(false);
        }
    }

    public void ShowEndGameUI(bool won)
    {
        victoryUI.SetActive(won);
        defeatUI.SetActive(!won);
        quitButton.SetActive(true);
    }
}