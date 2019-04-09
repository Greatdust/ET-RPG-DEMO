using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
#endif

namespace HighlightingSystem.Demo
{
	[DisallowMultipleComponent]
	public class SceneLoader : MonoBehaviour
	{
		static private readonly KeyCode[] keysNext = new KeyCode[] { KeyCode.PageDown, KeyCode.Joystick1Button3, KeyCode.Joystick2Button3, KeyCode.Joystick3Button3, KeyCode.Joystick4Button3, KeyCode.Joystick5Button3, KeyCode.Joystick6Button3, KeyCode.Joystick7Button3, KeyCode.Joystick8Button3 };
		static private readonly KeyCode[] keysPrev = new KeyCode[] { KeyCode.PageUp, KeyCode.Joystick1Button2, KeyCode.Joystick2Button2, KeyCode.Joystick3Button2, KeyCode.Joystick4Button2, KeyCode.Joystick5Button2, KeyCode.Joystick6Button2, KeyCode.Joystick7Button2, KeyCode.Joystick8Button2 };

		#region Serialized Fields
		[HideInInspector]
		public List<string> sceneNames = new List<string>();
		#endregion

		#region Public Fields
		public TextAnchor anchor = TextAnchor.UpperLeft;
		#endregion

		#region Private Fields
		private UI.DropdownState dropdownState = new UI.DropdownState();
		#endregion

		#if UNITY_EDITOR
		//
		[PostProcessSceneAttribute(2)]
		static public void OnPostProcessScene()
		{
			EditorBuildSettingsScene[] existingScenes = EditorBuildSettings.scenes;

			List<string> sceneNames = new List<string>();
			for (int i = 0, l = existingScenes.Length; i < l; i++)
			{
				EditorBuildSettingsScene scene = existingScenes[i];
				if (scene.enabled)
				{
					sceneNames.Add(Path.GetFileNameWithoutExtension(scene.path));
				}
			}

			SceneLoader[] sceneLoaders = FindObjectsOfType<SceneLoader>();
			for (int i = 0, l = sceneLoaders.Length; i < l; i++)
			{
				sceneLoaders[i].sceneNames = sceneNames;
			}
		}
		#endif

		#region MonoBehaviour
		// 
		void OnEnable()
		{
			UIManager.Register(DrawGUI, 1);
		}

		// 
		void OnDisable()
		{
			UIManager.Unregister(DrawGUI);
		}

		//
		void Start()
		{
			dropdownState.selected = -1;

			// Fill scenes dropdown list
			dropdownState.items.Clear();
			string activeSceneName = SceneManager.GetActiveScene().name;
			sceneNames.Sort();
			for (int i = 0, l = sceneNames.Count; i < l; i++)
			{
				string sceneName = sceneNames[i];

				GUIContent item = new GUIContent(sceneName);
				dropdownState.items.Add(item);
				
				if (dropdownState.selected == -1 && activeSceneName == sceneName)
				{
					dropdownState.selected = i;
				}
			}
		}

		// 
		void Update()
		{
			int input = InputHelper.GetKeyDown(keysNext, keysPrev);
			if (input > 0) { OnNext(); }
			else if (input < 0) { OnPrevious(); }
		}
		#endregion

		#region Public Methods
		// 
		public void DrawGUI()
		{
			GUI.skin = UI.skin;

			float dropdownWidth = 200f;
			float buttonWidth = 30f;
			float commonHeight = 30f;

			Rect position = new Rect(0f, 0f, dropdownWidth, commonHeight);
			Vector2 totalSize = new Vector2(dropdownWidth + 2f * (UI.spacing.x + buttonWidth), commonHeight);
			position.position = UI.Position(totalSize, anchor);

			// Dropdown
			int selected = UI.Dropdown(position, dropdownState);
			if (selected != dropdownState.selected)
			{
				OnValueChanged(selected);
			}

			bool cachedEnabled;

			// Previous
			cachedEnabled = GUI.enabled;
			GUI.enabled = dropdownState.selected > 0;
			position = new Rect(position.x + position.width + UI.spacing.x, position.y, buttonWidth, commonHeight);
			if (UI.Button(position, UI.labelArrowLeft)) { OnPrevious(); }
			GUI.enabled = cachedEnabled;

			// Next
			cachedEnabled = GUI.enabled;
			GUI.enabled = dropdownState.selected < dropdownState.items.Count - 1;
			position = new Rect(position.x + position.width + UI.spacing.x, position.y, buttonWidth, commonHeight);
			if (UI.Button(position, UI.labelArrowRight)) { OnNext(); }
			GUI.enabled = cachedEnabled;

			GUI.skin = null;
		}
		#endregion

		#region Private Methods
		//
		private void OnValueChanged(int index)
		{
			List<GUIContent> items = dropdownState.items;
			index = Mathf.Clamp(index, 0, items.Count - 1);

			dropdownState.selected = index;

			string sceneName = items[index].text;
			string activeSceneName = SceneManager.GetActiveScene().name;
			if (!string.Equals(activeSceneName, sceneName))
			{
				SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
			}
		}

		//
		private void OnPrevious()
		{
			OnValueChanged(dropdownState.selected - 1);
		}

		//
		private void OnNext()
		{
			OnValueChanged(dropdownState.selected + 1);
		}
		#endregion
	}
}