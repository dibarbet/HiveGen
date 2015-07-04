using UnityEngine;
using System.Collections;

public class Player : Mover
{
    private float m_RotationSpeed = 100.0f;
    private float m_Speed = 100f;

    private int m_MaxHealth = 100;
    public int HealthPoints { get; private set; }

    public GameObject PlayerObject { get; set; }

    private Collider2D col2D;
    private Rigidbody2D rgdBdy;

    public bool Die()
    {
        if (PlayerObject != null)
        {
            Object.Destroy(PlayerObject);
            return true;
        }
        return false;
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
            DecrementHealth(1);
            m_TimeInsideEnemy = 0;
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            if (m_TimeInsideEnemy % 100 == 0)
            {
                DecrementHealth(1);
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
        rgdBdy.mass = 1;
    }

    public override void Update()
    {
        //Debug.Log("Moving player");
        Position = PlayerObject.transform.position;

        //This code will make it follow the mouse
        /**
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);

        transform.position += Time.deltaTime * m_Speed * Input.GetAxis("Vertical") * transform.up;
        //transform.position += Time.deltaTime * m_Speed * Input.GetAxis("Horizontal") * transform.up;*/

        //This code uses WASD
        float translation = Input.GetAxis("Vertical") * m_Speed;
        float rotation = Input.GetAxis("Horizontal") * m_RotationSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, translation, 0);
        transform.Rotate(0, 0, -rotation);
        
    }

    public void Awake()
    {
        PlayerObject = this.gameObject;
        Position = transform.position;
        IsMoving = false;
        Speed = m_Speed;
        HealthPoints = m_MaxHealth;
        col2D = PlayerObject.GetComponent<Collider2D>();
        rgdBdy = PlayerObject.GetComponent<Rigidbody2D>();
    }

}
