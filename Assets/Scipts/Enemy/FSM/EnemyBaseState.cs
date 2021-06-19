//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

public abstract class EnemyBaseState //不需要: MonoBehaviour
{
    public abstract void EnterState(Enemy enemy);
    public abstract void OnUpdate(Enemy enemy);
}
