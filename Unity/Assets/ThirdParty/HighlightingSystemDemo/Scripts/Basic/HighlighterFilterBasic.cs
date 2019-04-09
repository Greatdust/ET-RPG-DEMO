using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

namespace HighlightingSystem.Demo
{
	public class HighlighterFilterBasic : MonoBehaviour
	{
		public Highlighter highlighter;
		public List<Renderer> highlightableRenderers;

		// 
		void OnEnable()
		{
			if (highlighter != null) { highlighter.rendererFilter = Filter; }
		}

		// 
		void OnDisable()
		{
			if (highlighter != null) { highlighter.rendererFilter = null; }
		}

		// 
		private bool Filter(Renderer renderer, List<int> submeshIndices)
		{
			if (!highlightableRenderers.Contains(renderer)) { return false; }

			// Highlight all submeshes
			submeshIndices.Add(-1);

			return true;
		}
	}
}