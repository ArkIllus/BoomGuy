using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyBaseState currentState;

    public Animator anim;
    public int animState; //在Update中实时与animator的"state"绑定

    [Header("Movement")]
    public float speed;

    public Transform pointA, pointB;
    public Transform targetPoint;

    public List<Transform> attackList = new List<Transform>();

    public PatrolState patrolState = new PatrolState();
    public AttackState attackState = new AttackState();

    public virtual void Init()
    {
        anim = GetComponent<Animator>();
    }

    //Awake()会优先于start()调用
    public void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        //移动到OnUpdate方法(实现在PatrolState等类中)中
        //SwitchPoint();

        TransitionToState(patrolState);
    }

    // Update is called once per frame
    void Update()
    {
        //移动到OnUpdate方法(实现在PatrolState等类中)中
        //if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.01)
        //{
        //    SwitchPoint();
        //}
        //MoveToTarget();
        anim.SetInteger("State", animState);
        currentState.OnUpdate(this);
    }

    public void TransitionToState(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public void MoveToTarget()
    {
        //使用MoveTowards移动
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
        FlipDirection();
    }

    public void AttackAction()//攻击玩家
    {

    }
    public virtual void SkillAction()//使用技能
    {

    }

    public void FlipDirection()
    {
        //使用rotation翻转
        if (transform.position.x < targetPoint.position.x)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public void SwitchPoint()
    {
        if (Mathf.Abs(pointA.position.x - transform.position.x) > Mathf.Abs(pointB.position.x - transform.position.x))
        {
            targetPoint = pointA;
        }
        else
        {
            targetPoint = pointB;
        }
    }

    //Trigger的自带方法
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!attackList.Contains(collision.transform)) //会不会出问题？
            attackList.Add(collision.transform);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        attackList.Remove(collision.transform);
    }
}
