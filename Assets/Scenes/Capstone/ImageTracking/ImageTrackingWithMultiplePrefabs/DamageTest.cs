using UnityEngine;
using UnityEngine.UI;

public class DamageLabelTest : MonoBehaviour
    {
        [SerializeField] private int critChance = 10;
        [SerializeField] private int critMultiplier = 2;

        [SerializeField] private int minDamage = 10;
        [SerializeField] private int maxDamage = 30;

        [SerializeField] private Vector3 exampleLocationOfTarget = Vector3.zero;

        public Button damageButton;

        private void Start()
        {
            damageButton.onClick.AddListener(DoDamage);
        }        

        public void DoDamage()
        {
            var damage = Random.Range(minDamage, maxDamage);
            var isCrit = Random.Range(0, 100) < critChance;
            if (isCrit)
                damage *= critMultiplier;
            
            SpawnsDamagePopups.Instance.DamageDone(damage, exampleLocationOfTarget, isCrit);
        }
        
    }