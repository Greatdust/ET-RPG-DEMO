using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

namespace HighlightingSystem.Demo
{
	public class HighlighterRevealerManager : MonoBehaviour
	{
		static private readonly Color color = new Color(0f, 1f, 0f, 1f);
		private const float fadeInTime = 0.2f;
		private const float fadeOutTime = 0.4f;

		static public readonly List<Highlighter> current = new List<Highlighter>();

		static private HighlighterRevealerManager manager;
		static private readonly List<HighlighterRevealer> revealers = new List<HighlighterRevealer>();
		static private readonly List<Highlighter> cached = new List<Highlighter>();

		#region Public Methods
		// 
		static public void Register(HighlighterRevealer revealer)
		{
			if (revealer == null) { return; }

			if (!revealers.Contains(revealer)) { revealers.Add(revealer); }

			// Ensure manager
			if (revealers.Count > 0 && manager == null)
			{
				GameObject go = new GameObject("HighlighterRevealerManager");
				go.hideFlags = HideFlags.HideAndDontSave;
				manager = go.AddComponent<HighlighterRevealerManager>();
			}
		}

		// 
		static public void Unregister(HighlighterRevealer revealer)
		{
			revealers.Remove(revealer);

			// Destroy manager
			if (revealers.Count == 0)
			{
				Destroy(manager.gameObject);
				manager = null;
			}
		}
		#endregion

		#region MonoBehaviour
		// After all movement finishes
		void LateUpdate()
		{
			current.Clear();

			// Update all revealers
			for (int i = revealers.Count - 1; i >= 0; i--)
			{
				var revealer = revealers[i];
				if (revealer == null)
				{
					revealers.RemoveAt(i);
					continue;
				}

				revealer.UpdateNow();
			}

			// Remove
			for (int i = cached.Count - 1; i >= 0; i--)
			{
				var highlighter = cached[i];
				if (!current.Contains(highlighter))
				{
					cached.RemoveAt(i);
					highlighter.ConstantOff(fadeOutTime);
				}
			}

			// Add
			for (int i = 0; i < current.Count; i++)
			{
				var highlighter = current[i];
				if (!cached.Contains(highlighter))
				{
					cached.Add(highlighter);
					highlighter.ConstantOn(color, fadeInTime);
				}
			}

			current.Clear();
		}

		// 
		void OnDestroy()
		{
			for (int i = 0; i < cached.Count; i++)
			{
				var highlighter = cached[i];
				highlighter.ConstantOff(fadeOutTime);
			}

			cached.Clear();
		}
		#endregion
	}
}