using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    int dir;
    public bool bombAvailable; //能不能踢炸弹
    public float damage = 1;
    public float kickForce;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (transform.position.x > other.transform.position.x)
            dir = -1;
        else 
            dir = 1; 

        if (other.CompareTag("Player"))
        {
            //敌人发动攻击后，进入攻击状态动画，播放到某一帧Hit Point才被激活
            //此时玩家可能逃出HitPoint范围，这样玩家就不会受伤
            other.GetComponent<IDamageable>().GetHit(damage);
            Debug.Log("玩家受伤");
            if (bombAvailable)
            {
                //往45°斜上方踢
                other.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir, 1) * kickForce, ForceMode2D.Impulse);
            }
        }

        if (other.CompareTag("Bomb") && bombAvailable)
        {
            //往45°斜上方踢
            other.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir, 1) * kickForce, ForceMode2D.Impulse);
            Debug.Log("炸弹被攻击踢飞");
        }
    }
}
