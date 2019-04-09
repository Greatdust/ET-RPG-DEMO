using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HighlightingSystem.Demo
{
	[System.Obsolete]
	public class HighlighterConstant : MonoBehaviour
	{
		public bool seeThrough = true;
		public Color color = Color.cyan;
	}

	#if UNITY_EDITOR
	[System.Obsolete]
	[CustomEditor(typeof(HighlighterConstant))]
	public class HighliterConstantEditor : Editor
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