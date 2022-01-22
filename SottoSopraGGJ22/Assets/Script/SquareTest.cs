using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SquareTest : MonoBehaviour
{
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public bool isGrounded;
    public float jumpForce;
    public float speed;
    Rigidbody2D rb;

    private PhotonView photonView;
    
    // Start is called before the first frame update
    void Start ()
    {
        rb = GetComponent <Rigidbody2D> ();
        groundCheck = GameObject.Find("Ground").transform;
        photonView = GetComponent<PhotonView>();
    }
 
    void Update () {
        if (photonView.IsMine)
        {
            if (Input.GetButtonDown ("Jump") && isGrounded) {
                rb.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGrounded = false;
            }
        }

    }
 
    void FixedUpdate ()
    {
        if (photonView.IsMine)
        {
            isGrounded = Physics2D.OverlapPoint(groundCheck.position, whatIsGround);
            float x = Input.GetAxis("Horizontal");
            Vector3 move = new Vector3(x * speed, rb.velocity.y, 0f);
            rb.velocity = move;
        }
    }
}
