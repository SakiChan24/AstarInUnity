using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapItem : MonoBehaviour
{
	private MapNode m_Node;
	private Button m_Button;
	private Image m_Image;

	private void Awake()
	{
		m_Button = this.GetComponent<Button>();
		m_Image = this.GetComponent<Image>();

		m_Button.onClick.AddListener(this._OnButtonClick);
	}

	public void Init(MapNode node)
	{
		m_Node = node;
	}

	private void _OnButtonClick()
	{
		this.ReverseNode();
	}


	public void ReverseNode()
	{
		m_Node.ReverseNode();
		this.RefreshItem();
	}

	public void RefreshItem()
	{
		if (m_Node.CurrState == MapNode.State.Block)
		{
			m_Image.color = Color.black;
		}
		else
		{
			m_Image.color = Color.white;
		}
	}

	public void DrawPathNode()
	{
		m_Image.color = Color.red;
	}

	public void DrawPushNode(float f)
	{
		m_Image.color = new Color(0f, 1f, Mathf.Clamp01(f / 180f));
	}
}
