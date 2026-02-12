using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    public Transform cameraPos;
    // Update is called once per frame
    private void Update()
    {
        transform.position = cameraPos.position;
    }
}
