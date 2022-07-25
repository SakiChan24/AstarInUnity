using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapPanel : MonoBehaviour
{
    public int Width;
    public int Height;

    public AstarMap Map = new AstarMap();
    public UIMapItem NodeItemOrg;
    public GridLayoutGroup NodesGroup;
    public Button GoButton;
    public int StartX, StartY, GoalX, GoalY;

    private UIMapItem[] m_MapItems;
    
    
    public void BuildMap(int width, int height)
    {
        Map.Init(width, height);
        NodeItemOrg.gameObject.SetActive(false);

        m_MapItems = new UIMapItem[Map.Grids.Length];

        for (int i = 0; i < m_MapItems.Length; ++i)
        {
            GameObject go = GameObject.Instantiate<GameObject>(NodeItemOrg.gameObject, NodesGroup.transform);
            UIMapItem item = go.GetComponent<UIMapItem>();
            MapNode node = Map.Grids[i];
            item.Init(node);
            m_MapItems[i] = item;

            go.SetActive(true);
        }
        NodesGroup.constraintCount = width;
    }


    public void RefreshItems()
    {
        for (int i = 0; i < m_MapItems.Length; ++i)
        {
            m_MapItems[i].RefreshItem();
        }
    }


    public void DrawPath(List<MapNode> nodes)
    {
        for (int i = 0; i < nodes.Count; ++i)
        {
            MapNode node = nodes[i];
			UIMapItem item = this.GetItem(node.X, node.Y);
			item.DrawPathNode();
        }
    }


	public void DrawPushNode(int index_x, int index_y, float f)
	{
		UIMapItem item = this.GetItem(index_x, index_y);
		item.DrawPushNode(f);
	}



	private UIMapItem GetItem(int index_x, int index_y)
	{
		int index = Map.GetIndex(index_x, index_y);
		UIMapItem item = m_MapItems[index];
		return item;
	}


    private void Awake()
    {
        this.BuildMap(Width, Height);
        GoButton.onClick.AddListener(()=>
		{
			this.RefreshItems();
			this.StopAllCoroutines();
			this.StartCoroutine(Map.AStarSearch(StartX, StartY, GoalX, GoalY, this.OnDrawPath, this.OnPushNode));
        });
    }


	private void OnDrawPath(List<MapNode> nodes)
	{
		if (null != nodes)
		{
			this.DrawPath(nodes);
		}
	}


	private void OnPushNode(int index_x, int index_y, float f)
	{
		this.DrawPushNode(index_x, index_y, f);
	}

    
}
