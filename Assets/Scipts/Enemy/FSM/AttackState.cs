using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    public override void EnterState(Enemy enemy)
    {
        //Debug.Log("发现敌人!!!");
        enemy.animState = 2; //进入Attack状态时先令动画状态=attack=2
        enemy.targetPoint = enemy.attackList[0]; //后续OnUpdate会实时判断选择目标
    }

    public override void OnUpdate(Enemy enemy)
    {
        if (enemy.hasBomb) return; //（避免目标设为手上的炸弹，左右反复移动）

        if (enemy.attackList.Count == 0)
        {
            enemy.TransitionToState(enemy.patrolState);
        }
        //存在多个目标时，实时判断选择目标(这里选择最近的目标)
        if (enemy.attackList.Count > 1)
        {
            for (int i = 0; i < enemy.attackList.Count; i++)
            {
                if(Mathf.Abs(enemy.transform.position.x - enemy.attackList[i].position.x) <
                   Mathf.Abs(enemy.transform.position.x - enemy.targetPoint.position.x))
                {
                    enemy.targetPoint = enemy.attackList[i];
                }
            }
        }
        //目标物体（炸弹）可能被销毁，targetPoint就不存在了，
        //但attackList.Count!=0（玩家或者其他炸弹还在），所以并不会切换到Patrol状态（有待改进.........）
        if (enemy.attackList.Count == 1) //不要省略==1，偶尔有可能==0
        {
            enemy.targetPoint = enemy.attackList[0];
        }

        //先攻击 后移动
        if (enemy.targetPoint.CompareTag("Player"))
        {
            enemy.AttackAction();
        }else if (enemy.targetPoint.CompareTag("Bomb"))
        {
            enemy.SkillAction();
        }

        enemy.MoveToTarget();
    }
}
