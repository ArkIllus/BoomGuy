using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whale : Enemy, IDamageable
{
    public float scaleCoeff; //变大系数
    public float scaleChange = 1; //变大了多少倍

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

    //吞噬炸弹
    public void Swallow() //Animation Event
    {
        //TODO:判断炸弹是否存在
        if (targetPoint.GetComponent<Bomb>())
        {
            targetPoint.GetComponent<Bomb>().TurnOff();
            targetPoint.gameObject.SetActive(false); //没有Destroy
        }
        //targetPoint.GetComponent<Bomb>()?.TurnOff();
        //targetPoint.gameObject.SetActive(false); //没有Destroy

        //Sprite Setting及所有子物体都会放大
        if (scaleChange < 3)
        {
            transform.localScale *= scaleCoeff;
            scaleChange *= scaleCoeff;
        }
        //TODO:超过3倍，Whale死亡,并吐出熄灭状态的炸弹
    }

}
