using UnityEngine;
using TMPro;

public class DamageTextController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fadeSpeed = 2f;
    public float lifetime = 1f;
    
    private TextMeshProUGUI text;
    private Color originalColor;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalColor = text.color;
    }

    public void SetDamageText(int damageAmount)
    {
        text.text = damageAmount.ToString();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, text.color.a - (fadeSpeed * Time.deltaTime));
    }
}