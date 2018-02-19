using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transport : MonoBehaviour {

	public Queue<Vector3> points;

	private bool active;
	private bool deadOnArrival;
	private Vector3 lastPoint;
	private Vector3 targetPoint;

	private float speed;

	// Use this for initialization
	void Awake () {
		points = new Queue<Vector3>();
		speed = Random.Range(20, 30);
	}
	
	// Update is called once per frame
	void Update () {
		if (active) {
			targetPoint.y = transform.position.y;
			// rotate towards the target
			transform.forward = Vector3.RotateTowards(transform.forward, targetPoint - transform.position, speed*Time.deltaTime, 0.0f);

			// move towards the target
			transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed*Time.deltaTime);
			
			if(transform.localPosition == targetPoint) {
				DequeuePoint();
			} 
		}
	}

	// Add new point to trajectory 
	public void QueuePoint(Vector3 point) {
		Debug.Log("Point Queued: " +  point);
		points.Enqueue(point);

		// If we're not active and have enough points to start a route set up the transport
		if(!active && points.Count >= 2) {
			Vector3 startPoint = getOffScreenPoint((points.ElementAt(0) - points.ElementAt(1)) * 5);
			Debug.Log("Activating Transport. Starting Point: " +  startPoint);
			transform.localPosition = new Vector3(startPoint.x, 0.5f, startPoint.z);
			active = true;
			DequeuePoint();
		}
	}
 
	// Follow the direction until we have an off screen starting point
	Vector3 getOffScreenPoint(Vector3 maybePoint) {
		Vector3 cameraPoint = Camera.main.WorldToViewportPoint(maybePoint);
		Debug.Log("cameraPoint: " + cameraPoint);
		if (cameraPoint.x < 0 || cameraPoint.x > 1 || cameraPoint.y < 0 || cameraPoint.y > 1) {
			return maybePoint;
		} else {
			return getOffScreenPoint(maybePoint * 5);
		}
	}

	// Dequeue next target point
	void DequeuePoint() {
		if (points.Count >= 1) {
			lastPoint = targetPoint;
			targetPoint = points.Dequeue();
		} else if (deadOnArrival) {
			Destroy(gameObject);
		} else {
			Vector3 targetDir = transform.localPosition - lastPoint;
			targetPoint = targetDir * 100;
			deadOnArrival = true;
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		Debug.Log("collision: " + gameObject.name + " hit " + collision.collider.name);
		if(collision.collider.name != "Plane") {
			active = false;
			Rigidbody rigid = GetComponent<Rigidbody>();
			rigid.velocity = rigid.velocity.normalized * 20;
		} 
	}
}
