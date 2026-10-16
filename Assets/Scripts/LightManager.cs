using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private Light[] lights;
    private float[] originalIntensity;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("FactoryLight");

        lights = new Light[objs.Length];
        originalIntensity = new float[objs.Length];

        for (int i = 0; i < objs.Length; i++)
        {
            lights[i] = objs[i].GetComponent<Light>();
            originalIntensity[i] = lights[i].intensity;
        }
    }

    public void StartFlickerTense()
     {
        foreach (Light l in lights)
        {
            StartCoroutine(FlickerLight(l));
        }
    }

    public void StartFlickerChase()
     {
        foreach (Light l in lights)
        {
            StartCoroutine(FlickerLight(l));
        }
    }

    public void SetLightIntensity(float multiplier)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = originalIntensity[i] * multiplier;
        }
    }

    IEnumerator FlickerLight(Light lightObj)
    {
        while (true)
        {
            lightObj.enabled = !lightObj.enabled;

            yield return new WaitForSeconds(
                Random.Range(0.05f, 0.8f)
            );
        }
    }
}
