using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public GameObject[] agents;
	private Vector3 sum;
	private Vector3 position;
	private Vector3 CameraPosition;
	private float differenceMag;
	// Use this for initialization
	void Start () {
		sum = new Vector3(0,0,0);
		differenceMag = 0;
	}

	// Update is called once per frame
	void LateUpdate () {
		Vector3 difference;
		sum = new Vector3 (0, 0, 0);
		for(int i=0; i<agents.Length; i++)
		{
			sum += agents[i].transform.position;
		}

		position = sum / (float)agents.Length;

		for (int i =0; i<agents.Length; i++)
		{
			difference = (position - agents[i].transform.position)/2.0f;
			differenceMag += difference.magnitude;
		}

		differenceMag = differenceMag / (float)agents.Length;
		CameraPosition = position;
		CameraPosition.z = CameraPosition.z + 15f;
		CameraPosition.y = 10f + 1.5f * (float)differenceMag;
		transform.position = CameraPosition;
		transform.LookAt (position);

	}
}
