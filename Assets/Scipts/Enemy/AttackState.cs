using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        //Debug.Log("发现敌人!!!");
        enemy.animState = 2; //attack状态为2
        enemy.targetPoint = enemy.attackList[0]; //选择attackList里的第一个敌人
    }

    public override void OnUpdate(Enemy enemy)
    {

    }
}
