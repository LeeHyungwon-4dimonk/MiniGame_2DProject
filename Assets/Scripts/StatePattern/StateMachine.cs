using System.Collections.Generic;

public class StateMachine
{
    public Dictionary<EState, BaseState> StateDic;
    public BaseState CurState;

    public StateMachine()
    {
        StateDic = new Dictionary<EState, BaseState>();
    }

    public void ChangeState(BaseState changedState)
    {
        if (CurState == changedState) return;

        CurState.Exit();
        CurState = changedState;
        CurState.Enter();
    }

    public void Update() => CurState.Update();

    public void FixedUpdate()
    {
        if(CurState.HasPhysics) CurState.FixedUpdate();
    }
}