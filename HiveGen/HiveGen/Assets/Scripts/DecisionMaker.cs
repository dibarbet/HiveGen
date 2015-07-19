using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DecisionMaker
{

    /**
     * Idea for tree
     *              thisEnemyHP?
     *              /           \
     *             low           high
     *            /               \
     */


    

    public DecisionTree<string> tree;
    Node<string> root;

	// Use this for initialization
	public DecisionMaker()
    {
        root = new Node<string>("Player Distance");
        //leaf
        root.AddChild(new Node<string>("CHASE"), "SIGHT");
        Node<string> EnemyHasSight = new Node<string>("Enemy Has Sight");
        root.AddChild(EnemyHasSight, "FAR");
        //reached leaf
        EnemyHasSight.AddChild(new Node<string>("FINDENEMY"), "YES");
        
        Node<string> bulletVisible = new Node<string>("Bullet Visible");
        EnemyHasSight.AddChild(bulletVisible, "NO");
        //leaf
        bulletVisible.AddChild(new Node<string>("FINDBULLET"), "YES");

        Node<string> playerHP = new Node<string>("Player HP");
        bulletVisible.AddChild(playerHP, "NO");
        //leaf
        playerHP.AddChild(new Node<string>("STAY"), "HIGH");
        //leaf
        playerHP.AddChild(new Node<string>("DEFENDGOAL"), "LOW");

        

        
        /**
        //decision node
        Node<string>lowDistance = root.AddChild(new Node<string>("Distance to Player", null, new List<string>() {"CLOSE", "SIGHT", "FAR"}), "LOW");
        //decision node
        Node<string>highDistance = root.AddChild(new Node<string>("Distance to Player", null, new List<string>() {"CLOSE", "SIGHT", "FAR"}), "HIGH");

        //leaf node
        highDistance.AddChild(new Node<string>("ATTACK", null, new List<string>()), "CLOSE");
        //decision node
        Node<string> playerHP = highDistance.AddChild(new Node<string>("Player HP", null, new List<string>() {"LOW", "HIGH"}), "SIGHT");
        //leaf node
        highDistance.AddChild(new Node<string>("ROAM", null, new List<string>()), "FAR");

        //leaf node
        lowDistance.AddChild(new Node<string>("ATTACK", null, new List<string>()), "CLOSE");
        lowDistance.AddChild(new Node<string>("DODGE", null, new List<string>()), "SIGHT");
        lowDistance.AddChild(new Node<string>("DEFEND", null, new List<string>()), "FAR");

        //Leaf nodes
        playerHP.AddChild(new Node<string>("CHASE", null, new List<string>()), "LOW");
        playerHP.AddChild(new Node<string>("DEFEND", null, new List<string>()), "HIGH");*/
	}
	
	// Update is called once per frame
	public string MakeDecision(List<AttributeValue<string>> attributes)
    {
        tree = new DecisionTree<string>(root, attributes);
        return tree.EvaluateTree();
    }
}

public class Node<T>
{
    public string Name { get; set; }
    public T Value { get; set; }

    public Node<T> Parent { get; set; }
    //public List<Node<T>> Children { get; private set; }
    //public List<T> ChildrenValues { get; private set; }
    public Dictionary<T, Node<T>> ChildDict;
    /**
    public Node(string name, T value, Node<T> parent)
    {
        this.Name = name;
        this.Value = value;
        this.Parent = parent;
        ChildDict = new Dictionary<T, Node<T>>();
    }*/

    public Node(string name)
    {
        this.Name = name;
        ChildDict = new Dictionary<T, Node<T>>();
    }

    //Add child and value, where value corresponds to the value of this node (e.g. if this node is true, go to child)
    public Node<T> AddChild(Node<T> child, T value)
    {
        ChildDict.Add(value, child);
        child.Parent = this;
        return child;
    }

    //set value for this node
    public void SetValue(T value)
    {
        this.Value = value;
    }

    //This returns the child corresponding the value of this node.
    //E.G:  If the value of this node is true, find the child node for the true branch
    public Node<T> GetChildFromClassification()
    {
        if (ChildDict.Keys.Count == 0)
        {
            return null;
        }
        return ChildDict[Value];
    }

    /*
    public Node<T> GetChildFromClassification(T value)
    {
        if (Children.Count == 0)
        {
            return null;
        }
        if (ChildrenValues != null)
        {
            int index = ChildrenValues.IndexOf(value);
            return Children[index];
        }
        return null;
    }*/

    public bool IsLeaf()
    {
        return ChildDict.Keys.Count == 0;
    }

    public override string ToString()
    {
        if (Value == null)
        {
            return Name.ToString() + ", Null Value";
        }
        else
        {
            return Name.ToString() + ", " + Value.ToString();
        }

    }
}

public class DecisionTree<T>
{
    Node<T> root;
    List<AttributeValue<T>> attributes;
    public DecisionTree(Node<T> root, List<AttributeValue<T>> attributes)
    {
        this.root = root;
        this.attributes = attributes;
    }

    public string EvaluateTree()
    {
        //Debug.Log("Attributes: " + attributes[0].value + ", " + attributes[1].value + ", " + attributes[2].value + ", " + attributes[3].value);
        Node<T> cur = root;
        string retVal = cur.Name;
        int iterations = 100;
        int curIter = 0;
        while (cur != null)
        {
            if (curIter >= iterations)
            {
                Debug.Log("INFINITE LOOP");
                return "";
            }
            retVal = cur.Name;
            if (cur.ChildDict.Keys.Count == 0)
            {
                return retVal;
            }
            string attribute = cur.Name;

            foreach (AttributeValue<T> attr in attributes)
            {
                //Debug.Log("Current name: " + cur.Name + "; attr name: " + attr.name);
                string attrName = attr.name;
                if (attrName.Equals(attribute))
                {
                    //Debug.Log("attrval: " + attr.value);
                    cur.SetValue(attr.value);
                    cur = cur.GetChildFromClassification();
                    break;
                }
            }
            curIter++;
        }
        return retVal;
    }

    public override string ToString()
    {
        return root.ToString() + "; Child 1: " + root.ChildDict.Keys.ToString();
    }
}

public class AttributeValue<T>
{
    public string name;
    public T value;

    public AttributeValue(string name)
    {
        this.name = name;
    }

    public AttributeValue(string name, T value)
    {
        this.name = name;
        this.value = value;
    }
}
