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

    public virtual bool IsAtGoal()
    {
        float remainingDist = (transform.position - GoalPos).sqrMagnitude;
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

    public virtual void Start()
    {

    }

    public virtual void Update()
    {
        
    }

}
