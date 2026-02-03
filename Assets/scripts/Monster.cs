using UnityEngine;

public class Monster : MonoBehaviour 
{ 
    [Header("몬스터 데이터")] 
    
    public float speed = 2.5f; 
    public float changeDirIntervalMin = 0.3f; 
    public float changeDirIntervalMax = 0.7f; //이동까지의 시간 간격을 랜덤으로! 
    public int health =10; 
    public int level; 
    Rigidbody2D rigidbody; 
    Vector2 dir; 
    float timer; 
    void Awake() 
    { 
        rigidbody = GetComponent<Rigidbody2D>(); 
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
}