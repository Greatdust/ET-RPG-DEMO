using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

namespace HighlightingSystem.Demo
{
	public class HighlighterFilterAdvanced : MonoBehaviour
	{
		public Highlighter highlighter;
		public Renderer highlightableRenderer;
		public float duration = 0.2f;

		private int index = 0;
		private int submeshCount;
		private int pow;

		// 
		void Awake()
		{
			submeshCount = highlightableRenderer.sharedMaterials.Length;
			// Count to this number in binary
			pow = (int)Math.Pow(2.0, submeshCount);
		}

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
		void Update()
		{
			int t = duration > 0f ? (int)(Time.time / duration) : 0;
			int newIndex = t - (t / pow) * pow;
			if (index != newIndex)
			{
				// Reinitialize highlighted materials if index have changed
				index = newIndex;
				if (highlighter != null) { highlighter.SetDirty(); }
			}
		}

		// 
		private bool Filter(Renderer renderer, List<int> submeshIndices)
		{
			if (renderer != highlightableRenderer) { return false; }

			for (int i = 0; i < submeshCount; i++)
			{
				if ((index & (1 << i)) != 0)
				{
					submeshIndices.Add(i);
				}
			}

			return true;
		}
	}
}