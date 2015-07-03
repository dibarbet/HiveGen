using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, Mover
{
    public float Speed { get; set; }

    public Vector3 StartPos { get; set; }

    public Vector3 GoalPos {get; set; }

    public Vector3 Position { get; set; }

    public bool IsMoving {get; set;}

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
        Speed = .5f;
    }

    public void MoveTo(Vector3 goal)
    {
        GoalPos = goal;
        StartPos = Position;
        IsMoving = true;
        m_MoveStartTime = Time.time;
    }

    public void StopMoving()
    {
        IsMoving = false;
    }

    void Update()
    {
        if (IsMoving)
        {
            EnemyObject.transform.position = Vector3.Lerp(StartPos, GoalPos, Speed * (Time.time - m_MoveStartTime));
        }
    }

    void Start()
    {

    }
}
