using UnityEngine;

public class ninjaStarScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 originCoords;
    
    void Start()
    {
        originCoords = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(originCoords.x, transform.position.y+(-5*Time.deltaTime), originCoords.z);
    }
}
