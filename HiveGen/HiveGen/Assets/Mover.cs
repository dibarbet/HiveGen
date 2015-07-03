using UnityEngine;
using System.Collections;

public interface Mover
{
    float Speed
    {
        get;
        set;
    }

    //Common methods of getting position return 3d vector
    Vector3 StartPos
    {
        get;
        set;
    }

    Vector3 GoalPos
    {
        get;
        set;
    }

    Vector3 Position
    {
        get;
        set;
    }

    bool IsMoving
    {
        get;
        set;
    }

    void StopMoving();

    void MoveTo(Vector3 goal);

}
