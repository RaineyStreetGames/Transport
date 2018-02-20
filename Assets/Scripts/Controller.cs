using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

	public GameObject plane;
	public GameObject transportType1;
	public GameObject transportType2;
	public GameObject transportType3;
	public GameObject transportType4;
	public GameObject transportType5;
	public GameObject transportType6;
	public GameObject transportType7;
	List<GameObject> transportTypeList;

	public GameObject pointType;

	private Transport current;
	private Vector3 lastPoint;
	private float pointRadius = 0.5f;

	// Use this for initialization
	void Start () {
		transportTypeList = new List<GameObject>();
		transportTypeList.Add(transportType1);
		transportTypeList.Add(transportType2);
		transportTypeList.Add(transportType3);
		transportTypeList.Add(transportType4);
		transportTypeList.Add(transportType5);
		transportTypeList.Add(transportType6);
		transportTypeList.Add(transportType7);
	}
	
	// Update is called once per frame
	void Update () {
		// Mouse Drag Event
		if (Input.GetMouseButton(0))
		{
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && hit.transform.name == plane.name)
			{
				// Debug.Log("Mouse down. Raycast: " +  hit.point);
				if (current == null) {
					current = newTransport(hit.point);
					current.QueuePoint(hit.point);
				} else {
					if(Vector3.Distance(hit.point, lastPoint) >= pointRadius) {
						Instantiate(pointType, hit.point, Quaternion.Euler(-90, 0, 0));
						lastPoint = hit.point;
						current.QueuePoint(hit.point);
					}
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
		Instantiate(pointType, point, Quaternion.Euler(-90, 0, 0));
		GameObject transportType = transportTypeList[Random.Range(0, transportTypeList.Count)];
		GameObject newTransport =  Instantiate(transportType, new Vector3(0, -10, 0), Quaternion.Euler (Vector3.one));
		return newTransport.GetComponent<Transport>();
	}
}
