using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighlightingSystem.Demo
{
	public class UIManager : MonoBehaviour
	{
		private class GUIRenderer
		{
			public Action callback;
			public int depth;
		}

		private class GUIRendererComparer : IComparer<GUIRenderer>
		{
			#region IComparer implementation
			public int Compare(GUIRenderer x, GUIRenderer y)
			{
				return x.depth.CompareTo(y.depth);
			}
			#endregion
		}

		#region Static Fields
		static private UIManager _singleton;
		static private UIManager singleton
		{
			get
			{
				if (_singleton == null)
				{
					GameObject go = new GameObject("UIManager");
					go.hideFlags = HideFlags.HideAndDontSave;
					_singleton = go.AddComponent<UIManager>();
				}
				return _singleton;
			}
		}
		#endregion

		#region Private Fields
		private List<GUIRenderer> guiRenderers = new List<GUIRenderer>();
		private GUIRendererComparer comparer = new GUIRendererComparer();
		private bool isDirty = false;
		#endregion

		#region Private Methods
		// 
		private void RegisterInternal(Action action, int depth)
		{
			if (action == null) { return; }

			GUIRenderer guiRenderer = new GUIRenderer()
			{
				depth = depth, 
				callback = action, 
			};
			guiRenderers.Add(guiRenderer);
			isDirty = true;
		}

		// 
		private void UnregisterInternal(Action action)
		{
			for (int i = guiRenderers.Count - 1; i >= 0; i--)
			{
				GUIRenderer guiRenderer = guiRenderers[i];
				if (guiRenderer.callback == action)
				{
					guiRenderers.RemoveAt(i);
				}
			}
		}
		#endregion

		#region Static Methods
		// 
		static public void Register(Action action, int depth)
		{
			singleton.RegisterInternal(action, depth);
		}

		// 
		static public void Unregister(Action action)
		{
			if (_singleton != null)
			{
				_singleton.UnregisterInternal(action);
			}
		}
		#endregion

		#region MonoBehaviour
		// 
		void Awake()
		{
			// Sacrifice GUILayout.* and GUI.depth to prevent garbage allocations
			useGUILayout = false;
		}

		// 
		void OnGUI()
		{
			if (isDirty)
			{
				isDirty = false;

				// Sort by depth
				guiRenderers.Sort(comparer);
			}

			// Draw back-to-front
			for (int i = guiRenderers.Count - 1; i >= 0; i--)
			{
				var guiRenderer = guiRenderers[i];
				if (guiRenderer.callback != null)
				{
					guiRenderer.callback();
				}
				else
				{
					guiRenderers.RemoveAt(i);
				}
			}
		}
		#endregion
	}
}
