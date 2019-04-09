using UnityEngine;
using System.Collections;
using HighlightingSystem;

namespace HighlightingSystem.Demo
{
	public class HighlighterBasic : MonoBehaviour
	{
		public Highlighter highlighter;
		public Color color;

		// 
		void Start()
		{
			highlighter.ConstantOnImmediate(color);
		}
	}
}