using UnityEngine;

public class ninjaStarScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Vector3 originCoords;
    public bool consumed = false;
    public Transform myTransform;
    public string tagetName;
    void Start()
    {
        originCoords = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(originCoords.x, transform.position.y+(-5*Time.deltaTime), originCoords.z);
    }

        private void OnTriggerEnter2D(Collider2D collision)
    {
        if (consumed)
        {
            return; // Exit if already consumed
        }
        print("triggered");
        if (collision.gameObject.name == tagetName)
        {
            print("triggered on player 2");
            // Assuming the target has a Rigidbody2D component
            damageHealthLaunch targetScript = collision.gameObject.GetComponent<damageHealthLaunch>();
            Rigidbody2D targetRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
            if (targetRigidbody != null)
            {
                Vector2 direction = new Vector2(
                    myTransform.position.x - collision.gameObject.transform.position.x,
                    myTransform.position.y - collision.gameObject.transform.position.y
                ).normalized;

                float xdirection = direction.x*-1;
                float ydirection = direction.y*-1;


                Vector2 forceDirection = new Vector2(xdirection, ydirection); // Adjust the direction as needed
                float forceMagnitude = 50*((targetScript.damage*0.1f)+0.8f); // Adjust the force magnitude as needed
                targetRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
                targetScript.damage += 10f; // Increase damage by 0.2
                print("added force to player 2");
                consumed = true; // Mark as consumed
                Destroy(gameObject); // Destroy the ninja star after use
            }
            else
            {
                Debug.LogError("Rigidbody2D component not found on the target object.");
            }
        }
        else
        {
            print("Thre name is: " + collision.gameObject.name);
        }
    }
}
