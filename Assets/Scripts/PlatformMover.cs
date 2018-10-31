using UnityEngine;

public class PlatformMover : MonoBehaviour{
    public int speed = 10;

    public delegate void PositionSelected();
    public static event PositionSelected OnPositionSelected;

    

    void Start(){
       
    }
   
    void Update()
    {
            
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            if (OnPositionSelected != null)
            {                
                OnPositionSelected();
            }
        }
    }

    void FixedUpdate(){
        Vector3 newPos;
        var verticalInput = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        if(Mathf.Abs(verticalInput) > 0)
        {
            newPos = transform.forward * verticalInput;
            newPos.y = 0;
            transform.position += newPos;
            
        }

        var horizontalInput = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        if (Mathf.Abs(horizontalInput) > 0)
        {
            newPos = transform.right * horizontalInput;
            newPos.y = 0;
            transform.position += newPos;
           
        }    

    }

    
}