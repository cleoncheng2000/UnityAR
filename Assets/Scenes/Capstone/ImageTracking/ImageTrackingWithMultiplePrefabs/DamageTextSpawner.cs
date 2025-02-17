using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DamageTextSpawner : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public Button damagebutton;

    void Start()
    {
        damagebutton.onClick.AddListener(() => SpawnDamageText(new Vector3(0, 0, 0), 100));
    }

    public void SpawnDamageText(Vector3 position, int damageAmount)
    {
        GameObject damageTextInstance = Instantiate(damageTextPrefab, position, Quaternion.identity);
        DamageTextController damageText = damageTextInstance.GetComponent<DamageTextController>();
        damageText.SetDamageText(damageAmount);
    }
}
