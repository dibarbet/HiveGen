using UnityEngine;
using System.Collections;

public class CameraUpdater : MonoBehaviour {

	private Transform target;
	public float distance = 10f;

	// Update is called once per frame
	void Update () {
		if (target==null)
			target = GameObject.FindGameObjectWithTag("Player").transform;
		transform.position = new Vector3(target.position.x, target.position.y, target.position.z - distance);
	}
}
