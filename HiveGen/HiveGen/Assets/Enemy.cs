using UnityEngine;
using System.Collections;

public class Enemy : Mover
{
    private float m_Speed = 0.1f;
    public override float Speed { get; set; }

    private int m_MaxHealth = 100;
    public int HealthPoints { get; private set; }

    public override Vector3 StartPos { get; set; }

    public override Vector3 GoalPos {get; set; }

    public override Vector3 Position { get; set; }

    public override bool IsMoving {get; set;}

    private GameObject m_EnemyObject;
    public GameObject EnemyObject
    {
        get { return m_EnemyObject; }
        set
        {
            if (value != null)
            {
                m_EnemyObject = value;
            }
        }
    }

    float m_MoveStartTime;

    public Enemy(GameObject obj)
    {
        EnemyObject = obj;
        Position = obj.transform.position;
        IsMoving = false;
        Speed = m_Speed;
        HealthPoints = m_MaxHealth;
    }

    public void Die()
    {
        Object.Destroy(EnemyObject);
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

    //Sets initial variables for starting movement, movement is done in update.
    public override void MoveTo(Vector3 goal)
    {
        GoalPos = goal;
        StartPos = Position;
        IsMoving = true;
        m_MoveStartTime = Time.time;
    }

    private bool IsAtGoal()
    {
        return Position.Equals(GoalPos);
    }

    public override void MoverUpdate()
    {
        Position = EnemyObject.transform.position;
        //Debug.Log(IsMoving);
        Debug.Log(IsAtGoal());
        Debug.Log("GOAL: " + GoalPos.ToString());
        Debug.Log("Cur: " + Position.ToString());
        if (IsAtGoal())
        {
            this.StopMoving();
        }
        if (IsMoving)
        {
            EnemyObject.transform.position = Vector3.Lerp(StartPos, GoalPos, Speed * (Time.time - m_MoveStartTime));
        }
    }

    public override void MoverStart()
    {

    }
}
