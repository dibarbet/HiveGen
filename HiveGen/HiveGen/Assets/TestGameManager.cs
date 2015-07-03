using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestGameManager : MonoBehaviour
{

    public GameObject EnemyObject;
    public GameObject PlayerObject;

    private List<Enemy> enemies;

	// Use this for initialization
	void Start () {
        enemies = new List<Enemy>();
        Enemy enemy = EnemyObject.GetComponent<Enemy>();
        enemy.MoveTo(new Vector3(10f, 10f));
	}
	
	// Update is called once per frame
	void Update ()
    {
        
        
	}

}
