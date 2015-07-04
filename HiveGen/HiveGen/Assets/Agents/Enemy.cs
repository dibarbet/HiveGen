using UnityEngine;
using System.Collections;

public class Enemy : Mover
{
    private float m_Speed = 100f;

    private int m_MaxHealth = 100;
    public int HealthPoints { get; private set; }

    public GameObject EnemyObject { get; set; }

    //True if operation succeeded
    public bool Die()
    {
        if (EnemyObject != null)
        {
            Object.Destroy(EnemyObject);
            return true;
        }
        return false;
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

    public override void Update()
    {
        Position = transform.position;
        //Debug.Log(IsMoving);
        //Debug.Log(IsAtGoal());
        //Debug.Log("GOAL: " + GoalPos.ToString());
        //Debug.Log("Cur: " + Position.ToString());
        if (IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, GoalPos, Speed * Time.deltaTime);
        }

    }

    public void Attack()
    {
        Player.DecrementHealth(5);
    }




    private Collider2D col2D;
    private Rigidbody2D rgdBdy;
    private Player Player;

    public void Awake()
    {
        EnemyObject = this.gameObject;
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Position = this.transform.position;
        IsMoving = false;
        Speed = m_Speed;
        HealthPoints = m_MaxHealth;
        col2D = EnemyObject.GetComponent<Collider2D>();
        rgdBdy = EnemyObject.gameObject.GetComponent<Rigidbody2D>();
    }
}
