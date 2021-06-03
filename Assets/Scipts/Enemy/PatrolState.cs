using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        enemy.animState = 0; //idle状态为0
        enemy.SwitchPoint();
    }

    public override void OnUpdate(Enemy enemy)
    {
        //当前animator的BaseLayer不在idle状态（动画中idle设置为播放完就播放run）
        if (!enemy.anim.GetCurrentAnimatorStateInfo(0).IsName("idle")) // 0表示BaseLayer
        {
            enemy.animState = 1; //run状态为1
            enemy.MoveToTarget();
        }

        if (Mathf.Abs(enemy.transform.position.x - enemy.targetPoint.position.x) < 0.01)
        {
            //enemy.SwitchPoint();//不需要
            enemy.TransitionToState(enemy.patrolState);//切换到patrol状态
        }

        if (enemy.attackList.Count > 0)
        {
            enemy.TransitionToState(enemy.attackState);
        }
    }
}
