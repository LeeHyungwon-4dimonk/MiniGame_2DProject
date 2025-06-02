public abstract class BaseState
{
    public bool HasPhysics;

    public abstract void Enter();

    public abstract void Update();

    // �������� �̵��� �ʿ��� �ÿ��� FixedUpdate�� ����Ͽ� ���
    public virtual void FixedUpdate() { }

    public abstract void Exit();
}

public enum EState
{
    Idle            /*���*/,
    Walk            /*�ȱ�*/,
    Jump            /*����*/, 
    MeleeAttack     /*���� ����*/, 
    Charge          /*����*/, 
    RangedAttack    /*���Ÿ� ����*/, 
    Die             /*����*/,
    Stun            /*����*/,
    Trace           /*�߰�*/,
}
