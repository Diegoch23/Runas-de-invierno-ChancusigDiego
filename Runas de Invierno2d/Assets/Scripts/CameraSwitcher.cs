using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera followCamera;

    void Start()
    {
        mainCamera.enabled = false;
        followCamera.enabled = true;
    }
}
