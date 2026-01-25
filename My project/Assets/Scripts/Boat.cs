using UnityEngine;
using UnityEngine.UIElements;

public class Boat : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private SpriteRenderer sprite;
    private Transform trans;
    private Animator anim;

    //MOVEMENT
    private bool anchored;
    private bool attemptingMovement;
    private float dirX = 0f;
    private float dirY = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        trans = GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Anchor();
        if (anchored == false)
        {
            Movement();
        }
        Animation();
    }

    private void Movement()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        dirY = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(dirX, dirY);
        rotatePlayer();
    }

    private void rotatePlayer() {
        if (dirY > 0f && dirX > 0f) {
            trans.rotation = Quaternion.Euler(0, 0, 315);
        } else if (dirY > 0f && dirX < 0f) {
            trans.rotation = Quaternion.Euler(0, 0, 45);
        } else if (dirY < 0f && dirX > 0f) {
            trans.rotation = Quaternion.Euler(0, 0, 225);
        } else if (dirY < 0f && dirX < 0f) {
            trans.rotation = Quaternion.Euler(0, 0, 135);
        } else if (dirX < 0f) {
            trans.rotation = Quaternion.Euler(0, 0, 90);
        } else if (dirY < 0f) {
            trans.rotation = Quaternion.Euler(0, 0, 180);
        } else if (dirX > 0f) {
            trans.rotation = Quaternion.Euler(0, 0, 270);
        } else if (dirY > 0f) {
            trans.rotation = Quaternion.Euler(0, 0, 0);
        } 
    }

    private void Animation() {
        if (dirX != 0f || dirY != 0f) {
            anim.SetInteger(("state"), 1);
        } else {
            anim.SetInteger(("state"), 0);
        }
    }

    private void AttemptingMovement()
    {

    }

    private void Anchor()
    {
        if (Input.GetKeyDown("a") && anchored == false)
        {
            anchored = true;
        }
        if (Input.GetKeyDown("a") && anchored == true)
        {
            anchored = false;
        }
    }
}
