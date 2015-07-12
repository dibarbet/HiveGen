using UnityEngine;
using System.Collections;

public class DecisionMaker : MonoBehaviour {

    public struct Node<T>
    {
        public string name { get; set; }
        public T value { get; set; }
        public Node(string name, T value)
        {
            this.name = name;
            this.value = value;
        }
    }

	// Use this for initialization
	void Start () {
        Node<bool> attack = new Node<bool>("ATTACK", false);
        Node<bool> defend = new Node<bool>("DEFEND", false);
        Node<bool> dodge = new Node<bool>("DODGE", false);
        Node<bool> roam = new Node<bool>("ROAM", false);


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
