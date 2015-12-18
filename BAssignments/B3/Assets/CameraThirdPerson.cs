using UnityEngine;
using System.Collections;

public class CameraThirdPerson : MonoBehaviour {
	public GameObject player;
	private Vector3 offset;
	// Use this for initialization
	void Start () {
		offset = player.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate()
	{
		Vector3 newPosition;
		float angle = player.transform.eulerAngles.y;
		Quaternion rotation = Quaternion.Euler(0, angle, 0);
		newPosition = player.transform.position - (rotation * offset);
		//transform.LookAt (player.transform);
		StartCoroutine (TransitionCamera (newPosition));

	}

	IEnumerator TransitionCamera(Vector3 EndPosition)
	{
		float TransitionTime = 1f;
		float t = 0.0f;
		Vector3 StartingPos = transform.position;
		while (t < 1.0f)
		{
			t += Time.deltaTime * (Time.timeScale/TransitionTime);
			
			
			transform.position = Vector3.Lerp(StartingPos, EndPosition, t);
			transform.LookAt (player.transform);
			yield return 0;
		}
		
	}
}
