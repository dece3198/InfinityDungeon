using UnityEngine;

public enum TextType
{
    Normal, Critical
}

public abstract class BaseState<T>
{
    public abstract void Enter(T state);
    public abstract void Update(T state);
    public abstract void FixedUpdate(T state);
    public abstract void Exit(T state);
}

public class Monster : MonoBehaviour
{
}
