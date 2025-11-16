using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform focusOverride;

    [SerializeField] private float followSmoothTime = 0.15f;
    [SerializeField] private float heightY = 3f;
    [SerializeField] private float focusOffsetX = 0f;
    [SerializeField] private float focusOffsetZ = 0f;
    [SerializeField] private bool hardCenter = false;

    [SerializeField] private float normalOrthosize = 11f;
    [SerializeField] private float zoomOrthoSize = 10f;
    [SerializeField] private float zoomLerpSpeed = 8f;

    [SerializeField] private float zoomLead = 6f;

    [SerializeField] private float fixedPitch = 90f;
    [SerializeField] private float fixedYaw = 0f;
    [SerializeField] private float fixedRoll = 0f;



    private Camera cam;
    private Vector3 followVelocity;

    private Vector3 GetFocus()
    {
        if (focusOverride != null)
        {
            return focusOverride.position;

        }

        if (target != null)
        {
            return target.position;
        }

        return transform.position;
    }

    private void SnapToFocus()
    {
        Vector3 f = GetFocus();
        transform.position = new Vector3(f.x + focusOffsetX, heightY, f.z + focusOffsetZ);
    }

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = normalOrthosize;
        }

        SnapToFocus();
        transform.rotation = Quaternion.Euler(fixedPitch, fixedYaw, fixedRoll);
    }

    private void Reset()
    {
        followSmoothTime = 0.15f;
        heightY = 10f;
        fixedPitch = 90f;
        fixedYaw = 0f;
        fixedRoll = 0f;
    }

    private void OnValidate()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }

        if (cam != null)
        {
            cam.orthographic = true;
        }

        transform.rotation = Quaternion.Euler(fixedPitch, fixedYaw, fixedRoll);
    }

    private void LateUpdate()
    {
        if (cam == null)
        {
            return;
        }

        Vector3 baseFocus = GetFocus();

        bool zooming = Input.GetMouseButton(1);

        Vector3 fwd = Vector3.forward;
        if (target != null)
        {
            fwd = target.forward;
        }

        fwd.y = 0f;

        if (fwd.sqrMagnitude > 0.0001f)
        {
            fwd.Normalize();
        }
        else
        {
            fwd = Vector3.forward;
        }

        float lead = zooming ? zoomLead : 0f;

        Vector3 focus = baseFocus + fwd * lead;
        focus.x += focusOffsetX;
        focus.z += focusOffsetZ;

        Vector3 targetPos = new Vector3(focus.x, heightY, focus.z);

        if (hardCenter)
        {
            transform.position = targetPos;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref followVelocity, followSmoothTime);
        }

        transform.rotation = Quaternion.Euler(fixedPitch, fixedYaw, fixedRoll);
        if(!cam.orthographic)
        {
            cam.orthographic = true;
        }

        float targetSize = zooming ? zoomOrthoSize : normalOrthosize;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomLerpSpeed * Time.deltaTime);
    }
}