using UnityEngine;

public class CamerWillWorkPls : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Transform playerTransform; // Reference to the player's transform
    public Vector2 offset; // Offset from the player position
    public float smoothSpeed = 0.04f; // Speed of the camera movement
    private Vector3 velocity = Vector3.zero; // Used for smooth damping 
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(playerTransform==null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform; 
        }

        Vector3 diseredPosition = new Vector3(playerTransform.position.x + offset.x, playerTransform.position.y + offset.y, transform.position.z);
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, diseredPosition, ref velocity, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
