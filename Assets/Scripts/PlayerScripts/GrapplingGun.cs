using UnityEngine;

public class GrapplingGun : MonoBehaviour
{   
    public Transform gunPosition;
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappable;
    public Transform gunTip, camera, player;
    public float maxDistance = 100f;
    private SpringJoint joint;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {   

        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (!joint)
        {
            ResetGunRotation();
        }
    }

    void LateUpdate()
    {
         DrawRope();
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 250f;
            joint.damper = 20f;
            joint.massScale = 0.5f;

            lr.positionCount = 2;
        }
    }

    private void StopGrapple()
    {   

        lr.positionCount = 0;
        Destroy(joint);
    }

    void DrawRope()
    {   
        if (!joint) return;

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    private void ResetGunRotation()
    {
        if (gunPosition != null)
        {
            gunPosition.localRotation = Quaternion.Lerp(gunPosition.localRotation, Quaternion.identity, Time.deltaTime * 5f);
        }
    }
}

