using UnityEngine;

public interface IMenuService
{
    void ShowMenu(GameObject menu);
    void HideMenu(GameObject menu);
    void StartGame();
    void ExitGame();
}