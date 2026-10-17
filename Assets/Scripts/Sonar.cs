using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal; // URP 2D Light paketi

public class SonarLight : MonoBehaviour
{
    public Light2D sonarLight;
    public float maxRadius = 8f;
    public float expandSpeed = 15f;
    public float shrinkSpeed = 5f;

    public void TriggerSonar()
    {
        StartCoroutine(PulseEffect());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerSonar();
        }
    }

    IEnumerator PulseEffect()
    {
        
        while (sonarLight.pointLightOuterRadius < maxRadius)
        {
            sonarLight.pointLightOuterRadius += Time.deltaTime * expandSpeed;
            yield return null;
        }

        
        while (sonarLight.pointLightOuterRadius > 0)
        {
            sonarLight.pointLightOuterRadius -= Time.deltaTime * shrinkSpeed;
            yield return null;
        }

        sonarLight.pointLightOuterRadius = 0f;
    }
}