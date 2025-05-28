using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float MoveSpeed;
    [SerializeField] public float JumpPow;

    public StateMachine StateMach;
    public Animator Anim;
    public Rigidbody2D Rigid;
    public SpriteRenderer SpriteRenderer;

    public Vector2 InputX;

    public bool IsMove;
    public bool IsJump;

    private void Awake()
    {
        Anim = GetComponent<Animator>();
        Rigid = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        StateMachineInit();
    }

    private void StateMachineInit()
    {
        StateMach = new StateMachine();
        StateMach.StateDic.Add(EState.Idle, new Player_Idle(this));
        StateMach.StateDic.Add(EState.Walk, new Player_Walk(this));

        StateMach.CurState = StateMach.StateDic[EState.Idle];
    }

    private void Update()
    {
        StateMach.Update();
    }

    private void FixedUpdate()
    {
        StateMach.FixedUpdate();
    }

    public Vector2 GetMove()
    {
        Vector2 direction = transform.right * InputX.x;
        return direction.normalized;
    }

    public void OnMove(InputValue value)
    {
        InputX = value.Get<Vector2>();
        InputX.Normalize();
    }
}
