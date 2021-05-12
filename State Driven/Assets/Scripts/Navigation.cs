using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public enum CharState
{ 
    Patrol,
    Sleep,
    Chase
}

public class Navigation : MonoBehaviour
{
    public CharState charState = CharState.Patrol;
    public Movement movement;

    public GameObject enemy;
    public float enemyDistance;

    public Node startNode;
    public Node endNode;
    public Node sleepNode;
    private Node currentNode;

    public List<Node> path;
    List<Node> unvisitedNodes = new List<Node>();
    List<Node> visitedNodes = new List<Node>();
    public List<Node> allNodes = new List<Node>();

    public bool nightTime = false;
    public bool finalNodeFound = false;
    private int patrolsToDo = 0;

    int loopCount = 0;

    private void Start()
    {
        CalculatePath();
    }

    void Update()
    {
        switch (charState)
        {
            case CharState.Patrol:
                Patrol();
                break;
            case CharState.Sleep:
                Sleep();
                break;
            case CharState.Chase:
                Chase();
                break;
            default:
                charState = CharState.Patrol;
                break;
        }

        enemyDistance = Vector3.Distance(this.transform.position, enemy.transform.position);

        switch (charState)
        {
            case CharState.Patrol:
                if (enemyDistance <= 10)
                {
                    charState = CharState.Chase;
                }
                else if (nightTime == true && patrolsToDo <= 0)
                {
                    charState = CharState.Sleep;
                }
                break;
            case CharState.Sleep:
                if (nightTime == false)
                {
                    charState = CharState.Patrol;
                }
                else if (enemyDistance <= 5)
                {
                    charState = CharState.Chase;
                }
                break;
            case CharState.Chase:
                if (enemyDistance >= 13)
                {
                    patrolsToDo = 5;
                    charState = CharState.Patrol;
                }
                break;
            default:
                charState = CharState.Patrol;
                break;
        }
    }

    private void Patrol()
    {
        if (finalNodeFound == true)
        {
            ResetNodes();
            finalNodeFound = false;
            startNode = endNode;
            endNode = allNodes[Random.Range(0, allNodes.Count)];
            CalculatePath();
        }
    }

    private void Sleep()
    {
        if (finalNodeFound == true)
        {
            ResetNodes();
            finalNodeFound = false;
            startNode = endNode;
            endNode = sleepNode;
            CalculatePath();
        }
    }

    private void Chase()
    {
        finalNodeFound = false;
        movement.enabled = true;
    }

    public void ResetNodes()
    {
        movement.enabled = false;

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
        patrolsToDo--;
    }
}