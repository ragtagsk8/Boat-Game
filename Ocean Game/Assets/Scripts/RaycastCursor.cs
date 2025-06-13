using UnityEngine;

public class RaycastCursor : MonoBehaviour
{
    //private Vector2 mouseTrans;

    [SerializeField] public GameObject cursorBoatObject;
    [SerializeField] public CircleCollider2D cursorColl;

    void Start()
    {
        //cursorColl = GetComponent<CircleCollider2D>();
        Physics2D.IgnoreCollision(cursorBoatObject.GetComponent<CapsuleCollider2D>(), cursorColl);
    }

    void Update()
    {
        transform.position = new Vector3(Input.mousePosition.x,Input.mousePosition.y,-5);
    }

    private void CapsuleCast()
    {
        //Physics2D.CapsuleCast(transform.position, cursorColl*2, );
    }
}
