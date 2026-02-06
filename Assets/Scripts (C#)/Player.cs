using UnityEngine;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    Rigidbody2D rigid;
    SpriteRenderer spriter;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

    }
    private void LateUpdate()
    {
        if (inputVec.x != 0) // °È±â
        {
            spriter.flipX = inputVec.x < 0;
        }
    }
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
}