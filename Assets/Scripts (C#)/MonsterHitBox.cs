using UnityEngine;

public class MonsterHitBox : MonoBehaviour
{
    Monster monster;

    void Awake()
    {
        monster = GetComponentInParent<Monster>();
    }

    public void TakeDamage(float dmg)
    {
        if (monster == null) return;
        monster.TakeDamage(dmg);
    }
}