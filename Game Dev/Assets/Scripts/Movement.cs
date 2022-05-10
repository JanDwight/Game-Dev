using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    Rigidbody2D rD;
    float horizontal, vertical; //dirX = Direction
    
    public float moveSpeed = 10f, jumpForce = 200f;
    
    public float distance = 1f;
   public LayerMask boxMask;
   [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private LayerMask ladderLayerMask;
    private Animator anim;
    private BoxCollider2D boxCollider2D;
    public float raydistance;
    private bool isClimbing = false;
    GameObject box;
    
    

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rD = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();

    }

    // Update is called once per frames
    void Update()
    {
        horizontal = SimpleInput.GetAxis("Horizontal");
        rD.velocity = new Vector2(horizontal * moveSpeed, rD.velocity.y);

        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, distance, boxMask);


        // For Running Animation Que

        if(horizontal == 0)

        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
        }

        // Where do the Character face, Left or Right?
        if (SimpleInput.GetAxis("Horizontal") < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (SimpleInput.GetAxis("Horizontal") > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        
        if (hit.collider != null && hit.collider.gameObject.tag == "pushable" && SimpleInput.GetAxis("Horizontal"))
        {
            box = hit.collider.gameObject;
            box.GetComponent<FixedJoint2D>().enabled = true;
            box.GetComponent<boxpull>().beingPushed = true;
            box.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
        }
    }

    void FixedUpdate()
    {
        rD.velocity = new Vector2(horizontal * moveSpeed, rD.velocity.y);
        JumpAnim();
        Climb();
    }

    public void DoJump()
    {
        if (IsGrounded())
        {
            rD.velocity = Vector2.up * jumpForce;
        }     
    }

    public bool IsGrounded()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, .07f, platformLayerMask);
        return raycastHit2d.collider != null;
    }

    private void JumpAnim()
    {
        float vertical = SimpleInput.GetAxis("Vertical");
        if (!IsGrounded())
        {
            anim.SetBool("isJumping", true);
        }
        else
        {
            anim.SetBool("isJumping", false);
        }
    }

    public void Climb()
    {
        vertical = SimpleInput.GetAxis("Vertical");

        RaycastHit2D hitinfo = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.up, .07f, ladderLayerMask);

        if (hitinfo.collider != null)
        {
            if (vertical > .95)
            {
                Climbing = true;
            }
        }
        else
        {
            Climbing = false;
            anim.SetBool("isClimbing", false);
            rD.constraints = RigidbodyConstraints2D.None;
        }

        if (Climbing == true)
        {
            rD.velocity = new Vector2(vertical * 1f, rD.position.x);
            Debug.Log(vertical);
            rD.constraints = RigidbodyConstraints2D.FreezePositionX;
            rD.gravityScale = 80;
            anim.SetBool("isClimbing", true);
            anim.SetBool("isJumping", false);
        }
        else
        {
            rD.gravityScale = 2;
        }

    }



}
