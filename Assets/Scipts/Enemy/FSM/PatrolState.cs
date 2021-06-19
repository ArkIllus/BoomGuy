using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        enemy.animState = 0; //进入Patrol状态时先令动画状态=idle=0
        enemy.SwitchPoint();
    }

    public override void OnUpdate(Enemy enemy)
    {
        //当前animator的BaseLayer不在idle状态（动画中idle设置为播放完就播放run）
        if (!enemy.anim.GetCurrentAnimatorStateInfo(0).IsName("idle")) // 0表示BaseLayer
        {
            enemy.animState = 1; //处于patrol状态时，检测到idle动画放完后，令动画状态=run=1
            enemy.MoveToTarget();
        }

        //到达目标点后，也是先idle一会儿再向另一个目标点run，即再次进入Patrol状态
        if (Mathf.Abs(enemy.transform.position.x - enemy.targetPoint.position.x) < 0.01f)
        {
            //enemy.SwitchPoint();//不需要
            enemy.TransitionToState(enemy.patrolState);//切换到Patrol状态
        }

        //检测到敌人（Player或Bomb），进入Attack状态
        if (enemy.attackList.Count > 0)
        {
            enemy.TransitionToState(enemy.attackState);
        }
    }
}
