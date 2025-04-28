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
        transform.position = new Vector3(originCoords.x, originCoords.y+(-2*Time.deltaTime), originCoords.z);
    }
}
