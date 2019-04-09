using UnityEngine;
using System.Collections;
using HighlightingSystem;

namespace HighlightingSystem.Demo
{
	public class HighlighterToggle : MonoBehaviour
	{
		public Highlighter highlighter;
		public float delayMin = 1f;
		public float delayMax = 1f;

		private float start = 0f;
		private float delay = 0f;
		private bool state = false;

		#region MonoBehaviour
		// 
		void OnValidate()
		{
			if (delayMin < 0f) { delayMin = 0f; }
			if (delayMax < 0f) { delayMax = 0f; }
			if (delayMin > delayMax) { delayMin = delayMax; }
		}

		// 
		void Update()
		{
			float time = Time.time;
			if (time >= start + delay)
			{
				state = !state;
				start = time;
				delay = Random.Range(delayMin, delayMax);

				if (state)
				{
					Color color = Highlighter.HSVToRGB(Random.value, 1f, 1f);
					highlighter.ConstantOnImmediate(color);
				}
				else
				{
					highlighter.ConstantOffImmediate();
				}
			}
		}
		#endregion
	}
}