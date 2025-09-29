using UnityEngine;

public enum StageState
{
    None, Start, Wait, Exit
}

public class StageNone : BaseState<StageManager>
{
    public override void Enter(StageManager state)
    {
    }

    public override void Exit(StageManager state)
    {
    }

    public override void FixedUpdate(StageManager state)
    {
    }

    public override void Update(StageManager state)
    {
    }
}

public class StageStart : BaseState<StageManager>
{
    public override void Enter(StageManager state)
    {
    }

    public override void Exit(StageManager state)
    {
    }

    public override void FixedUpdate(StageManager state)
    {
    }

    public override void Update(StageManager state)
    {
    }
}

public class StageWait : BaseState<StageManager>
{
    public override void Enter(StageManager state)
    {
    }

    public override void Exit(StageManager state)
    {
    }

    public override void FixedUpdate(StageManager state)
    {
    }

    public override void Update(StageManager state)
    {
    }
}

public class StageExit : BaseState<StageManager>
{
    public override void Enter(StageManager state)
    {
    }

    public override void Exit(StageManager state)
    {
    }

    public override void FixedUpdate(StageManager state)
    {
    }

    public override void Update(StageManager state)
    {
    }
}



public class StageManager : Singleton<StageManager>
{
    [SerializeField] private Transform[] startPos;
    public StageState stageState;
    private StateMachine<StageState, StageManager> stateMachine = new StateMachine<StageState, StageManager> ();

    private new void Awake()
    {
        base.Awake();
        stateMachine.Reset(this);
        stateMachine.AddState(StageState.None, new StageNone());
        stateMachine.AddState(StageState.Start, new StageStart());
        stateMachine.AddState(StageState.Wait, new StageWait());
        stateMachine.AddState(StageState.Exit, new StageExit());
        ChangeState(StageState.None);
    }

    public void ChangeState(StageState state)
    {
        stateMachine.ChangeState(state);
        stageState = state;
    }
}
