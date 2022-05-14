using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rD;
    float horizontal, vertical; //dirX = Direction
    
    public float moveSpeed = 10f, jumpForce = 200f;

    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private LayerMask ladderLayerMask;
    [SerializeField] private LayerMask boxMask;
    private Animator anim;
    private BoxCollider2D boxCollider2D;
    public float raydistance;
    private bool Climbing = false;
    AudioSource footSfx;

    void Start()
    {
        anim = GetComponent<Animator>();
        rD = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        footSfx = GetComponent<AudioSource>();
    }  


    // Update is called once per frames
    void Update()
    {
        horizontal = SimpleInput.GetAxis("Horizontal");
        rD.velocity = new Vector2(horizontal * moveSpeed, rD.velocity.y);

        
       
            
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


    }

    void FixedUpdate()
    {
        rD.velocity = new Vector2(horizontal * moveSpeed, rD.velocity.y);
        JumpAnim();
        Climb();
        FootSfx();
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
        RaycastHit2D boxcastHit2d = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, .07f, platformLayerMask);
        return boxcastHit2d.collider != null;
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
            rD.constraints = RigidbodyConstraints2D.FreezeRotation; ;
        }

        if (Climbing == true)
        {
            rD.velocity = new Vector2(vertical, 17f);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Box") && horizontal != 0)
        {
            anim.SetBool("isPushing", true);
        }

        if (collision.gameObject.CompareTag("Death"))
        {
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        anim.SetBool("isPushing", false);
    }
    public void FootSfx()
    {
        horizontal = SimpleInput.GetAxis("Horizontal") * moveSpeed;
        rD.velocity = new Vector2(horizontal, rD.velocity.y);
    
        if(rD.velocity.x != 0) 

        {
            if (!footSfx.isPlaying)
            {
                footSfx.Play();
            }
            else
            {
                footSfx.Stop();
            }
        }
    }
}
