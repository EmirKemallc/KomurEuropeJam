using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Sonar : MonoBehaviour
{
    public Light2D rightLight; // yuvarlak kısmı sağa bakan yarım daire
    public Light2D leftLight;  // aynı şekil, kod 180° çevirir
    public Light2D upLight;    // aynı şekil, kod 90° çevirir

    [Header("Konum")]
    public Vector2 centerOffset = new Vector2(0f, 0.2f);

    [Header("Işık")]
    public float maxIntensity = 1f;
    public float startSize = 0.5f;
    public float endSize = 3f;
    public float travelDistance = 0.5f;

    [Header("Zamanlama")]
    public float expandDuration = 0.4f;
    public float fadeDuration = 0.5f;

    private Light2D[] lights;
    private Vector3[] directions;

    private void Start()
    {
        lights = new[] { rightLight, leftLight, upLight };
        directions = new[] { Vector3.right, Vector3.left, Vector3.up };

        // Işıklar karakterden ayrılıyor, kedinin -1 scale'inden etkilenmesin
        foreach (var l in lights) l.transform.SetParent(null, true);

        // Şekil sağa bakıyor: sağ = 0°, sol = 180°, yukarı = 90°
        rightLight.transform.rotation = Quaternion.identity;
        leftLight.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        upLight.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        foreach (var l in lights) l.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StopAllCoroutines();
            StartCoroutine(Pulse());
        }
    }

    private void SetLights(Vector3 origin, float dist, float size, float intensity)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].transform.position = origin + directions[i] * dist;
            lights[i].transform.localScale = new Vector3(size, size, 1f); // scale pozitif
            lights[i].intensity = intensity;
        }
    }

    private IEnumerator Pulse()
    {
        Vector3 origin = transform.position + (Vector3)centerOffset;

        SetLights(origin, 0f, startSize, 0f);
        foreach (var l in lights) l.enabled = true;

        float t = 0f;
        while (t < expandDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / expandDuration);
            float e = 1f - Mathf.Pow(1f - p, 3f);

            SetLights(origin,
                travelDistance * e,
                Mathf.Lerp(startSize, endSize, e),
                Mathf.Lerp(0f, maxIntensity, Mathf.Sqrt(p)));
            yield return null;
        }

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / fadeDuration);
            float i = Mathf.Lerp(maxIntensity, 0f, p * p);
            foreach (var l in lights) l.intensity = i;
            yield return null;
        }

        foreach (var l in lights) l.enabled = false;
    }
}