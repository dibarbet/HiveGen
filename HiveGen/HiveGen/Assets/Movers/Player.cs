﻿using UnityEngine;
using System.Collections;

public class Player : Mover
{
    private float m_RotationSpeed = 100.0f;
    private float m_Speed = 2.0f;
    public override float Speed
    {
        get { return m_Speed; }
        set { m_Speed = value; }
    }

    private int m_MaxHealth = 100;
    public int HealthPoints { get; private set; }

    private Collider2D col2D;
    private Rigidbody2D rgdBdy;

// <<<<<<< HEAD:HiveGen/HiveGen/Assets/Agents/Player.cs
	// public GameObject shot;
	// public Transform shotSpawn;
	// public float fireRate;
	// private float nextFire;

    // public bool Die()
// =======
    public GameObject BulletPrefab;

    public void Awake()
    {
        Position = transform.position;
        IsMoving = false;
        HealthPoints = m_MaxHealth;
        col2D = this.gameObject.GetComponent<Collider2D>();
        rgdBdy = this.gameObject.GetComponent<Rigidbody2D>();
    }

    public override void Update()
    {
        Position = transform.position;

        if (Input.GetKeyDown("space"))
        {
            //Instantiate at the correct position and rotation
            var nBullet = GameObject.Instantiate(BulletPrefab, transform.position, transform.rotation);
            //Ignore collision between this and player.
            Physics2D.IgnoreCollision(BulletPrefab.GetComponent<Collider2D>(), col2D);
        }
        //This code will make it follow the mouse
        /**
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);

        transform.position += Time.deltaTime * Speed * Input.GetAxis("Vertical") * transform.up;
        //transform.position += Time.deltaTime * Speed * Input.GetAxis("Horizontal") * transform.up;*/

        //This code uses WASD
        float translation = Input.GetAxis("Vertical") * Speed;
        float rotation = Input.GetAxis("Horizontal") * m_RotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, translation, 0);
        transform.Rotate(0, 0, -rotation);
    }

    public bool DecrementHealth(int amt)
    {
        if (amt >= 0)
        {
            if ((HealthPoints - amt) <= 0)
            {
                this.Die();
            }
            else
            {
                HealthPoints -= amt;
            }
            return true;
        }
        return false;
    }

    public bool IncrementHealth(int amt)
    {
        if (amt >= 0)
        {
            if (HealthPoints == 0)
            {
                this.Die();
                return false;
            }
            else if ((HealthPoints + amt) >= 100)
            {
                HealthPoints = 100;
            }
            else
            {
                HealthPoints += amt;
            }
            return true;
        }
        return false;
    }

    private int m_TimeInsideEnemy;
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            //DecrementHealth(1);
            m_TimeInsideEnemy = 0;
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            if (m_TimeInsideEnemy % 100 == 0)
            {
                //DecrementHealth(1);
            }
        }
        m_TimeInsideEnemy++;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            m_TimeInsideEnemy = 0;
        }
    }
// <<<<<<< HEAD:HiveGen/HiveGen/Assets/Agents/Player.cs

    // public override void Update()
    // {
        // //Debug.Log("Moving player");
        // Position = transform.position;

        // //This code will make it follow the mouse
        // /**
        // Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);

        // transform.position += Time.deltaTime * m_Speed * Input.GetAxis("Vertical") * transform.up;
        // //transform.position += Time.deltaTime * m_Speed * Input.GetAxis("Horizontal") * transform.up;*/

        // //This code uses WASD
        // float translation = Input.GetAxis("Vertical") * m_Speed;
        // float rotation = Input.GetAxis("Horizontal") * m_RotationSpeed;
        // translation *= Time.deltaTime;
        // rotation *= Time.deltaTime;
        // transform.Translate(0, translation, 0);
        // transform.Rotate(0, 0, -rotation);

		// if(Input.GetButton("Fire1") && Time.time > nextFire){
			// nextFire = Time.time + fireRate;
			// Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
		// }
    // }

// =======
// >>>>>>> origin/master:HiveGen/HiveGen/Assets/Movers/Player.cs
}
