using UnityEngine;
using System.Collections;

public class BulletMover : Mover {

	public float speed = 50f;
	//private Rigidbody2D rigidbody = this.gameObject.GetComponent.;
//	var Player : GameObject
//
//	void Start(){
//		transform.forward = Player.transform.forward;
//	}

	void Update () {
		transform.Translate(transform.right * speed * Time.deltaTime);
	}
}
