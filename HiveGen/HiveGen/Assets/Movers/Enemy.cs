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

    DecisionMaker MakeDecision;
    public string curDecision;
    string prevDecision;

    AttributeValue<string> playerDist;
    AttributeValue<string> bulletClose;
    AttributeValue<string> prevDistDecision;
    AttributeValue<string> prevBulletDecision;

    AttributeValue<string> prevPlayerHPDecision;
    AttributeValue<string> playerHPDecision;

    AttributeValue<string> prevEnemyAttackedDecision;
    AttributeValue<string> enemyAttackedDecision;

    GameObject Exit;

    private int m_MaxHealth = 100;
    public int HealthPoints { get; private set; }

    private Collider2D col2D;
    private SpriteRenderer sprite;

    private Player Player;

    public int TileX { get; set; }
    public int TileY { get; set; }

    private SpatialAStar<GameManager.SpecialPathNode, System.Object> aStar;
    private LinkedList<GameManager.SpecialPathNode> path;
    private LinkedListNode<GameManager.SpecialPathNode> CurrentGoalNode;


    //Use awake, start is not always called at object creation, leading to null reference errors
    public void Awake()
    {
        MakeDecision = new DecisionMaker();
        playerDist = new AttributeValue<string>("Player Distance");
        bulletClose = new AttributeValue<string>("Bullet Visible");
        prevDistDecision = new AttributeValue<string>("Player Distance");
        prevBulletDecision = new AttributeValue<string>("Bullet Visible");
        prevPlayerHPDecision = new AttributeValue<string>("Player HP");
        playerHPDecision = new AttributeValue<string>("Player HP");
        prevEnemyAttackedDecision = new AttributeValue<string>("Enemy Has Sight");
        enemyAttackedDecision = new AttributeValue<string>("Enemy Has Sight");
        Position = this.transform.position;
        IsMoving = false;
        HealthPoints = m_MaxHealth;
        col2D = this.GetComponent<Collider2D>();
        StartPos = transform.position;
        sprite = this.GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Exit = GameObject.FindGameObjectWithTag("Exit");
    }


    GameManager.SpecialPathNode prevPlayerTile;
    GameManager.SpecialPathNode nTile;

    private float nextActionTime = 0.0f;
    private float period = 0.01f;

    public override void Update()
    {
        Bullet target = null;
        Enemy attacked = null;
        //Debug.Log("CURRENT DECISION: " + curDecision);

        prevDecision = curDecision;

        //Determine if player tile changed
        prevPlayerTile = nTile;
        nTile = Player.GetTileOn();
        
        //Only calculate distance change if new tile.
        if (nTile != prevPlayerTile)
        {
            //
        }
        //Get bullet if close to move to
        target = BulletVisibleDecision();
        PlayerHPDecision();
        attacked = FriendlyEnemyHasSight();
        //Only need to recalculate distance to player if the player changes tiles
        if (prevBulletDecision.value != bulletClose.value || nTile != prevPlayerTile || curDecision == "DONEMOVINGTOBULLET" || curDecision == "DONEMOVINGTOENEMY")
        {
            //Prevents too many decisions from happening
            if ((Time.time > nextActionTime))
            {
                nextActionTime = Time.time + period;

                //We have to perform the decision even distance result is the same, to make sure the enemy paths to the newest player position
                //If we did not, the enemy could just stay in range but never be hit by the enemy
                string dis = DistanceToPlayerDecision();
                if (dis == "FAR")
                {
                    if (curDecision != "MOVINGTOBULLET")// && curDecision != "MOVINGTOENEMY")
                    {
                        //Debug.Log("Making a decision");
                        curDecision = MakeDecision.MakeDecision(new List<AttributeValue<string>>() { playerDist, bulletClose, playerHPDecision, enemyAttackedDecision });
                    }
                }
                else
                {
                    //Debug.Log("Making a decision");
                    curDecision = MakeDecision.MakeDecision(new List<AttributeValue<string>>() { playerDist, bulletClose, playerHPDecision, enemyAttackedDecision });
                }
                
            }   
        }

        

        
        //Execute decision

        if (curDecision == "CHASE")
        {
            GameManager.SpecialPathNode playerTile = Player.GetTileOn();
            this.MoveToTile(playerTile);
            //To make sure it keeps moving, rather than sitting on "CHASE"
            curDecision = "FINDINGPLAYER";
        }
        else if (curDecision == "STAY")
        {
            IsMoving = false;
        }
        else if (curDecision == "FINDBULLET")
        {
            
            if (target != null)
            {
                Debug.Log(target);
                this.MoveToTile(target.GetTileOn());
                curDecision = "MOVINGTOBULLET";
            }
        }
        else if (curDecision == "DEFENDGOAL")
        {
            GameManager.SpecialPathNode exitTile = GameManager.ExitTile;
            Debug.Log("EXit: " + exitTile.X + ", " + exitTile.Y);
            this.MoveToTile(exitTile);
            curDecision = "DEFENDINGGOAL";
        }
        else if (curDecision == "DEFENDINGGOAL")
        {
            float dist = Vector3.Distance(this.transform.position, GameManager.ExitTile.tile.transform.position);
            if (dist < 2.0f)
            {
                IsMoving = false;
                curDecision = "ATGOAL";
            }
        }
        else if (curDecision == "FINDENEMY")
        {
            if (attacked != null)
            {
                this.MoveToTile(attacked.GetTileOn());
                curDecision = "MOVINGTOENEMY";
            }
        }

        Position = transform.position;
        if ((CurrentGoalNode != null) && IsAtGoal(CurrentGoalNode.Value.tile.transform.position))
        {

            LinkedListNode<GameManager.SpecialPathNode> next = CurrentGoalNode.Next;
            if (next == null)
            {
                //Debug.Log("Final node found");
                //end of list
                IsMoving = false;
                if (curDecision == "MOVINGTOBULLET")
                {
                    curDecision = "DONEMOVINGTOBULLET";
                }
                if (curDecision == "MOVINGTOENEMY")
                {
                    curDecision = "DONEMOVINGTOENEMY";
                }
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

    float minDist = 5.0f;

    public string PlayerHPDecision()
    {
        prevPlayerHPDecision.value = playerHPDecision.value;
        int playerHP = Player.HealthPoints;
        if (playerHP < 50)
        {
            playerHPDecision.value = "LOW";
            return "LOW";
            
        }
        else
        {
            playerHPDecision.value = "HIGH";
            return "HIGH";
        }
    }


    public Enemy FriendlyEnemyHasSight()
    {
        prevEnemyAttackedDecision.value = enemyAttackedDecision.value;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        if (enemies == null || enemies.Length == 0)
        {
            enemyAttackedDecision.value = "NO";
            return null;
        }
        else
        {
            foreach(Enemy e in enemies)
            {
                if (e.curDecision == "FINDINGPLAYER")
                {
                    enemyAttackedDecision.value = "YES";
                    return e;
                }
            }
        }
        enemyAttackedDecision.value = "NO";
        return null;
    }

    public string DistanceToPlayerDecision()
    {
        prevDistDecision.value = playerDist.value;
        float dist = Vector3.Distance(this.transform.position, Player.transform.position);
        string result;
        if (dist < minDist)
        {
             result = "SIGHT";
             playerDist.value = result;
             
             //Debug.Log("Distance result: " + result + "; distance: " + dist);
             return result;
        }
        else
        {
            result = "FAR";
            playerDist.value = result;
            
            //Debug.Log("Distance result: " + result + "; distance: " + dist);
            return result;
        }
    }

    public Bullet BulletVisibleDecision()
    {
        prevBulletDecision.value = bulletClose.value;
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        if (bullets.Length == 0)
        {
            bulletClose.value = "NO";
            return null;
        }
        string result = "NO";
        foreach (Bullet b in bullets)
        {
            float dist = Vector3.Distance(this.transform.position, b.transform.position);
            if (dist < minDist)
            {
                result = "YES";
                bulletClose.value = result;
                //Debug.Log("Bullet result: " + result);
                return b;
            }
            else
            {
                result = "NO";
            }
        }
        bulletClose.value = result;
        
        //Debug.Log("Bullet result: " + result);
        return null;
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

    private IEnumerator ColorSprite()
    {
        Color original = sprite.color;
        sprite.color = new Color(0f, 0f, 0f, 1f);
        yield return new WaitForSeconds(0.1f);
        sprite.color = original;
    }

    //Perhaps trigger animations here?
    public void Attack()
    {
        Debug.Log("ATTACK!");
        if (Player != null)
        {
            Player.DecrementHealth(5);
            StartCoroutine(ColorSprite());
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
            attacking = false;
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
