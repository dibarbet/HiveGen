using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TestGameManager : MonoBehaviour
{

    public GameObject EnemyObject;
    public GameObject PlayerObject;
    public GameObject BulletPref;
    public Text HP;

    private List<Enemy> enemies;
    private Player player;

	// Use this for initialization
	void Start () {
        enemies = new List<Enemy>();
        Enemy enemy = EnemyObject.GetComponent<Enemy>();
        enemy.MoveTo(new Vector3(300f, 150f));
        player = PlayerObject.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        HP.text = player.HealthPoints.ToString();
        
	}

}
