using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public enum CharState
{ 
    Work,
    Sleep,
    Eat
}

public class Navigation : MonoBehaviour
{
    public CharState charState = CharState.Work;
    public Movement movement;

    public Node startNode;
    public Node endNode;
    private Node currentNode;
    public Node workNode;
    public Node sleepNode;
    public Node eatNode;

    public List<Node> path;
    List<Node> unvisitedNodes = new List<Node>();
    List<Node> visitedNodes = new List<Node>();
    public List<Node> allNodes = new List<Node>();

    public bool finalNodeFound = false;

    public int money;
    public int energy;
    public int hunger;

    int loopCount = 0;

    private void Start()
    {
        CalculatePath();
    }

    void FixedUpdate()
    {
        switch (charState)
        {
            case CharState.Work:
                Work();
                break;
            case CharState.Sleep:
                Sleep();
                break;
            case CharState.Eat:
                Eat();
                break;
        }

        switch (charState)
        {
            case CharState.Work:
                money += 6;
                energy -= 4;
                hunger -= 6;
                break;
            case CharState.Sleep:
                money -= 2;
                energy += 7;
                hunger -= 1;
                break;
            case CharState.Eat:
                money -= 6;
                energy -= 1;
                hunger += 10;
                break;
        }

        switch (charState)
        {
            case CharState.Work:
                if (hunger < 20)
                {
                    finalNodeFound = true;
                    charState = CharState.Eat;
                }
                else if (energy < 20)
                {
                    finalNodeFound = true;
                    charState = CharState.Sleep;
                }
                break;
            case CharState.Sleep:
                if (energy >= 100)
                {
                    finalNodeFound = true;
                    energy = 100;
                    charState = CharState.Work;
                }
                else if (hunger <= 10)
                {
                    finalNodeFound = true;
                    charState = CharState.Eat;
                }
                else if (money <= 10)
                {
                    finalNodeFound = true;
                    charState = CharState.Work;
                }
                break;
            case CharState.Eat:
                if (hunger >= 100)
                {
                    finalNodeFound = true;
                    hunger = 100;
                    charState = CharState.Work;
                }
                else if (money <= 10)
                {
                    finalNodeFound = true;
                    charState = CharState.Work;
                }
                else if (energy <= 10)
                {
                    finalNodeFound = true;
                    charState = CharState.Sleep;
                }
                break;
            default:
                charState = CharState.Work;
                break;
        }
    }

    private void Work()
    {
        if (finalNodeFound == true)
        {
            ResetNodes();
            startNode = endNode;
            endNode = workNode;
            CalculatePath();
        }
    }

    private void Sleep()
    {
        if (finalNodeFound == true)
        {
            ResetNodes();
            startNode = endNode;
            endNode = sleepNode;
            CalculatePath();
        }
    }

    private void Eat()
    {
        if (finalNodeFound == true)
        {
            ResetNodes();
            startNode = endNode;
            endNode = eatNode;
            CalculatePath();
        }
    }

    public void ResetNodes()
    {
        movement.enabled = false;
        finalNodeFound = false;

        foreach (Node node in allNodes)
        {
            node.ResetNode();
        }

        visitedNodes.Clear();
        path.Clear();
    }

    private void CalculatePath()
    {
        //setup
        startNode.distance = 0;
        unvisitedNodes.Add(startNode);
        float endNodeDistance = Vector3.Distance(startNode.transform.position, endNode.transform.position);

        //loop
        while (unvisitedNodes.Count > 0 && loopCount < 1000)
        {
            currentNode = unvisitedNodes[0];

            if (unvisitedNodes[0] == endNode)
            {
                visitedNodes.Add(currentNode);
                unvisitedNodes.Remove(currentNode);
                continue;
            }

            List<NodeTuples> neighbours = currentNode.neighbours;

            foreach (NodeTuples tuple in neighbours)
            {
                if (visitedNodes.Contains(tuple.node))
                    continue;

                float heuristic = Vector3.Distance(currentNode.transform.position, endNode.transform.position);

                if (endNodeDistance < heuristic)
                    continue;

                float dist = currentNode.distance + tuple.weight + heuristic;

                if (tuple.node.distance > dist)
                {
                    tuple.node.distance = dist;
                    tuple.node.prevNode = currentNode;
                }

                if (!unvisitedNodes.Contains(tuple.node))
                    unvisitedNodes.Add(tuple.node);
            }

            visitedNodes.Add(currentNode);
            unvisitedNodes.Remove(currentNode);

            unvisitedNodes.OrderBy(n => n.distance);
            loopCount++;
        }
        loopCount = 0;
        
        Node nodeCheck = endNode;

        while (nodeCheck.prevNode != null && loopCount < 100)
        {
            path.Add(nodeCheck);
            nodeCheck = nodeCheck.prevNode;
            loopCount++;
        }
        path.Add(startNode);
        loopCount = 0;
        movement.enabled = true;
        movement.nodesInPath = path.Count - 1;
    }
}