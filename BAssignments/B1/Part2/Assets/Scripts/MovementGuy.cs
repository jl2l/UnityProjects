using UnityEngine;
using System.Collections;

public class MovementGuy : MonoBehaviour {
	
	Transform mainCamera;		
	Animator animController;	
	public float speedIncrement;     
	public float currentSpeed;     
	int jumpHash = Animator.StringToHash("Jump");
	int notjumpHash = Animator.StringToHash ("NotJump");

	// Use this for initialization
	void Start () {
		mainCamera = Camera.main.transform;		
		animController = GetComponent<Animator>();	
		currentSpeed = 0.0f;
		speedIncrement = 0.08f;
	}
	
	// Update is called once per frame
	void Update ()
	{

		float verticalAxis = Input.GetAxis("Vertical");
		float horizontalAxis = Input.GetAxis("Horizontal");

		if(Input.GetKeyDown(KeyCode.Space))
		{
			animController.SetTrigger (jumpHash);
			StartCoroutine(StopJumping(3.0f));
			return;
		}
				
	
		Vector3 cameraForward = mainCamera.TransformDirection(Vector3.forward);
		cameraForward.y = 0;	
		
	
		Vector3 cameraRight = mainCamera.TransformDirection(Vector3.right);
		
	
		Vector3 targetDirection = horizontalAxis * cameraRight + verticalAxis * cameraForward;
		

		Vector3 lookDirection = targetDirection.normalized;
		

		if (lookDirection != Vector3.zero)
			transform.rotation = Quaternion.LookRotation(lookDirection);

	
			if (verticalAxis != 0 || horizontalAxis != 0)
		{

			if (currentSpeed < 2)
			{
				currentSpeed += speedIncrement;
			}

			if(Input.GetKey(KeyCode.LeftShift) && currentSpeed < 4)
			{
				currentSpeed += speedIncrement;
				Debug.Log ("I'm in");
			}
		}

		else //default to idle if no input
		{

			if (currentSpeed > 0)
				currentSpeed -= speedIncrement; 
		}


		animController.SetFloat("MoveSpeed", currentSpeed);
		



	}

	IEnumerator StopJumping (float DelayInSeconds)
	{

		yield return new WaitForSeconds (DelayInSeconds);
		animController.SetTrigger (notjumpHash);


		
		
	}




}