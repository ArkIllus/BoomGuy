using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed;
    public float jumpForce; // (或者叫jumpSpeed)

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
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    //物理相关的update
    public void FixedUpdate()
    {
        PhysicsCheck();
        Movement();
        Jump();
    }

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
        //float horizontalInput = Input.GetAxis("Horizontal"); // -1 ~ 1 (包含小数)
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // -1 ~ 1 (-1,0,1)

        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        //转向的3种方式
        //1.修改Rotation 2.修改Scale 3.修改Sprite Renderer(不推荐)
        //这里采用第2种
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(horizontalInput, 1, 1);
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
}
