using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DecisionMaker : MonoBehaviour {

    public class Node<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }

        public Node<T> Parent { get; set; }
        public List<Node<T>> Children { get; private set; }

        public Node(string name, T value, Node<T> parent)
        {
            this.Name = name;
            this.Value = value;
            this.Parent = parent;
            this.Children = new List<Node<T>>();
        }

        public void AddChild(Node<T> child)
        {
            Children.Add(child);
            child.Parent = this;
        }
    }

	// Use this for initialization
	void Start () {
        Node<bool> attack = new Node<bool>("ATTACK", false, null);
        Node<bool> defend = new Node<bool>("DEFEND", false, null);
        Node<bool> dodge = new Node<bool>("DODGE", false, null);
        Node<bool> roam = new Node<bool>("ROAM", false, null);


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
