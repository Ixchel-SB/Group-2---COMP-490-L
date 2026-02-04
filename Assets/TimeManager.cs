using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public Material morningSkybox;  // drag Morning material here
    public Material daySkybox;      // drag Day material here
    public Material sunsetSkybox;   // drag Sunset material here
    public Material nightSkybox;    // drag Night material here

    public Light sunLight;          // optional, to adjust intensity

    [Range(0f, 24f)]
    public float hours = 12f;       // current hour

    void Update()
    {
        if (hours >= 6f && hours < 9f)
        {
            RenderSettings.skybox = morningSkybox;
            if (sunLight != null) sunLight.intensity = 0.8f;
        }
        else if (hours >= 9f && hours < 17f)
        {
            RenderSettings.skybox = daySkybox;
            if (sunLight != null) sunLight.intensity = 1f;
        }
        else if (hours >= 17f && hours < 19f)
        {
            RenderSettings.skybox = sunsetSkybox;
            if (sunLight != null) sunLight.intensity = 0.6f;
        }
        else
        {
            RenderSettings.skybox = nightSkybox;
            if (sunLight != null) sunLight.intensity = 0.2f;
        }

        DynamicGI.UpdateEnvironment();
    }
}

