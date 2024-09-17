using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleArea : MonoBehaviour
{
    private Boss parent;
    private void Start()
    {
        parent = GetComponentInParent<Boss>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus playerHealth = other.GetComponentInParent<PlayerStatus>();
            int damage = Random.Range(parent.minAtk, parent.maxAtk);

            bool isCriticalHit = Random.value < parent.criticalHitChance;

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, isCriticalHit);
            }
        }
    }
}
