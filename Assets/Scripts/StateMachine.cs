using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T1, T2> where T2 : MonoBehaviour
{
    private T2 Unit;
    private BaseState<T2> curState;
    private Dictionary<T1, BaseState<T2>> stateDic;

    public void Reset(T2 unit)
    {
        this.Unit = unit;
        curState = null;
        stateDic = new Dictionary<T1, BaseState<T2>>();
    }

    public void Update()
    {
        curState.Update(Unit);
    }

    public void FixedUpdate()
    {
        curState.FixedUpdate(Unit);
    }

    public void AddState(T1 state, BaseState<T2> baseState)
    {
        stateDic.Add(state, baseState);
    }

    public void ChangeState(T1 state)
    {
        if (curState != null)
        {
            curState.Exit(Unit);
        }
        curState = stateDic[state];
        curState.Enter(Unit);
    }
}