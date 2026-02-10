using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public PlayerInfo info;
    Rigidbody2D rigid;
    public void Init(float damage) //  초기화하는 변수
    {
        this.damage = damage;


    }
    private void Start()
    {
        Init(info.playerDamage);
        gameObject.SetActive(false);
        Debug.Log("무기 활성화");
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
}