using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

//Main wall running script found at: https://learn.unity.com/project/creating-an-fps-wall-run-mechanic-beginner-prototype-series?uv=2020.2
//      (Project download can be found here: https://on.unity.com/3lMUj73, pulled WallRun script)
[RequireComponent(typeof(PlayerController))]
public class WallRun : MonoBehaviour
{
    public float wallMaxDistance = 1;
    public float wallSpeedMultiplier = 1.2f;
    public float minimumHeight = 1.2f;
    public float maxAngleRoll = 20;
    [Range(0.0f, 1.0f)]
    public float normalizedAngleThreshold = 0.1f;

    public float jumpDuration = 1;
    public float wallBouncing = 3;
    public float cameraTransitionDuration = 1;

    public float wallGravityDownForce = 20f;

    public bool useSprint;

    PlayerController controller;

    Vector3[] directions;
    RaycastHit[] hits;

    bool isWallRunning = false;
    Vector3 lastWallPosition;
    Vector3 lastWallNormal;
    float elapsedTimeSinceJump = 0;
    float elapsedTimeSinceWallAttach = 0;
    float elapsedTimeSinceWallDetatch = 0;
    bool jumping;

    //Finds grounded player
    bool IsPlayergrounded() => controller.GetGrounded();

    //Checks conditions for wall running
    bool CanWallRun()
    {
        bool isSprinting = controller.IsSprinting();
        isSprinting = !useSprint ? true : isSprinting;

        return !IsPlayergrounded() && HeightCheck() && isSprinting;
    }

    //Raycast to see if player is a minimum height off of the ground for wall running
    bool HeightCheck()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumHeight);
    }

    //Set our controller and raycasts
    void Start()
    {
        controller = GetComponent<PlayerController>();

        directions = new Vector3[]{
            Vector3.right,
            Vector3.right + Vector3.forward,
            Vector3.forward,
            Vector3.left + Vector3.forward,
            Vector3.left
        };
    }


    public void LateUpdate()
    {
        isWallRunning = false;

        if (CanAttach())
        {
            hits = new RaycastHit[directions.Length];
            //Make raycasts to find nearby walls
            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 dir = transform.TransformDirection(directions[i]);
                Physics.Raycast(transform.position, dir, out hits[i], wallMaxDistance);
                if (hits[i].collider != null)
                {
                    //Object hit
                    Debug.DrawRay(transform.position, dir * hits[i].distance, Color.green);
                }
                else
                {
                    //Object not hit
                    Debug.DrawRay(transform.position, dir * wallMaxDistance, Color.red);
                }
            }

            if (CanWallRun())
            {
                //If wall run conditions met, get walls and select closest one for attaching
                hits = hits.ToList().Where(h => h.collider != null).OrderBy(h => h.distance).ToArray();
                if (hits.Length > 0)
                {
                    OnWall(hits[0]);
                    lastWallPosition = hits[0].point;
                    lastWallNormal = hits[0].normal;
                }
            }
        }

        if (isWallRunning)
        {
            //Timer for wall running
            elapsedTimeSinceWallDetatch = 0;
            elapsedTimeSinceWallAttach += Time.deltaTime;
            controller.playerVelocity += Vector3.down * wallGravityDownForce * Time.deltaTime;
        }
        else
        {
            //Timer for wall running cooldown
            elapsedTimeSinceWallAttach = 0;
            elapsedTimeSinceWallDetatch += Time.deltaTime;
        }
    }

    //Finds if player can attach to a wall. Necessary to avoid attaching near beginning of jump and changing speed/gravity values too early
    bool CanAttach()
    {
        if (jumping)
        {
            elapsedTimeSinceJump += Time.deltaTime;
            if (elapsedTimeSinceJump > jumpDuration)
            {
                elapsedTimeSinceJump = 0;
                jumping = false;
            }
            return false;
        }

        return true;
    }

    //Causes movement along wall
    void OnWall(RaycastHit hit)
    {
        float d = Vector3.Dot(hit.normal, Vector3.up);
        if (d >= -normalizedAngleThreshold && d <= normalizedAngleThreshold)
        {
            //Sets forward movement for wall run
            Vector3 alongWall = transform.TransformDirection(Vector3.forward);

            //Ray in direction of wall
            Debug.DrawRay(transform.position, alongWall.normalized * 10, Color.green);
            //Ray away from wall
            Debug.DrawRay(transform.position, lastWallNormal * 10, Color.magenta);

            //Set player velocity based on wall running logic
            controller.playerVelocity = alongWall * Input.GetAxis("Vertical") * wallSpeedMultiplier;
            //Player is wall running
            isWallRunning = true;
        }
    }

    //Makes player jump off of walls if wall running (needs changing for our scripting purposes)
    public Vector3 GetWallJumpDirection()
    {
        if (isWallRunning)
        {
            return lastWallNormal * wallBouncing + Vector3.up;
        }
        return Vector3.zero;
    }

    //Gets if player is wall running for other scripts
    public bool IsWallRunning()
    {
        return isWallRunning;
    }
}
