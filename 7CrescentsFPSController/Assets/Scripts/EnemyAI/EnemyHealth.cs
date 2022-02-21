using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Tooltip("Can miktarı")]
    public int health;

    [Tooltip("Can gösterge metni")]
    public Text healthText;

    [Tooltip("Darbe alındı(etkilendi)")]
    public bool impact;
    public void TakeDamage()
    {
       // Debug.Log("Impact" + impact) ;
        health--;
        healthText.text = health.ToString();
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            //Debug.Log("Impact" + impact);

            impact = true;
        }
    }
}
