using UnityEngine;

public class DamagePopUpAnimation : MonoBehaviour
{
    public AnimationCurve opacityCurve;
    private TMPro.TextMeshProUGUI text;
    private float time = 0;
    private Vector3 origin;
    private float duration = 1f; // Duration of the animation

    void Start()
    {
        text = transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        origin = transform.position;
    }

    void Update()
    {
        // Update the opacity
        text.color = new Color(1, 1, 1, opacityCurve.Evaluate(time / duration));

        // Update the scale from 2x to 1x
        float scale = Mathf.Lerp(2f, 1f, time / duration);
        transform.localScale = Vector3.one * scale;

        // Update the height from 0 to 10
        float height = Mathf.Lerp(0f, 10f, time / duration);
        transform.position = origin + new Vector3(0, height, 0);

        // Increment time
        time += Time.deltaTime;

        // Destroy the pop-up after the animation is complete
        if (time > duration)
        {
            Destroy(gameObject);
        }
    }
}
