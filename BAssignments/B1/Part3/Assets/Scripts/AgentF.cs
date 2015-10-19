using UnityEngine;
using System.Collections;

public class AgentF : MonoBehaviour {
	NavMeshAgent navMeshAgent;
	bool selected;
	Renderer rend;

	void Start () {
		navMeshAgent = GetComponentInParent<NavMeshAgent>();
		rend = GetComponent<Renderer>();
		selected = false;
	}

	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Input.GetButtonDown ("Fire2") && selected) {
			if (Physics.Raycast (ray, out hit, 100)) {
				if(Input.GetKey(KeyCode.LeftShift)) {
					navMeshAgent.speed = 6f;
				} else {
					navMeshAgent.speed = 3.5f;
				}
				navMeshAgent.destination = hit.point;
				navMeshAgent.Resume();
				selected = false;
				rend.materials[1].SetColor("_Color", Color.white);
			}
		}
	}

	void OnMouseOver()
	{
		if (Input.GetMouseButtonDown (0) && !selected) {
			selected = true;
			Debug.Log ("Selected");
			rend.materials[1].SetColor("_Color", Color.green);
		}
	}

}
