using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player_trans;
    private Transform camera_trans;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera_trans = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        camera_trans.position = new Vector3(player_trans.position.x, player_trans.position.y, -10f);
    }
}
