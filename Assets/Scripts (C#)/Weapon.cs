using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public PlayerInfo info;
    public void Init(float damage) //  초기화하는 변수
    {
        this.damage = damage;
        

    }
    private void Start()
    {
        Init(info.playerDamage);
    }
}
