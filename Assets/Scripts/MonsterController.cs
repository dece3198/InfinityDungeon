using UnityEngine;

public class MonsterController : Monster
{
    [SerializeField] private float hp;
    public float HP 
    {
        get { return hp; }
        set 
        { 
            hp = value; 
        } 
    }
}
