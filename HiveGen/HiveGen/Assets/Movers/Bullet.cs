using UnityEngine;
using System.Collections;

public class Bullet : Mover
{

    private float m_StartTime;

    private float m_Speed = 10.0f;
    public override float Speed
    {
        get { return m_Speed; }
        set { m_Speed = value; }
    }
    Rigidbody2D rgd;
	// Use this for initialization
    //To make this work, make the sprite larger.
	public override void Start ()
    {
        m_StartTime = Time.time;
        rgd = this.gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	public override void Update ()
    {
        //Debug.Log("Bullet");
        if (Time.time - m_StartTime <= 15)
        {
            rgd.velocity = transform.up * Speed;
        }
        else
        {
            Die();
        }
        
	}

    //Select is trigger on the collider box, make sure rigid body exists, then use this rather than on collision enter.
    //This prevents wacky physics
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag != "Player" && col.gameObject.tag != "Floor")
        {
            //Debug.Log("Destroying bullet");
            Die();
            if (col.gameObject.tag == "Enemy")
            {
                col.gameObject.GetComponent<Enemy>().DecrementHealth(10);
            }
        }
    }
}
