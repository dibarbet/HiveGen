using UnityEngine;
using System.Collections;

public class CameraUpdater : MonoBehaviour {

	private Transform target;
	public float distance = 10f;

	// Update is called once per frame
	void Update () {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
            else
            {
                DestroyObject(this);
                return;
            }
            
        }
		transform.position = new Vector3(target.position.x, target.position.y, target.position.z - distance);
	}
}
