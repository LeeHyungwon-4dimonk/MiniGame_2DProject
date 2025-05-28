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
    public InputAction JumpAction;

    public bool IsMove;
    public bool IsJump;
    public bool IsLand;

    private void Awake()
    {
        Anim = GetComponent<Animator>();
        Rigid = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        JumpAction = GetComponent<PlayerInput>().actions["Jump"];
        StateMachineInit();
    }

    private void StateMachineInit()
    {
        StateMach = new StateMachine();
        StateMach.StateDic.Add(EState.Idle, new Player_Idle(this));
        StateMach.StateDic.Add(EState.Walk, new Player_Walk(this));
        StateMach.StateDic.Add(EState.Jump, new Player_Jump(this));

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

    public void OnMove(InputValue value)
    {
        InputX = value.Get<Vector2>();
        InputX.Normalize();
    }

    public void OnJump()
    {
        if (IsJump == false && JumpAction.IsPressed())
        {
            IsJump = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            IsJump = false;
            IsLand = true;            
        }
    }
}
