using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Animator anim;
    private Collider2D coll;
    private Rigidbody2D rb;

    public float startTime;
    public float waitTime;
    public float bombForce;
    public float damage = 3;

    [Header("Check")]
    public float radius;
    public LayerMask targetLayer;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //当前Base Layer的动画的状态
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("bomb_off"))
        {
            if (Time.time > startTime + waitTime)
            {
                anim.Play("bomb_explotion");
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void Explotion() //Animation Event
    {
        //能够发生碰撞的gameobeject一定有collider和rigidbody

        coll.enabled = false; //相当于把Collider2D取消勾选，防止炸弹爆炸时自己也会上天
        //一旦碰撞体消失，物体会自然下落，掉出屏幕，可以在下面更改刚体的重力系数

        Collider2D[] aroundObjects = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        //targetLayer（检测哪些Layer会被爆炸影响）在unity中设置

        rb.gravityScale = 0; //更改刚体的重力系数，防止炸弹掉出屏幕

        foreach (var item in aroundObjects)
        {
            Vector3 pos = transform.position - item.transform.position;

            item.GetComponent<Rigidbody2D>().AddForce((-pos + Vector3.up) * bombForce , ForceMode2D.Impulse); //Vector3.up=(0,1,0) ???

            //重新点燃熄灭的炸弹
            if (item.CompareTag("Bomb") && item.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("bomb_off"))
            {
                item.GetComponent<Bomb>().TurnOn();
            }

            if (item.CompareTag("Player") || item.CompareTag("Enemy"))
                //GetComponent同样可以获得（该组件挂载的对象的类？实现的）接口
                item.GetComponent<IDamageable>().GetHit(damage);

        }
    }

    public void TurnOff()
    {
        anim.Play("bomb_off");
        gameObject.layer = LayerMask.NameToLayer("NPC"); //改变该炸弹的Layer，这样敌人的CheckArea就不会检测到炸弹了
    }

    public void TurnOn()
    {
        startTime = Time.time; //重新开始计时
        anim.Play("bomb_on");
        gameObject.layer = LayerMask.NameToLayer("Bomb"); //改回炸弹的Layer
    }

    public void DestroyThis() //Animation Event
    {
        Destroy(gameObject);
    }

}
