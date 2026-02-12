using UnityEngine;

public class RotateGun : MonoBehaviour
{
    public GrapplingGun grappling;

    // Update is called once per frame
    void Update()
    {
        if (!grappling.IsGrappling()) return;
        transform.LookAt(grappling.GetGrapplePoint());
    }
}
