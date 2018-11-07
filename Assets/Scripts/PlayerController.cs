using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera mainCam;
    public float speed = 10.0f;
    public Rigidbody rb;

    void Start(){
        Time.timeScale = 1.0f;
    }
    void Update()
    {        
        Vector3 newPos;
       
        var verticalInput = Input.GetAxis("Vertical")  * speed * Time.deltaTime;
     
        if(Mathf.Abs(verticalInput) > 0)
        {
           
            newPos = mainCam.transform.forward * verticalInput; 
            newPos.y = 0;          
            transform.position += newPos;
            //rb.MovePosition(transform.position + newPos);
           
        }

        var horizontalInput = Input.GetAxis("Horizontal")  * speed * Time.deltaTime;
        if (Mathf.Abs(horizontalInput) > 0)
        {
          
            newPos = mainCam.transform.right * horizontalInput;
            newPos.y = 0;     
            transform.position += newPos;       
            //rb.MovePosition(transform.position + newPos);
            
        }
            
                
    }

   
}