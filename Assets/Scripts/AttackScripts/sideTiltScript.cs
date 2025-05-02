using UnityEngine;

public class sideTiltScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string tagetName;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
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
                    transform.position.x - collision.gameObject.transform.position.x,
                    transform.position.y - collision.gameObject.transform.position.y
                ).normalized;

                float xdirection = direction.x-1;
                float ydirection = direction.y-1;


                Vector2 forceDirection = new Vector2(xdirection, ydirection); // Adjust the direction as needed
                float forceMagnitude = 50*(targetScript.damage+0.2f); // Adjust the force magnitude as needed
                targetRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
                targetScript.damage += 10f; // Increase damage by 0.2
                print("added force to player 2");
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
