using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;
    private Animator anim;
    private FixedJoystick joystick;

    public float speed;
    public float jumpForce; // (或者叫jumpSpeed)

    [Header("Player State")]
    public float health;
    public bool isDead; // 默认初始化为false
    //public bool isHurt;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask groundLayer;

    [Header("States Check")]
    public bool isGround; //（近似）在地面上
    public bool willJump; // 将要起跳 （默认初始化为false）
    public bool isJump; // 处于跳跃状态（包括跳跃+后续的下落）（默认初始化为false）

    [Header("Jump FX")]
    public GameObject jumpFX;
    public GameObject landFX;

    [Header("Attack Settings")]
    public GameObject bombPrefab;
    public float nextAttack = 0; // 下次可以攻击的时间
    public float attackRate; // 攻击间隔

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        joystick = FindObjectOfType<FixedJoystick>();

        GameManager.instance.GetPlayer(this);

        //每次进入新的scene，加载血量
        health = GameManager.instance.LoadHealth();
        UIManager.instance.UpdateHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("dead", isDead); //"dead"位置1：放在这里表示每帧都会检测是否死亡
        //TODO:为什么不会立即播放死亡动画？
        //死亡时不检测输入
        if (isDead) return; 

        //用来判断是否正在播放受伤动画
        //isHurt = anim.GetCurrentAnimatorStateInfo(1).IsName("player_hit");
        CheckInput();
    }

    //物理相关的update
    public void FixedUpdate()
    {
        //死亡时速度立刻归零，只会被往上炸飞一下。
        //不然如果有x轴初速度，还会横向一直匀速飞（直到撞墙）
        //匀速是∵玩家Collider的Material的摩擦力Friction=0
        //且不再进行物理相关的update
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        PhysicsCheck();
        
        //非受伤状态才可以移动和跳跃
        //if (!isHurt)
        //{
        //    Movement();//input会覆盖Rigidbody的速度，所以用isHurt来锁定就可以让 Player 被击飞了
        //    Jump();
        //}
        Movement();
        Jump();
    }

    //PC端：键盘输入
    void CheckInput()
    {
        if(Input.GetButtonDown("Jump") && isGround)
        {
            willJump = true;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack();
        }
    }
    void Movement()
    {
        //PC端：键盘操作
        //float horizontalInput = Input.GetAxis("Horizontal"); // -1 ~ 1 (包含小数)
        //float horizontalInput = Input.GetAxisRaw("Horizontal"); // -1 ~ 1 (-1,0,1)

        //移动端：操纵杆
        //TODO：现在变成-1 ~ 1 (包含小数)了,可以变速移动了
        float horizontalInput = joystick.Horizontal;

        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        //转向的4种方式
        //1.修改Rotation 2.修改localScale 3.修改Sprite Renderer(不推荐) //4.修改eulerAngles
        //这里采用第2种 修改localScale
        //if (horizontalInput != 0)
        //{
        //    transform.localScale = new Vector3(horizontalInput, 1, 1);
        //}

        //4.修改eulerAngles
        if (horizontalInput > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if(horizontalInput < 0) //避免=0的时候也翻转过去
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

    }

    //void Jump()
    //{
    //    if (willJump)
    //    {
    //        rb.gravityScale = 4; //为了跳跃过程更快
    //        rb.velocity = new Vector2(rb.velocity.x, jumpForce); //起跳后，在空中左右移动速度不变 
    //        willJump = false;
    //    }
    //}

    //void PhysicsCheck()
    //{
    //    isGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    //    if (isGround)
    //    {
    //        rb.gravityScale = 1; 
    //    }
    //}
    //这样写有隐患，如果checkRadius比较大，
    //有可能在调用jump的下一帧(0.02s)仍然判断为isGround=true，
    //重力又重置为1了，就能跳很高

    //改进：只要在空中（isGround=false），重力恒定为4
    //刚起跳的几帧，如果贴近地面（isGround=true），重力仍然为1
    void Jump()
    {
        if (willJump)
        {
            //rb.gravityScale = 4;
            isJump = true;

            jumpFX.SetActive(true);
            jumpFX.transform.position = transform.position + new Vector3(0.05f, -0.45f, 0); //注意需要保持FX位置与Player一致 //float需要在数字后面写f

            rb.velocity = new Vector2(rb.velocity.x, jumpForce); //当前处理方式：起跳后，在空中左右移动速度时刻与键盘输入一致 
                                                                 //处理方式二：起跳后，在空中左右移动速度保持起跳前一刻的速度 
            willJump = false;
        }
    }

    public void ButtonJump()
    {
        if(isGround) willJump = true;
    }

    public void Attack()
    {
        if (Time.time > nextAttack)
        {
            Instantiate(bombPrefab, transform.position, bombPrefab.transform.rotation);
            nextAttack = Time.time + attackRate;
        }
    }

    void PhysicsCheck()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer); //true即检测到的collider2d不为空
        if (isGround)
        {
            rb.gravityScale = 1;
            isJump = false;
        }
        else
        {
            rb.gravityScale = 4; //为了跳跃过程更快
        }
    }

    public void LandFX() //(在player_ground动画中的)Animation Event (不能用isGround去setActive LandFX)
    {
        landFX.SetActive(true);
        landFX.transform.position = transform.position + new Vector3(0.05f, -0.75f, 0); //注意需要保持FX位置与Player一致
    }

    //unity内置方法 (无需在任何Update或FixeduUpdate中调用)
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }

    public void GetHit(float damage)
    {
        //TODO：考虑去掉受伤的短暂无敌
        //TODO: Q:多个敌人不断攻击玩家，可能会频繁触发hit，玩家在HitState动画状态 触发hit 并且 死亡isDead=true ，这时HitState会播放哪一个动画呢？
        //A:由于以下代码的顺序，会先isDead，再触发hit，而isDead与dead在Update中每帧绑定，所以应该会先死亡？？？

        //受伤动画播放期间短暂无敌，防止该过程中因受到多次伤害被秒杀
        if (!anim.GetCurrentAnimatorStateInfo(1).IsName("player_hit"))
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

        UIManager.instance.UpdateHealth(health);
    }
}
