using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private float turnSpeedDeg = 540f;     // rotation(degree) per sec
    [SerializeField] private float modelForwardOffsetDeg = 0f;

    [SerializeField] private float aimPlaneYFromPlayer = 0f;    // if 0 player's present Y axiz

    private Rigidbody rb;
    private float desiredYaw;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (cam == null )
        {
            cam = Camera.main;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null)
        {
            return;
        }

        Ray ray = cam.ScreenPointToRay( Input.mousePosition );

        float planeY = transform.position.y + aimPlaneYFromPlayer;
        Plane plane = new Plane(Vector3.up, new Vector3(0f, planeY, 0f));

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);

            Vector3 to = hit - transform.position;
            to.y = 0f;
            if (to.sqrMagnitude > 0.0001f)
            {
                float yaw = Mathf.Atan2(to.x, to.z) * Mathf.Rad2Deg;

                yaw += modelForwardOffsetDeg;

                desiredYaw = yaw;
            }
        }
    }

    private void FixedUpdate()
    {
        Quaternion current = transform.rotation;
        Quaternion target = Quaternion.Euler(0f, desiredYaw, 0f);

        float maxStep = turnSpeedDeg * Time.fixedDeltaTime;

        if (rb != null && !rb.isKinematic)
        {
            rb.MoveRotation(Quaternion.RotateTowards(current, target, maxStep));
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(current, target, maxStep);
        }
    }
}
