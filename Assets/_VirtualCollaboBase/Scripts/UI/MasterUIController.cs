using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterUIController : MonoBehaviour
{
	[SerializeField] private GameObject m_UICanvas = null;

	private void Update()
	{
		if( Input.GetKeyDown(KeyCode.X) )
		{
			m_UICanvas.SetActive( !m_UICanvas.activeSelf );
		}
	}
}
