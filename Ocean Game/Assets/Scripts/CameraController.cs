using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform boatTrans;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = new Vector3(boatTrans.position.x, boatTrans.position.y, -10);
    }
}
