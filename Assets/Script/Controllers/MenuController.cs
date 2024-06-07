// Controllers/MenuController.cs

using UnityEngine;
using Zenject;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    private IMenuService menuService;

    [Inject]
    public void Construct(IMenuService menuService)
    {
        this.menuService = menuService;
    }

    public void ShowMainMenu()
    {
        menuService.ShowMenu(mainMenu);
    }

    public void HideMainMenu()
    {
        menuService.HideMenu(mainMenu);
    }

    public void ShowPauseMenu()
    {
        menuService.ShowMenu(pauseMenu);
    }

    public void HidePauseMenu()
    {
        menuService.HideMenu(pauseMenu);
    }

    public void ShowSettingsMenu()
    {
        menuService.ShowMenu(settingsMenu);
    }

    public void HideSettingsMenu()
    {
        menuService.HideMenu(settingsMenu);
    }

    public void StartGame()
    {
        menuService.StartGame();
    }

    public void ExitGame()
    {
        menuService.ExitGame();
    }
}