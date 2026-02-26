using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject SettingsPanel;
    public void StartTheGame()
    {
        SceneManager.LoadScene(0);
    }
    public void OpenSettings()
    {
        SettingsPanel.SetActive(true);
    }

}
