using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator aim;

    float horizontalInput;
    public float movespeed = 5f;
    bool isFacingRight = false;
    public float jump = 5f;
    public bool isGrounded = false;
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        FlipSpriter();
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jump);
            isGrounded = false;
            aim.SetBool("isJumping", !isGrounded);
        }
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * movespeed, rb.velocity.y);
        aim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        aim.SetFloat("yVelocity", rb.velocity.y);
    }
    void FlipSpriter()
    {
        if(isFacingRight&& horizontalInput<0f||!isFacingRight&&horizontalInput>0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 Is = transform.localScale;
            Is.x *= -1f;
            transform.localScale = Is;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
        aim.SetBool("isJumping", !isGrounded);
    }
}
