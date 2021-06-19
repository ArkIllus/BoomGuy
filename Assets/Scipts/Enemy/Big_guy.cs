using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Big_guy : Enemy, IDamageable
{
    public Transform pickupPoint;
    public float throwForce;

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

    public void PickUpBomb() //Animation Event
    {
        if (targetPoint.CompareTag("Bomb") && !hasBomb) //目标点是炸弹（通过tag判断） 且 未持有炸弹
        {
            targetPoint.gameObject.transform.position = pickupPoint.position; //要先targetPoint.gameObject获得炸弹这个物体(？)
            //targetPoint.position = pickupPoint.position; //不targetPoint.gameObject好像也行

            targetPoint.SetParent(pickupPoint); //使该炸弹（Transform？）成为pickupPoint的子集，跟随pickupPoint移动

            targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; //改成运动学模型（不会因有重力掉落）

            hasBomb = true;
        }
    }

    public void ThrowAway() //Animation Event 扔向玩家
    {
        //有可能炸弹爆炸/被Cucumber熄灭
        if (hasBomb)
        {
            targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; //改回动力学模型（会因有重力掉落）
            targetPoint.SetParent(transform.parent.parent); //炸弹不再跟随pickupPoint移动 (这里返回到BigGuy层级，也可以再返回上一级)

            // FindObjectOfType 找到挂载PlayerController类的物体
            if (FindObjectOfType<PlayerController>().gameObject.transform.position.x - transform.position.x < 0)
            {
                //从Transform获得炸弹的刚体
                targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * throwForce, ForceMode2D.Impulse);
            }
            else
            {
                targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1) * throwForce, ForceMode2D.Impulse);
            }
        }
        hasBomb = false;
    }
}
