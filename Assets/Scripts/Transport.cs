using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transport : MonoBehaviour {

	public Queue<Vector3> points;

	public bool active;
	public bool expired;
	private bool deadOnArrival;
	private Vector3 lastPoint;
	private Vector3 targetPoint;

	private float speed;
	private float collisionVelocity = 20.0f;
	private float explosiveVelocity = 500.0f;
	private float expirtationPeriod = 10.0f;
	private float honkRadius = 5.0f;

	private Rigidbody rigid;
	private AudioSource[] audioSources;
	private AudioSource honkSource;
	private AudioSource crashSource;


	// Use this for initialization
	void Awake () {
		points = new Queue<Vector3>();
		speed = Random.Range(20, 30);
		rigid = GetComponent<Rigidbody>();
		rigid.isKinematic = true;
		
		audioSources = GetComponents<AudioSource>();
		foreach (AudioSource audioSource in audioSources) {
			StartCoroutine (AudioFade.In (audioSource, 2.0f));
		}

		honkSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		honkSource.spatialBlend = 1.0f;
		honkSource.rolloffMode = AudioRolloffMode.Custom;
		honkSource.maxDistance = 70.0f;
		honkSource.dopplerLevel = 0.0f;
		honkSource.clip = Controller.honkList[Random.Range(0, Controller.honkList.Count)];

		crashSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		crashSource.spatialBlend = 1.0f;
		crashSource.rolloffMode = AudioRolloffMode.Custom;
		crashSource.maxDistance = 70.0f;
		crashSource.dopplerLevel = 0.0f;
		crashSource.clip = Controller.crashList[Random.Range(0, Controller.crashList.Count)];
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

			if(tag != "Plane") {
				IEnumerable<Collider> honkObjects = Physics.OverlapSphere(transform.position, honkRadius)
					.Where(c => c.tag != "Terrain" && c.tag != "Plane" && c.transform.position != transform.position && !c.GetComponent<Transport>().expired);
				if (honkObjects.Count() > 0) {
					PlaySource(honkSource, 0.6f);
				}
			}
		}
		if (expired) {
			expirtationPeriod -= Time.deltaTime;
			if ( expirtationPeriod < 0 ) {
				Remove();
			}
		}
	}

	void Remove() {
		foreach (AudioSource audioSource in audioSources) {
			StartCoroutine (AudioFade.VolumeOut (audioSource, 1.5f));
		}
		Destroy(gameObject, 1.5f);
	}

	void PlaySource(AudioSource source, float vol) {
		if (!source.isPlaying) {
			source.volume = vol;
			source.Play();
		}
	}

	// Add new point to trajectory 
	public void QueuePoint(Vector3 point) {
		if (!expired || deadOnArrival) {
			// Debug.Log("Point Queued: " +  point);
			points.Enqueue(point);

			// If we're not active and have enough points to start a route set up the transport
			if(!active && points.Count >= 2) {
				Vector3 startPoint = getOffScreenPoint(points.ElementAt(0) - points.ElementAt(1));
				// Debug.Log("Activating Transport. Starting Point: " +  startPoint);
				transform.localPosition = new Vector3(startPoint.x, 0.1f, startPoint.z);
				rigid.isKinematic = false;
				active = true;
				DequeuePoint();
			}
		}
	}
 
	// Follow the direction until we have an off screen starting point
	Vector3 getOffScreenPoint(Vector3 maybePoint) {
		// Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), maybePoint, Quaternion.Euler(-90, 0, 0));
		Vector3 cameraPoint = Camera.main.WorldToViewportPoint(maybePoint);
		// Debug.Log("cameraPoint: " + cameraPoint);
		if (cameraPoint.x < -0.25 || cameraPoint.x > 1.25 || cameraPoint.y < -1.5 || cameraPoint.y > 1.25) {
			return maybePoint;
		} else {
			return getOffScreenPoint(maybePoint + (maybePoint.normalized * 5));
		}
	}

	// Dequeue next target point
	void DequeuePoint() {
		if (points.Count >= 1) {
			lastPoint = targetPoint;
			targetPoint = points.Dequeue();
		} else if (deadOnArrival) {
			Remove();
		} else {
			Vector3 targetDir = transform.localPosition - lastPoint;
			targetPoint = targetDir * 100;
			deadOnArrival = true;
		}
	}

	void OnCollisionEnter(Collision collision){
		Debug.Log("collision: " + gameObject.name + " hit " + collision.collider.name);
		if(collision.collider.tag != "Terrain") {
			active = false;
			expired = true;
			rigid.velocity = collisionVelocity * rigid.velocity.normalized;
			rigid.AddExplosionForce(explosiveVelocity, collision.transform.position, explosiveVelocity);
			rigid.useGravity = true;
			foreach (AudioSource audioSource in audioSources) {
				StartCoroutine (AudioFade.PitchOut (audioSource, 1.5f));
			}
			StartCoroutine (AudioFade.PitchOut (honkSource, 10.0f));
		} 

		if(expired == true) {
			foreach (ContactPoint contact in collision.contacts) {
				if(collision.collider.tag == "Plane") {
					Instantiate(Controller.largeExplosion, contact.point, Quaternion.Euler(-90, 0, 0));
					PlaySource(crashSource, 0.5f);
				} else if (collision.collider.tag != "Terrain")  {
					Instantiate(Controller.smallExplosion, contact.point, Quaternion.Euler(-90, 0, 0));
					PlaySource(crashSource, 0.5f);
				} else {
					Instantiate(Controller.sparks, contact.point, Quaternion.Euler(-90, 0, 0));
					PlaySource(crashSource, 0.3f);
				}
			}
		}
	}
}
