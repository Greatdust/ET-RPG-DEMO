using System;
using UnityEngine;
using HighlightingSystem;

namespace HighlightingSystem.Demo
{
	public class HighlighterHover : MonoBehaviour
	{
		// Hover color
		public Color hoverColor = Color.red;

		// RaycastController should trigger this method via onHover event
		public void OnHover(RaycastHit hitInfo)
		{
			Transform tr = hitInfo.collider.transform;
			if (tr == null) { return; }

			var highlighter = tr.GetComponentInParent<Highlighter>();
			if (highlighter == null) { return; }

			// Hover
			highlighter.Hover(hoverColor);
		}
	}
}