using UnityEngine;
using System.Collections;

public class MoveObstacle : MonoBehaviour {
	public bool selected;
	private Renderer rend;
	private float scalar = 39.0f;
	private Vector3 position;
	public float smoothTime = 1.0F;
	private Vector3 VelocityPos = new Vector3(1 ,1,1);
	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer>();
		position = new Vector3 (0, 0, 0);
		rend.material.SetColor("_Color", Color.red);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (selected)
		{
			position = transform.position;

			if(Input.GetKeyDown(KeyCode.UpArrow))
			{
				position.z = transform.position.z - scalar;
			}
			if(Input.GetKeyDown(KeyCode.DownArrow))
			{
				position.z = transform.position.z + scalar;
			}
			if(Input.GetKeyDown(KeyCode.LeftArrow))
			{
				position.x = transform.position.x + scalar;
			}
			if(Input.GetKeyDown(KeyCode.RightArrow))
			{
				position.x = transform.position.x - scalar;
			}

			transform.position = Vector3.SmoothDamp(transform.position, position,ref VelocityPos, smoothTime);
		}

	
	}

	void OnMouseOver()
	{
		//Debug.Log ("Mouse is here.");
		if (Input.GetMouseButtonDown (0)) {
			if(selected)
			{
			selected = false;
			rend.material.SetColor("_Color", Color.red);
			}
			else
			{
			selected = true;
			rend.material.SetColor("_Color", Color.yellow);
			}
			Debug.Log ("Selected");

		}
	}
}
