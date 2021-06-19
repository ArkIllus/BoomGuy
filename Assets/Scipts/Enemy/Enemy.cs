using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour //IDamageable不在这里实现，在每种敌人里实现
{
    EnemyBaseState currentState;

    public Animator anim;
    public int animState; //在Update中实时与animator的"state"绑定
    private GameObject alarmSign;

    [Header("Base State")]
    public float health;
    public bool isDead; // 默认初始化为false
    public bool hasBomb; //是否持有炸弹
    public bool isBoss;

    [Header("Movement")]
    public float speed;
    public Transform pointA, pointB;
    public Transform targetPoint;

    [Header("Attack Settings")]
    public float nextAttack = 0; // 下次可以攻击的时间
    public float nextSkill = 0; // 下次可以使用技能的时间
    public float attackRate; // 普通攻击间隔
    public float skillRate; // 技能间隔
    public float attackRange, skillRange;

    public List<Transform> attackList = new List<Transform>();

    public PatrolState patrolState = new PatrolState();
    public AttackState attackState = new AttackState();

    //Init()写成虚方法
    public virtual void Init()
    {
        anim = GetComponent<Animator>();
        alarmSign = transform.GetChild(0).gameObject; //获得当前Transform下面的第0个子物体（即Alarm Sign）
    }

    //Awake：始终在任何 Start 函数之前并在实例化预制件之后调用此函数
    public void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        //移动到OnUpdate方法(实现在PatrolState等类中)中
        //SwitchPoint();

        GameManager.instance.AddEnemy(this);

        TransitionToState(patrolState);
        if (isBoss)
            UIManager.instance.SetBossHealthBar(health);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //移动到OnUpdate方法(实现在PatrolState等类中)中
        //if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.01)
        //{
        //    SwitchPoint();
        //}
        //MoveToTarget();

        anim.SetBool("dead", isDead); //"dead"位置1：放在这里表示每帧都会检测是否死亡
        //另外 注意敌人死亡时，会被往上炸飞，如果有x轴初速度，还会横向飞（有减速）
        if (isDead)
        {
            if (isBoss)
                UIManager.instance.UpdateBossHealthBar(health);
            GameManager.instance.EnemyDead(this);
            return;
        }

        anim.SetInteger("state", animState);
        currentState.OnUpdate(this);

        if (isBoss)
            UIManager.instance.UpdateBossHealthBar(health); //也可以在Enemy中实现IDamageble接口,再在GetHit()中更新boss的血条
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
        //注意，不是用x坐标判断，而是二维坐标距离判断
        if (Vector2.Distance(transform.position, targetPoint.position) < attackRange)
        {
            if (Time.time > nextAttack)
            {
                //播放攻击动画
                anim.SetTrigger("attack");
                Debug.Log("对玩家普通攻击");
                nextAttack = Time.time + attackRate;
            }
        }
    }
    public virtual void SkillAction()//（对炸弹）使用技能
    {
        if (Vector2.Distance(transform.position, targetPoint.position) < skillRange)
        {
            if (Time.time > nextSkill)
            {
                //播放技能动画
                anim.SetTrigger("skill");
                Debug.Log("对炸弹使用技能");
                nextSkill = Time.time + skillRate;
            }
        }
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

    //Trigger的自带方法 （CheckArea是一个Trigger）
    public void OnTriggerStay2D(Collider2D collision)
    {
        //持有炸弹或敌人死亡或玩家死亡（游戏结束）时，不添加attackList（TODO:搞清楚如果玩家死亡，为什么attackList会清零？？？）
        if (!attackList.Contains(collision.transform) && !hasBomb && !isDead && !GameManager.instance.gameOver) //transform而不是gameobject会不会出问题？好像不会
            attackList.Add(collision.transform);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        attackList.Remove(collision.transform);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //启动协程 
        //敌人死亡或玩家死亡（游戏结束）后不会继续发现玩家/炸弹，出现“！”标志
        if (!isDead && !GameManager.instance.gameOver)
            StartCoroutine(OnAlarm());
        //或 
        //StartCoroutine("OnAlarm");
    }

    //协程 （协程看上去也是子程序，但执行过程中，在子程序内部可中断，然后转而执行别的子程序，在适当的时候再返回来接着执行）
    IEnumerator OnAlarm()
    {
        alarmSign.SetActive(true); //激活GameObject
        //等待Alarm Sign的Animator的show动画片段(clip)的时长 （位于BaseLayer 第0个动画 的片段(clip)）
        yield return new WaitForSeconds(alarmSign.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length); 
        alarmSign.SetActive(false);
    }
}
