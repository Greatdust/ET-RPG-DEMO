using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HighlightingSystem.Demo
{
	[System.Obsolete]
	public class HighlighterFlashing : MonoBehaviour
	{
		public bool seeThrough = true;
		public Color flashingStartColor = Color.blue;
		public Color flashingEndColor = Color.cyan;
		public float flashingDelay = 2.5f;
		public float flashingFrequency = 2f;
	}

	#if UNITY_EDITOR
	[System.Obsolete]
	[CustomEditor(typeof(HighlighterFlashing))]
	public class HighliterFlashingEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.HelpBox("Component deprecated. Press upgrade button to automatically convert it.", MessageType.Warning);

			if (GUILayout.Button("Upgrade", GUILayout.Height(30f)))
			{
				HighlightingUpgrade.Upgrade((target as MonoBehaviour).transform);
			}
		}
	}
	#endif
}