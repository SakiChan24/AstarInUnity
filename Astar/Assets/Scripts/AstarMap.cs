using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapNode
{
    public enum State
    {
        Pass = 0,
        Block,
    }
    
    public int X;
    public int Y;

    public State CurrState = State.Pass;
    

    public void ReverseNode()
    {
        if (CurrState == State.Pass)
        {
            CurrState = State.Block;
        }
        else if (CurrState == State.Block)
        {
            CurrState = State.Pass;
        }
    }
}


public class AstarNode
{
    public static int INDEX_OFFSET = 0;
    public int X;
    public int Y;
    public float F;


    public AstarNode(int x, int y)
    {
        X = x;
        Y = y;
    }


    public int Index
    {
        get { return X + Y * INDEX_OFFSET; }
    }

}


public class AstarNodeComparer : IComparer<AstarNode>
{
    public int Compare(AstarNode x, AstarNode y)
    {
        return -x.F.CompareTo(y.F);
    }
}



public class AstarMap
{
    public int Width = 1;
    public int Height = 1;
    public MapNode[] Grids;
    public int[][] Steps = new int[][] {new int[]{-1, 0}, new int[] { 1, 0}, new int[] { 0, -1 } , new int[] { 0, 1 }};
    //public int[][] Steps = new int[][] { new int[] { -1, 0 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { 0, 1 }, new int[] { -1, -1 }, new int[] { -1, 1 }, new int[] { 1, -1 } , new int[] {1, 1} };
    public const float DIAGONAL_COST = 1.4142135623f;


    public int GetX(int index)
    {
        return index % Width;
    }


    public int GetY(int index)
    {
        return index / Width;
    }


    public int GetIndex(int x, int y)
    {
        return x + y * Width;
    }


    public void Init(int width, int height)
    {
        Grids = new MapNode[width * height];
        for (int i = 0; i < Grids.Length; ++i)
        {
            MapNode node = new MapNode();
            node.X = this.GetX(i);
            node.Y = this.GetY(i);
            Grids[i] = node;
        }
        Width = width;
        Height = height;
        AstarNode.INDEX_OFFSET = width;
    }


    public static int HDiagonal(AstarNode node1, AstarNode node2)
    {
        return Mathf.Min(Mathf.Abs(node1.X - node2.X), Mathf.Abs(node1.Y - node2.Y));
    }


    public static int HStraight(AstarNode node1, AstarNode node2)
    {
        return Mathf.Abs(node1.X - node2.X) + Mathf.Abs(node1.Y - node2.Y);
    }

    
    public static float Heuristic(AstarNode node1, AstarNode node2)
    {
		//return 0f;
		return (Mathf.Abs(node1.X - node2.X) + Mathf.Abs(node1.Y - node2.Y)) * 1.1f;
		//return Mathf.Max(Mathf.Abs(node1.X - node2.X), Mathf.Abs(node1.Y - node2.Y));
		//return Mathf.Sqrt((node1.X - node2.X) * (node1.X - node2.X) + (node1.Y - node2.Y) * (node1.Y - node2.Y));
		//return DIAGONAL_COST * HDiagonal(node1, node2) + HStraight(node1, node2) - 2 * HDiagonal(node1, node2);

		//float absSubX = Mathf.Abs(node1.X - node2.X);
		//float absSubY = Mathf.Abs(node1.Y - node2.Y);
		//return (DIAGONAL_COST - 2f) * Mathf.Min(absSubX, absSubY) + absSubX + absSubY;
	}

    public bool CanReach(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
        {
            return false;
        }

        int index = this.GetIndex(x, y);
        MapNode mapNode = Grids[index];
        return mapNode.CurrState == MapNode.State.Pass;
    }

	public const float DELAY_DURATION = 0f;

    public IEnumerator AStarSearch(int startX, int startY, int goalX, int goalY, Action<List<MapNode>> res_callback, Action<int, int, float> push_callback = null)
    {
        //if (!this.CanReach(goalX, goalY))
        //{
        //    Debug.LogFormat("Can't reach. ");
        //    return null;
        //}


        PriorityQueue<AstarNode> q = new PriorityQueue<AstarNode>(new AstarNodeComparer());

        Dictionary<int, int> cameFrom = new Dictionary<int, int>();
        Dictionary<int, float> costSoFar = new Dictionary<int, float>();

        AstarNode startNode = new AstarNode(startX, startY);
        startNode.F = float.MaxValue;
        cameFrom[startNode.Index] = -1;
        costSoFar[startNode.Index] = 0;
        q.Push(startNode);

		if (null != push_callback)
		{
			push_callback(startNode.X, startNode.Y, startNode.F);
			yield return new WaitForSeconds(DELAY_DURATION);
		}

		AstarNode goalNode = new AstarNode(goalX, goalY);
        goalNode.F = 0;

        bool canReach = false;

        int min_h_index = startNode.Index;
        float min_h = startNode.F;

        while (q.Count > 0)
        {
            AstarNode currNode = q.Pop();

            if (currNode.Index == goalNode.Index)
            {
                canReach = true;
                break;
            }

            for (int i = 0; i < Steps.Length; ++i)
            {
                int stepX = Steps[i][0];
                int stepY = Steps[i][1];
                int nextX = currNode.X + stepX;
                int nextY = currNode.Y + stepY;

                if (! this.CanReach(nextX, nextY))
                {
                    continue;
                }

                AstarNode nextNode = new AstarNode(nextX, nextY);
                float stepCost = (0 != stepX && 0 != stepY) ? DIAGONAL_COST : 1f;
                float newCost = costSoFar[currNode.Index] + stepCost;
                if (! costSoFar.ContainsKey(nextNode.Index) || newCost < costSoFar[nextNode.Index])
                {
                    costSoFar[nextNode.Index] = newCost;
                    float h = Heuristic(nextNode, goalNode);
                    nextNode.F = newCost + h;
                    q.Push(nextNode);
					
					if (null != push_callback)
					{
						push_callback(nextNode.X, nextNode.Y, nextNode.F);
						yield return new WaitForSeconds(DELAY_DURATION);
					}
										
					cameFrom[nextNode.Index] = currNode.Index;

                    if (h < min_h)
                    {
                        min_h = h;
                        min_h_index = nextNode.Index;
                    }
                }
            }
        }

        int goalIndex = goalNode.Index;
        if (! canReach)
        {
            Debug.LogFormat("Can't reach. ");
            
            goalIndex = min_h_index;
        }

		List<MapNode> nodes = new List<MapNode>();
		for (int currNodeIndex = goalIndex; currNodeIndex != -1; currNodeIndex = cameFrom[currNodeIndex])
        {
            MapNode node = new MapNode();
            node.X = this.GetX(currNodeIndex);
            node.Y = this.GetY(currNodeIndex);
            nodes.Add(node);
        }
        nodes.Reverse();

		res_callback(nodes);

		//return nodes;
	}


    public List<MapNode> AstarBidirectionalSearch()
    {
        return null;
    }

}


