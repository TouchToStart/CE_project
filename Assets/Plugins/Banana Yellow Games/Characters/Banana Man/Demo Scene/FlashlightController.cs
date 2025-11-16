using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FlashlightController : MonoBehaviour
{
    public bool isON = true;
    public bool isZooming = false;

    public float normalAngle = 90f;
    public float normalRange = 10f;
    public float normalIntensity = 15f;

    public float zoomAngle = 25f;
    public float zoomRange = 25f;
    public float zoomIntensity = 20f;

    public KeyCode toggleKey = KeyCode.F;
    public Light spotlight;

    private void Awake()
    {
        if (spotlight == null)
        {
            spotlight = GetComponent<Light>();
            if (spotlight == null )
            {
                spotlight = gameObject.AddComponent<Light>();
            }
        }
        spotlight.type = UnityEngine.LightType.Spot;
    }

    private void Update()
    {
        if (toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey))
        {
            isON = !isON;
        }

        isZooming = Input.GetMouseButton(1);

        float targetRange;
        float targetIntensity;
        float targetOuterAngle;

        if (isZooming)
        {
            targetRange = zoomRange;
            targetIntensity = zoomIntensity;
            targetOuterAngle = zoomAngle;
        }
        else
        {
            targetRange = normalRange;
            targetIntensity = normalIntensity;
            targetOuterAngle = normalAngle;
        }

        if (!isON)
        {
            spotlight.intensity = 0f;
            spotlight.range = 0f;
            spotlight.spotAngle = targetOuterAngle;
        }
        else
        {
            spotlight.intensity = targetIntensity;
            spotlight.range = targetRange;
            spotlight.spotAngle = targetOuterAngle;
        }

        try
        {
            spotlight.innerSpotAngle = 0f;
        }
        catch
        {
            ;
        }
    }

    private void OnValidate()
    {
        if (spotlight != null)
        {
            if (spotlight.type != UnityEngine.LightType.Spot)
            {
                spotlight.type = UnityEngine.LightType.Spot;
            }
        }
    }
}