public abstract class BaseState
{
    public bool HasPhysics;

    public abstract void Enter();

    public abstract void Update();

    // 물리적인 이동이 필요할 시에만 FixedUpdate를 상속하여 사용
    public virtual void FixedUpdate() { }

    public abstract void Exit();
}

public enum EState
{
    Idle            /*대기*/,
    Walk            /*걷기*/,
    Jump            /*점프*/, 
    MeleeAttack     /*근접 공격*/, 
    Charge          /*차지*/, 
    RangedAttack    /*원거리 공격*/, 
    Die             /*죽음*/,
    Stun            /*스턴*/,
    Trace           /*추격*/,
}
