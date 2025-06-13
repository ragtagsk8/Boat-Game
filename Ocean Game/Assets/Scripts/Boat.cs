using UnityEngine;

public class Boat : MonoBehaviour
{
    //UNITY COMPONENTS
    private Rigidbody2D rb;
    private SpriteRenderer rbSprite;
    private CapsuleCollider2D coll;
    private Camera cam;

    //MOVEMENT
    private float dirX;
    private float dirY;
    private bool anchored;
    private bool attemptingMovement;


    //LEVELS
    private int hullLevel;
    private int arrowsLevel;
    private int cannonsLevel;
    private int mastLevel;
    private int brigLevel;
    //STATS
    private int boatHealth;
    [SerializeField] private int boatSpeed;

    //RESOURCES
    private int wood;
    private int gold;
    private int food;
    private int metal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rbSprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        //hullLevel = 1;
        //arrowsLevel = 1;
        //cannonsLevel = 1;
        //mastLevel = 1;
        //brigLevel = 1;

        anchored = false;
        attemptingMovement = true;
        //may need to make this false later
    }

    void Update()
    {
        //transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        Anchor();
        RaycastMouse();

        if (anchored == false && attemptingMovement == true)
        {
            Movement();
        }
    }

    private void Movement()
    {
        transform.position = Vector2.MoveTowards(transform.position, WorldPositionMouse(), boatSpeed * Time.deltaTime);
        float angle = Mathf.Atan2(WorldPositionMouse().y - transform.position.y, WorldPositionMouse().x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private Vector2 WorldPositionMouse()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }
    
    private void Anchor()
    {
        if (Input.GetKeyDown("a") && anchored == false)
        {
            anchored = true;
            //trigger animation
        }
        if (Input.GetKeyDown("a") && anchored == true)
        {
            anchored = false;
        }

        if (anchored == true)
        {
            //contraints
            //maybe disable gravity if need be when I introduce wind
        }
    }

    private void RaycastMouse()
    {
        
    }
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null)
        {
            Debug.Log("Mouse nearby");
            attemptingMovement = false;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Mouse not nearby");
        attemptingMovement = true;
    }
    */
}
