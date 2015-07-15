using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
    public virtual float Speed { get; set; }

    //Common methods of getting position return 3d vector
    public virtual Vector3 StartPos { get; set; }

    public virtual Vector3 GoalPos { get; set; }

    public virtual Vector3 Position { get; set; }

    public virtual bool IsMoving { get; set; }

    public void StopMoving()
    {
        IsMoving = false;
    }

    private Vector3 prevStart;
    private Vector3 prevGoal;

    public void PauseMoving()
    {
        prevStart = StartPos;
        prevGoal = GoalPos;
        GoalPos = transform.position;
    }

    public void UnPauseMoving()
    {
        StartPos = prevStart;
        GoalPos = prevGoal;
    }

    public GameManager.SpecialPathNode GetTileOn()
    {
        GameManager.SpecialPathNode[,] board = GameManager.boardArray;
        foreach (GameManager.SpecialPathNode node in board)
        {
            if (node == null)
            {
                //Debug.Log("Null tile");
                continue;
            }
            else
            {
                //Debug.Log("Not null tile");
            }
            Debug.Log(node.tile.transform.position);
            if (node.tile.GetComponent<Collider2D>().bounds.Contains(this.transform.position))
            {
                Debug.Log("Found correct tile");
                return node;
            }
        }
        return null;
    }

    public virtual bool IsAtGoal(Vector3 goal)
    {
        float remainingDist = (transform.position - goal).sqrMagnitude;
        if (remainingDist < float.Epsilon)
        {
            return true;
        }
        return false;
    }

    public virtual void MoveTo(Vector3 goal)
    {
        GoalPos = goal;
        StartPos = Position;
        IsMoving = true;
    }

    public virtual bool Die()
    {
        if (this.gameObject != null)
        {
            Object.Destroy(this.gameObject);
            return true;
        }
        return false;
    }

    public virtual void Start()
    {
        GoalPos = transform.position;
        StartPos = transform.position;
    }

    public virtual void Update()
    {
        
    }

}
