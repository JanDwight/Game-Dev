using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D rD;
    float dirX; //dirX = Direction
    public float moveSpeed = 10f, jumpForce = 700f;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rD = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frames
    void Update()
    {
        dirX = SimpleInput.GetAxis("Horizontal") ;
        rD.velocity = new Vector2(dirX * moveSpeed, rD.velocity.y);

        if (SimpleInput.GetButtonDown("Jump"))
            DoJump();

        if(dirX == 0)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
        }

        if (SimpleInput.GetAxis("Horizontal") < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (SimpleInput.GetAxis("Horizontal") > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void FixedUpdate()
    {
        rD.velocity = new Vector2(dirX * moveSpeed, rD.velocity.y);
    }
    public void DoJump()
    {
            rD.AddForce(transform.up * jumpForce);
    }
}
