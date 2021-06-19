using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bald_pirate : Enemy, IDamageable
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
}
