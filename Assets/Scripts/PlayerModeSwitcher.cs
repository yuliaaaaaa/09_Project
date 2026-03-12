using UnityEngine;

public enum PlayerMode { Ground, Water }
public class PlayerModeSwitcher : MonoBehaviour
{
    public PlayerMode currentMode = PlayerMode.Ground;

    public GroundPlayerController groundController;
    public PlayerController waterController;

    public Animator groundAnimator;
    public Animator waterAnimator;

    public GameObject groundSpriteRoot;
    public GameObject waterSpriteRoot;

    public void SetMode(PlayerMode mode)
    {
        if (currentMode == mode) return;
        currentMode = mode;
        ApplyMode(mode);
    }

    private void ApplyMode(PlayerMode mode)
    {
        bool isWater = (mode == PlayerMode.Water);
        if (groundController) groundController.enabled = !isWater;
        if(waterController) waterController.enabled = isWater;

        if (groundSpriteRoot) groundSpriteRoot.SetActive(!isWater);
        if (waterSpriteRoot) waterSpriteRoot.SetActive(isWater);
    }
}
