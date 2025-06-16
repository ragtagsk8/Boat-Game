using UnityEngine;

public class Boat : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private SpriteRenderer sprite;

    //MOVEMENT
    private bool anchored;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Anchor();
        if (anchored == false)
        {
            Movement();
        }
    }

    private void Movement()
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
