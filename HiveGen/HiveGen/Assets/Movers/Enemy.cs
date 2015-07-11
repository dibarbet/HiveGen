using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AStar;

public class Enemy : Mover
{
    private float m_Speed = 1.0f;
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

    public int TileX { get; set; }
    public int TileY { get; set; }

    private SpatialAStar<GameManager.SpecialPathNode, System.Object> aStar;
    private LinkedList<GameManager.SpecialPathNode> path;
    private LinkedListNode<GameManager.SpecialPathNode> CurrentGoalNode;

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
        StartPos = transform.position;
    }

    public void InstantiateAStar(GameManager.SpecialPathNode[,] board)
    {
        aStar = new SpatialAStar<GameManager.SpecialPathNode, System.Object>(board);
    }

    public bool MoveToTile(GameManager.SpecialPathNode tile)
    {
        path = aStar.Search(TileX, TileY, tile.X, tile.Y, null);
        //Debug.Log("Enemy: " + TileX + ", " + TileY + "; Player: " + tile.X + ", " + tile.Y);
        if (path != null && path.Count > 0)
        {
            IsMoving = true;
            GoalPos = path.First.Value.tile.transform.position;
            CurrentGoalNode = path.First;
            LinkedListNode<GameManager.SpecialPathNode> next = path.First;
            string pathStr = "";
            while (next != null)
            {
                pathStr += "(" + next.Value.X + ", " + next.Value.Y + "); ";
                next = next.Next;
            }
            //Debug.Log(pathStr);
            return true;
        }
        return false;
    }

    private void MoveToNode(LinkedListNode<GameManager.SpecialPathNode> node)
    {
        if (node != null)
        {
            IsMoving = true;
            CurrentGoalNode = node;
            GoalPos = node.Value.tile.transform.position;
        }
        
    }

    public override void Update()
    {
        Position = transform.position;
        if ((CurrentGoalNode != null) && IsAtGoal(CurrentGoalNode.Value.tile.transform.position))
        {
            
            LinkedListNode<GameManager.SpecialPathNode> next = CurrentGoalNode.Next;
            if (next == null)
            {
                //Debug.Log("Final node found");
                //end of list
                IsMoving = false;
            }
            else
            {
                MoveToNode(next);
            }
        }
        else
        {
            if (CurrentGoalNode != null)
            {
                TileX = CurrentGoalNode.Value.X;
                TileY = CurrentGoalNode.Value.Y;
            }
        }
        if (IsMoving)
        {
            //Debug.Log("goal: " + GoalPos);
            transform.position = Vector3.MoveTowards(transform.position, GoalPos, Speed * Time.deltaTime);
        }
    }

    //Perhaps trigger animations here?
    public void Attack()
    {
        Debug.Log("ATTACK!");
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

    private bool attacking = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        //Ignore collisions between enemies
        if (col.gameObject.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(col.gameObject.GetComponent<Collider2D>(), col2D);
        }
        if (col.gameObject.tag == "Player")
        {
            //Debug.Log("Goal before collision: " + GoalPos.ToString());
            //this.PauseMoving();
            //Debug.Log("Goal after collision (should be position): " + GoalPos.ToString());
            if (!attacking)
            {
                InvokeRepeating("Attack", .1f, .5f);
                attacking = true;
            }
            
        }
        else if (col.gameObject.tag == "Terrain")
        {
            Physics2D.IgnoreCollision(col.collider, col2D);
            //Debug.Log("Goal before collision: " + GoalPos.ToString());
            //this.PauseMoving();
            //Debug.Log("Goal after collision (should be position): " + GoalPos.ToString());
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Terrain")
        {
            //this.UnPauseMoving();
            //Debug.Log("Restored goal after exit collision: " + GoalPos.ToString());
            CancelInvoke("Attack");
        }
        else if (col.gameObject.tag == "Terrain")
        {
            //Debug.Log("Goal before collision: " + GoalPos.ToString());
            //this.PauseMoving();
            //Debug.Log("Goal after collision (should be position): " + GoalPos.ToString());
        }
        MoveToNode(CurrentGoalNode);
    }
}
