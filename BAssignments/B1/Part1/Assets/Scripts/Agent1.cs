using UnityEngine;
using System.Collections;

public class Agent1 : MonoBehaviour {
	public NavMeshAgent navMeshAgent;
	public bool selected;
	private Renderer rend;

	// Use this for initialization
	void Start () {
		navMeshAgent = GetComponent<NavMeshAgent>();
		selected = false;
		rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Input.GetButtonDown ("Fire2") && selected) {
			if (Physics.Raycast (ray, out hit, 100)) {
				navMeshAgent.destination = hit.point;
				navMeshAgent.Resume ();
				selected = false;
				rend.material.SetColor("_Color", Color.magenta);

			}
		}

	}

	void OnMouseOver()
	{
		//Debug.Log ("Mouse is here.");
		if (Input.GetMouseButtonDown (0)) {
			selected = true;
			Debug.Log ("Selected");
			rend.material.SetColor("_Color", Color.yellow);
		}
	}
}
