using UnityEngine;

public class MenuService : IMenuService
{
    public void ShowMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void HideMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void StartGame()
    {
        // Add logic to start the game, e.g., load the main game scene
        Debug.Log("Game Started");
    }

    public void ExitGame()
    {
        // Add logic to exit the game
        Debug.Log("Game Exited");
        Application.Quit();
    }
}