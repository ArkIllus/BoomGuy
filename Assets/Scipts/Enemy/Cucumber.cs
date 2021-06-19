using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cucumber : Enemy, IDamageable
{
    public void GetHit(float damage)
    {
        health -= damage;
        if (health < 1)
        {
            health = 0;
            isDead = true;
            //anim.SetBool("dead",isDead); //"dead"位置2：放在这里表示只有受到伤害才可能死亡 (Animator中dead初始化为false) 不用每帧都检测是否死亡
        }
        anim.SetTrigger("hit");
    }

    //public int health;
    //public Rigidbody2D rb;

    //public override void SkillAction()
    //{

    //}

    //不需要在这里获得Rigidbody2D，只是举个例子
    //public override void Init()
    //{
    //    base.Init();
    //    rb = GetComponent<Rigidbody2D>();
    //}

    public void SetOff() //Animation Event
    {
        //如果遇到吹灭的瞬间炸弹爆炸导致攻击目标丢失可以做一个非空的判断：
        //问号'?'的作用表示判断是否为空
        targetPoint.GetComponent<Bomb>()?.TurnOff();
    }
}
