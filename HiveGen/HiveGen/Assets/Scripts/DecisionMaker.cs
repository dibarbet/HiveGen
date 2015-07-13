using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DecisionMaker : MonoBehaviour {

    /**
     * Idea for tree
     *              thisEnemyHP?
     *              /           \
     *             low           high
     *            /               \
     */


    public class Node<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }

        public Node<T> Parent { get; set; }
        public List<Node<T>> Children { get; private set; }
        public List<T> ChildrenValues { get; private set; }

        public Node(string name, T value, Node<T> parent)
        {
            this.Name = name;
            this.Value = value;
            this.Parent = parent;
            this.Children = new List<Node<T>>();
            this.ChildrenValues = new List<T>();
        }

        public Node(string name, Node<T> parent, List<T> values)
        {
            this.Name = name;
            this.Parent = parent;
            this.Children = new List<Node<T>>();
            this.ChildrenValues = values;
        }

        //Add child and value, where value corresponds to the value of this node (e.g. if this node is true, go to child)
        public Node<T> AddChild(Node<T> child, T value)
        {
            Children.Add(child);
            child.Parent = this;
            ChildrenValues.Add(value);
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
            if (ChildrenValues != null)
            {
                int index = ChildrenValues.IndexOf(this.Value);
                return Children[index];
            }
            return null;
        }

        public Node<T> GetChildFromClassification(T value)
        {
            if (ChildrenValues != null)
            {
                int index = ChildrenValues.IndexOf(value);
                return Children[index];
            }
            return null;
        }
    }

	// Use this for initialization
	void Start () {
        Node<string> root = new Node<string>("This enemy's hp", null, new List<string>(){"LOW", "HIGH"});
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
        playerHP.AddChild(new Node<string>("DEFEND", null, new List<string>()), "HIGH");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
