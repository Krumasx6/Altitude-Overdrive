using UnityEngine;

public class RotateGun : MonoBehaviour
{
    public SwingingGun swinging;
    
    [Header("Which Gun Is This?")]
    public bool isLeftGun = true;
    
    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;

    void Update()
    {
        bool isSwinging = isLeftGun ? swinging.IsSwingingLeft() : swinging.IsSwingingRight();
        
        if (!isSwinging) return;

        Vector3 grapplePoint = isLeftGun ? swinging.GetGrapplePointLeft() : swinging.GetGrapplePointRight();
        
        Quaternion targetRotation = Quaternion.LookRotation(grapplePoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}