using UnityEngine;
using System.Collections;

public abstract class Mover
{
    public virtual float Speed
    {
        get;
        set;
    }

    //Common methods of getting position return 3d vector
    public virtual Vector3 StartPos
    {
        get;
        set;
    }

    public virtual Vector3 GoalPos
    {
        get;
        set;
    }

    public virtual Vector3 Position
    {
        get;
        set;
    }

    public virtual bool IsMoving
    {
        get;
        set;
    }

    public void StopMoving()
    {
        IsMoving = false;
    }

    public abstract void MoveTo(Vector3 goal);

    public abstract void MoverUpdate();

    public abstract void MoverStart();
}
