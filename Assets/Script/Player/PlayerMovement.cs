using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float speed = 10f;
    private float turnSpeed = 100f;
    public bool IsOutSide;
    public static PlayerMovement Instance;
    public void Awake()
    {
        IsOutSide = false;
        if (Instance != null) 
        {
            Destroy(Instance);
        }
        Instance = this;
    }
    public void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(0, horizontalInput * turnSpeed * Time.deltaTime, 0);
    }
}
