using UnityEngine;

public class Monster : MonoBehaviour 
{ 
    [Header("몬스터 정보")] 
    public int level; 
    public float maxHealth; //max값으로 health를 초기화
    private float speed; 
    private float changeDirIntervalMin; 
    private float changeDirIntervalMax; //이동까지의 시간 간격을 랜덤으로! 
    [Header("몬스터 개별 체력")]
    public float health; //런타입값. 나중에 몬스터 체력 여기서 깎임
    Rigidbody2D rigidbody; 
    Vector2 dir; 
    float timer; 
    void Awake() 
    { 
        rigidbody = GetComponent<Rigidbody2D>(); 
        health = maxHealth;
    } 
    void OnEnable() 
    { 
        RandomDirection(); ResetTimer(); 
    } 
    void FixedUpdate() 
    { 
        rigidbody.MovePosition(rigidbody.position + dir * speed * Time.fixedDeltaTime); 
        timer -= Time.fixedDeltaTime; 
        if (timer <= 0f) 
        { 
            RandomDirection(); 
            ResetTimer(); 
        } 
    } 
    void OnCollisionEnter2D(Collision2D collision) 
    { // 벽에 막히면 방향 바꾸기 
        RandomDirection(); 
        ResetTimer(); 
        //임시로 작성. 벽에 부딪히면 체력 감소
        health=health-0.1f;
    } 
    void RandomDirection() 
    { 
        int r = Random.Range(0, 4); 
        dir = r 
        switch 
        { 
            0 => Vector2.up, 
            1 => Vector2.down, 
            2 => Vector2.left, 
            3 => Vector2.right, 
        }; 
    } 
    void ResetTimer() 
    { 
        timer = Random.Range(changeDirIntervalMax,changeDirIntervalMin); 
    } 

    public void ApplyMonsterInfo(MonsterInfo info)
    {
        level = info.level;
        maxHealth = info.maxHealth;
        health = info.maxHealth;
        speed = info.speed;
        changeDirIntervalMax = info.changeDirIntervalMax;
        changeDirIntervalMin = info.changeDirIntervalMin;
    }

}