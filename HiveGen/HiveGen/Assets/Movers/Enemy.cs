using UnityEngine;
using System.Collections;

public class Enemy : Mover
{
    private float m_Speed = 100.0f;
    public override float Speed
    {
        get { return m_Speed; }
        set { m_Speed = value; }
    }

    private int m_MaxHealth = 100;
    public int HealthPoints { get; private set; }

    public GameObject EnemyObject { get; set; }

    private Collider2D col2D;
    private Rigidbody2D rgdBdy;
    private Player Player;

    //Use awake, start is not always called at object creation, leading to null reference errors
    public void Awake()
    {
        EnemyObject = this.gameObject;
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Position = this.transform.position;
        IsMoving = false;
        HealthPoints = m_MaxHealth;
        col2D = EnemyObject.GetComponent<Collider2D>();
        rgdBdy = EnemyObject.gameObject.GetComponent<Rigidbody2D>();
    }

    public override void Update()
    {
        Position = transform.position;
        if (IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, GoalPos, Speed * Time.deltaTime);
        }
    }

    //Perhaps trigger animations here?
    public void Attack()
    {
        Player.DecrementHealth(5);
    }

    //returns true if operation can succeed.
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
            Debug.Log("Decreasing HP: " + HealthPoints.ToString());
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
            Debug.Log("Increasing HP");
            return true;
        }
        return false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            this.StopMoving();
            InvokeRepeating("Attack", .3f, 1f);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            IsMoving = true;
            CancelInvoke("Attack");
        }
    }
}
