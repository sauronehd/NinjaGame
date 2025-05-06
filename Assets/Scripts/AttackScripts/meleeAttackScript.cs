using UnityEngine;

public class meleeAttackScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string tagetName;
    public Transform playerTransform;
    public bool consumed = false;
    public float forceMagnitudeBase;
    public float damage;
    public float stunTime;
    //Stun time not in use - must add another state into player(StunLockState. Will then set a timer within that state to the stunTime value. 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    playerTransform.position.x - collision.gameObject.transform.position.x,
                    playerTransform.position.y - collision.gameObject.transform.position.y
                ).normalized;

                float xdirection = direction.x*-1;
                float ydirection = direction.y*-1;


                Vector2 forceDirection = new Vector2(xdirection, ydirection); // Adjust the direction as needed
                float forceMagnitude = forceMagnitudeBase*((targetScript.damage*0.1f)+0.8f); // Adjust the force magnitude as needed
                targetRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
                targetScript.damage += damage; // Increase damage by 0.2
                print("added force to player 2");
                consumed = true; // Mark as consumed
                
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
