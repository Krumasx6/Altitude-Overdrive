using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwingingGun : MonoBehaviour
{       

    [Header("References")]
    public Transform gunPosLeft, gunPosRight;
    public LineRenderer lrLeft, lrRight;
    private Vector3 grapplePointLeft, grapplePointRight;
    public LayerMask whatIsGrappleable;
    public Transform gunTipLeft, gunTipRight, camera, player;
    private SpringJoint jointLeft;
    private SpringJoint jointRight;
    [SerializeField] private PlayerMovement pm;

    [Header("Swinging Settings")]
    public float maxDistance = 25f;
    public bool isSwingingLeft;
    public bool isSwingingRight;

    [Header("Keybinds Settings")]
    public KeyCode leftGrapple = KeyCode.Q;
    public KeyCode rightGrapple = KeyCode.E;

    void Awake()
    {
        if (lrLeft == null && gunPosLeft != null)
        {
            lrLeft = gunPosLeft.GetComponent<LineRenderer>();
            if (lrLeft == null)
            {
                lrLeft = gunPosLeft.gameObject.AddComponent<LineRenderer>();
                lrLeft.startWidth = 0.1f;
                lrLeft.endWidth = 0.1f;
            }
        }
        
        if (lrRight == null && gunPosRight != null)
        {
            lrRight = gunPosRight.GetComponent<LineRenderer>();
            if (lrRight == null)
            {
                lrRight = gunPosRight.gameObject.AddComponent<LineRenderer>();
                lrRight.startWidth = 0.1f;
                lrRight.endWidth = 0.1f;
            }
        }

        if (pm == null)
            pm = FindFirstObjectByType<PlayerMovement>();
    }

    void Update()
    {   
        // Left Grapple
        if (Input.GetKeyDown(leftGrapple)) StartSwingingLeft();
        if (Input.GetKeyUp(leftGrapple)) StopSwingingLeft();

        // Right Grapple
        if (Input.GetKeyDown(rightGrapple)) StartSwingingRight();
        if (Input.GetKeyUp(rightGrapple)) StopSwingingRight();

        pm.swinging = isSwingingLeft || isSwingingRight;

        // Reset Gun Rotation
        if (!jointLeft && !jointRight)
        {
            ResetGunRotation();
        }
    }

    void LateUpdate()
    {
         DrawRope();
    }

    private void StartSwingingLeft()
    {   
        pm.swinging = true;
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            isSwingingLeft = true;
            
            grapplePointLeft = hit.point;
            jointLeft = player.gameObject.AddComponent<SpringJoint>();
            jointLeft.autoConfigureConnectedAnchor = false;
            jointLeft.connectedAnchor = grapplePointLeft;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePointLeft);

            jointLeft.maxDistance = distanceFromPoint * 0.8f;
            jointLeft.minDistance = distanceFromPoint * 0.25f;

            jointLeft.spring = 100f;
            jointLeft.damper = 10f;
            jointLeft.massScale = 0.5f;

            lrLeft.positionCount = 2;
        }
    }

    private void StartSwingingRight()
    {   
        pm.swinging = true;
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            isSwingingRight = true;
            
            grapplePointRight = hit.point;
            jointRight = player.gameObject.AddComponent<SpringJoint>();
            jointRight.autoConfigureConnectedAnchor = false;
            jointRight.connectedAnchor = grapplePointRight;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePointRight);

            jointRight.maxDistance = distanceFromPoint * 0.8f;
            jointRight.minDistance = distanceFromPoint * 0.25f;

            jointRight.spring = 100f;
            jointRight.damper = 10f;
            jointRight.massScale = 0.5f;

            lrRight.positionCount = 2;
        }
    }

    private void StopSwingingLeft()
    {   
        pm.swinging = false;
        isSwingingLeft = false;
        lrLeft.positionCount = 0;
        Destroy(jointLeft);
    }

    private void StopSwingingRight()
    {   
        pm.swinging = false;
        isSwingingRight = false;
        lrRight.positionCount = 0;
        Destroy(jointRight);
    }

    void DrawRope()
    {   
        // Draw left rope
        if (jointLeft)
        {
            lrLeft.SetPosition(0, gunTipLeft.position);
            lrLeft.SetPosition(1, grapplePointLeft);
        }

        // Draw right rope
        if (jointRight)
        {
            lrRight.SetPosition(0, gunTipRight.position);
            lrRight.SetPosition(1, grapplePointRight);
        }
    }

    public bool IsSwinging()
    {
        return jointLeft != null || jointRight != null;
    }

    public bool IsSwingingLeft()
    {
        return jointLeft != null;
    }

    public bool IsSwingingRight()
    {
        return jointRight != null;
    }

    public Vector3 GetGrapplePointLeft()
    {
        return grapplePointLeft;
    }

    public Vector3 GetGrapplePointRight()
    {
        return grapplePointRight;
    }

    private void ResetGunRotation()
    {
        if (gunPosLeft != null)
        {
            gunPosLeft.localRotation = Quaternion.Lerp(gunPosLeft.localRotation, Quaternion.identity, Time.deltaTime * 5f);
        }
        
        if (gunPosRight != null)
        {
            gunPosRight.localRotation = Quaternion.Lerp(gunPosRight.localRotation, Quaternion.identity, Time.deltaTime * 5f);
        }
    }
}