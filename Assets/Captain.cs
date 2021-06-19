using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captain : Enemy, IDamageable
{
    private SpriteRenderer sprite;
    public int escapeSpeedCoeff = 2;

    public override void Init()
    {
        base.Init();
        sprite = GetComponent<SpriteRenderer>();
    }

    public override void Update()
    {
        base.Update();

        //如果逃跑动画期间炸弹爆炸了，没来得及取消翻转，进入PatrolState（会使animState=0），
        //炸弹爆炸时间未知，所以需要在Update中实时判断和更新翻转状态【1】
        if (animState == 0) sprite.flipX = false;
    }

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

    public override void SkillAction()
    {
        base.SkillAction();

        //朝炸弹相反的方向逃跑，移动速度为正常移动速度的2倍，【逃跑的持续时间为skill动画的播放时间】
        if (anim.GetCurrentAnimatorStateInfo(1).IsName("skill"))
        {
            //使用Sprite Renderer的Flip翻转
            sprite.flipX = true;
            //TODO：如果Player和Bomb在Captain两边，还是有可能会倒退跑步
            if (transform.position.x > targetPoint.position.x)
            {
                //使用Vector2.MoveTowards实现向某个方向移动
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.right, speed * escapeSpeedCoeff * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.left, speed * escapeSpeedCoeff * Time.deltaTime);
            }
        }
        else
        {
            //还有一种可能，逃跑动画期间炸弹爆炸了，没来得及取消翻转，进入PatrolState，
            //炸弹爆炸时间未知，所以需要在Update中实时判断和更新翻转状态【1】
            sprite.flipX = false;
        }
    }
}
