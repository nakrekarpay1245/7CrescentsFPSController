using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health;
    public void TakeDamage(float value)
    {
        health -= value;
        if (health <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Debug.Log("I am sorry bro I died");
    }
}
