using UnityEngine;

public class PawAttack : MonoBehaviour
{
    [Header("Pati")]
    public LayerMask pushableLayer;                       // sadece bu layer itilir
    public Vector2 hitOffset = new Vector2(0.8f, 0f);     // sağa bakarkenki kutu merkezi
    public Vector2 hitSize = new Vector2(0.8f, 0.8f);
    public float cooldown = 0.4f;

    [Header("Güç")]
    public float pushForce = 8f;     // yatay güç
    public float upwardForce = 1.5f; // hafif yukarı kaldırır, sürtünmeye takılmasın

    private float nextPawTime;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextPawTime)
        {
            nextPawTime = Time.time + cooldown;
            Swat();
        }
    }

    private float Facing()
    {
        return transform.lossyScale.x < 0 ? -1f : 1f;
    }

    private void Swat()
    {
        float f = Facing();
        Vector2 center = (Vector2)transform.position + new Vector2(hitOffset.x * f, hitOffset.y);

        Debug.Log("Swat çalıştı. Layer mask değeri: " + pushableLayer.value);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, hitSize, 0f, pushableLayer);
        Debug.Log("Bulunan collider sayısı: " + hits.Length);

        foreach (var hit in hits)
        {
            Rigidbody2D body = hit.attachedRigidbody;
            if (body == null) continue;

       
            Vector2 velocity = new Vector2(pushForce * f, upwardForce);
            body.AddForce(velocity * body.mass, ForceMode2D.Impulse);
        }
    }

    // Scene ekranında vuruş alanını görmek için (Play'de ya da editörde)
    private void OnDrawGizmosSelected()
    {
        float f = transform.lossyScale.x < 0 ? -1f : 1f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + new Vector2(hitOffset.x * f, hitOffset.y), hitSize);
    }
}