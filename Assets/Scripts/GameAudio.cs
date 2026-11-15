using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public AudioClip placeClip;          // blok yerine oturunca (tek ses)
    public AudioClip[] clearClips;       // satır silinince, içlerinden rastgele biri
    [Range(0f, 1f)] public float volume = 0.8f;

    private AudioSource source;
    private int lastClear = -1;

    void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
    }

    public void PiecePlaced()
    {
        if (placeClip != null) source.PlayOneShot(placeClip, volume);
    }

    public void LinesCleared()
    {
        if (clearClips == null || clearClips.Length == 0) return;

        // Aynı ses art arda iki kez gelmesin
        int i = Random.Range(0, clearClips.Length);
        if (clearClips.Length > 1 && i == lastClear)
            i = (i + 1 + Random.Range(0, clearClips.Length - 1)) % clearClips.Length;
        lastClear = i;

        source.PlayOneShot(clearClips[i], volume);
    }
}