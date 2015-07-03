using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestGameManager : MonoBehaviour
{

    public GameObject obj;

    private List<Mover> Movers;

	// Use this for initialization
	void Start () {
        Movers = new List<Mover>();
        Debug.Log("Attempting Move");
        Enemy enem = new Enemy(obj);
        Movers.Add(enem);
        enem.MoveTo(new Vector3(10f, 10f));
	}

    void UpdateAllMovers()
    {
        foreach (Mover mov in Movers)
        {
            mov.MoverUpdate();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateAllMovers();
	}
}
