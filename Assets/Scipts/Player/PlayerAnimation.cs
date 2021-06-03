using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerController controller;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("x_speed", Mathf.Abs(rb.velocity.x)); //目前设置的发生转换的速度为0.1
        anim.SetFloat("y_speed", rb.velocity.y); //目前设置的发生转换的速度为0
        anim.SetBool("jump", controller.isJump);
        anim.SetBool("ground", controller.isGround);
    }
}
