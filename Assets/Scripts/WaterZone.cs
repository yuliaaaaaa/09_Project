using UnityEngine;

public class WaterZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerModeSwitcher switcher = other.GetComponent<PlayerModeSwitcher>();
        if (switcher != null)
        {
            switcher.SetMode(PlayerMode.Water);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerModeSwitcher switcher = other.GetComponent<PlayerModeSwitcher>();
        if (switcher != null)
        {
            switcher.SetMode(PlayerMode.Ground);
        }
    }


}
