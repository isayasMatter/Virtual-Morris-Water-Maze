using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
public class Movement : MonoBehaviour {
	public SteamVR_Action_Vector2 movementAction;	

	public SteamVR_ActionSet actionSet;
	public SteamVR_Action_Vector2 a_move;
	private SteamVR_Input_Sources hand;
	public int speed = 10;

	public GameObject mainPlayer;

	private Vector3 newMovement;

	private Rigidbody rb;
	private Interactable interactable;
	// Use this for initialization
	void Start () {
		rb = mainPlayer.GetComponent<Rigidbody>();
		interactable = GetComponent<Interactable>();
		interactable.activateActionSetOnAttach = actionSet;
	}
	

	// void OnMovementAction(SteamVR_Action_In actionIn){
	// 	Vector2 newAxis = movementAction.GetAxis(hand.handType);
	// 	//Debug.Log(newAxis.ToString());
	// 	if (newAxis.x > 0.2 && newAxis.y > 0.2){
	// 		Vector3 newPos = new Vector3(newAxis.x, 0.0f, newAxis.y);

	// 		rb.AddForce(newPos * speed);
	// 	}
		
	// }
	
	// Update is called once per frame
	void Update () {
		if (interactable.attachedToHand)
            {
                hand = interactable.attachedToHand.handType;
                Vector2 m = a_move.GetAxis(hand);
                newMovement = new Vector3(m.x, 0, m.y);                
            }
            else
            {
                newMovement = Vector2.zero; 
            }

           // float rot = transform.eulerAngles.y;

			Debug.Log(newMovement.ToString());

            //newMovement = Quaternion.AngleAxis(rot, Vector3.up) * newMovement;
            

           rb.AddForce(newMovement * speed);
        }
		
	
}
