using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    Animator anim;
    BoxCollider2D coll;
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();

        GameManager.instance.GetDoorExit(this);

        //碰撞器使用enabled而没有游戏物体的setActive
        coll.enabled = false;
    }

    public void OpenDoor() //在GameManager中调用，当所有敌人都被消灭时打开门
    {
        anim.Play("open");
        coll.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //写到GameManager里
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //进入下一个场景
            GameManager.instance.NextLevel();
        }
    }
}
