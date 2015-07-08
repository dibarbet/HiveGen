using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AStar;

public class Enemy : Mover
{
    private float m_Speed = 5.0f;
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

    private SpatialAStar<GameManager.SpecialPathNode, System.Object> aStar;

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
        aStar = new SpatialAStar<GameManager.SpecialPathNode, System.Object>(GameManager.boardArray);
        StartPos = transform.position;
    }

    public override void MoveTo(Vector3 goal)
    {
        //LinkedList<GameManager.SpecialPathNode> path = aStar.Search(transform.position.x, transform.y, goal.x, goal.y, null);
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
        if (Player != null)
        {
            Player.DecrementHealth(5);
        }
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
            Debug.Log("Goal before collision: " + GoalPos.ToString());
            this.PauseMoving();
            Debug.Log("Goal after collision (should be position): " + GoalPos.ToString());
            InvokeRepeating("Attack", .3f, 1f);
        }
        else if (col.gameObject.tag == "Terrain")
        {
            Debug.Log("Goal before collision: " + GoalPos.ToString());
            this.PauseMoving();
            Debug.Log("Goal after collision (should be position): " + GoalPos.ToString());
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Terrain")
        {
            this.UnPauseMoving();
            Debug.Log("Restored goal after exit collision: " + GoalPos.ToString());
            CancelInvoke("Attack");
        }
        else if (col.gameObject.tag == "Terrain")
        {
            Debug.Log("Goal before collision: " + GoalPos.ToString());
            this.PauseMoving();
            Debug.Log("Goal after collision (should be position): " + GoalPos.ToString());
        }
    }
}
