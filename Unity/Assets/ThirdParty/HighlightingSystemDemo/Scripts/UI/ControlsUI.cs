using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighlightingSystem.Demo
{
	public class ControlsUI : MonoBehaviour
	{
		public TextAnchor anchor = TextAnchor.LowerLeft;

		private GUIContent labelControls = new GUIContent()
		{
			text = @"Controls: 
W, S, A, D, Q, E, C, Space, RMB drag - camera movement
LMB click - toggle tween
RMB click - toggle Normal / Overlay modes
'1' - fade in / out constant highlighting
'2' - turn on / off constant highlighting immediately
'3' - turn off all types of highlighting immediately"
		};

		// 
		void OnEnable()
		{
			UIManager.Register(DrawGUI, 4);
		}

		// 
		void OnDisable()
		{
			UIManager.Unregister(DrawGUI);
		}

		// 
		public void DrawGUI()
		{
			UI.Content(labelControls, anchor);
		}
	}
}