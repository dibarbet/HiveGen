using UnityEngine;
using System.Collections;

public class CameraUpdater : MonoBehaviour {

	public Transform target;
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
				if(!GameManager.titleImage.activeSelf && !GameManager.instance.titleBool && !GameManager.instance.doingSetup){
					print ("destroying camera");
					DestroyObject(this);
				}
                return;
            }
            
		}
		//print ("camera target: "+target);
		transform.position = new Vector3(target.position.x, target.position.y, target.position.z - distance);
	}
}
