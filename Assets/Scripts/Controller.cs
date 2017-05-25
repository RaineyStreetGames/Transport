using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

	public GameObject plane;
	public GameObject transportType;

	public Transport current;
	public Vector3 lastPoint;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Mouse Drag Event
		if (Input.GetMouseButton(0))
		{
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
			{
				Debug.Log("Mouse down. Raycast: " +  hit.point);
				if (current == null) {
					lastPoint = hit.point;
					current = newTransport(hit.point);
				} else {
					
				}
			}
		}

		// Mouse Up Event
		if (Input.GetMouseButtonUp(0)) {
			current = null;
		}
	}

	private Transport newTransport(Vector3 point) {
		Debug.Log("New Transport");
		lastPoint = point;
		GameObject newTransport =  Instantiate (transportType, Vector3.one, Quaternion.Euler (Vector3.zero));
		return newTransport.GetComponent<Transport>();
	}
}
