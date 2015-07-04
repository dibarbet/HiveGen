using UnityEngine;
using System.Collections;

public class Bullet : Mover
{

    private float m_StartTime;

    private float m_Speed = 300.0f;
    public override float Speed
    {
        get { return m_Speed; }
        set { m_Speed = value; }
    }

	// Use this for initialization
	void Start ()
    {
        m_StartTime = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Rigidbody2D rgd = this.gameObject.GetComponent<Rigidbody2D>();
        rgd.velocity = transform.up * Speed;
	}

    //Select is trigger on the collider box, make sure rigid body exists, then use this rather than on collision enter.
    //This prevents wacky physics
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag != "Player")
        {
            Debug.Log("Destroying bullet");
            Object.Destroy(this.gameObject);
            if (col.gameObject.tag == "Enemy")
            {
                col.gameObject.GetComponent<Enemy>().DecrementHealth(10);
            }
        }
        
    }
}
