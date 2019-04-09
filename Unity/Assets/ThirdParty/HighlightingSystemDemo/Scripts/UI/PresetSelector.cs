using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HighlightingSystem;

namespace HighlightingSystem.Demo
{
	[DisallowMultipleComponent]
	public class PresetSelector : MonoBehaviour
	{
		#region Public Fields
		public TextAnchor anchor = TextAnchor.UpperRight;
		#endregion

		#region Private Fields
		private HighlightingRenderer hr;
		private UI.DropdownState dropdownState = new UI.DropdownState();
		private Camera[] allCameras;
		#endregion

		#region MonoBehaviour
		// 
		void OnEnable()
		{
			hr = FindRenderer();
			UpdateDropdown();
			UIManager.Register(DrawGUI, 2);
		}

		// 
		void OnDisable()
		{
			hr = null;
			UIManager.Unregister(DrawGUI);
		}

		// 
		void Update()
		{
			TrackChanges();
		}
		#endregion

		#region Public Methods
		// 
		public void DrawGUI()
		{
			GUI.skin = UI.skin;

			Vector2 size = new Vector2(200f, 30f);
			Rect position = new Rect(Vector2.zero, size);
			position.position = UI.Position(size, anchor);

			// Dropdown
			int selected = UI.Dropdown(position, dropdownState);
			if (selected != dropdownState.selected)
			{
				OnValueChanged(selected);
			}

			GUI.skin = null;
		}
		#endregion

		#region Private Methods
		// 
		private void OnValueChanged(int index)
		{
			var items = dropdownState.items;
			if (hr == null || index < 0 || index >= items.Count) { return; }

			dropdownState.selected = index;

			string name = items[index].text;
			hr.LoadPreset(name);
		}

		// 
		private HighlightingRenderer FindRenderer()
		{
			HighlightingRenderer result = null;

			Camera camera = Camera.main;
			if (camera != null)
			{
				result = camera.GetComponent<HighlightingRenderer>();
				if (result != null)
				{
					return result;
				}
			}

			int l = Camera.allCamerasCount;
			if (allCameras == null || allCameras.Length < l) { allCameras = new Camera[l]; }

			l = Camera.GetAllCameras(allCameras);
			for (int i = 0; i < l; i++)
			{
				camera = allCameras[i];

				// Get first HighlightingRenderer found on camera
				if (result == null)
				{
					result = camera.GetComponent<HighlightingRenderer>();
				}

				// Release reference to the camera
				allCameras[i] = null;
			}

			return result;
		}

		// 
		private void TrackChanges()
		{
			HighlightingRenderer newHr = FindRenderer();
			if (newHr != hr)
			{
				hr = newHr;
				UpdateDropdown();
			}
			else if (hr != null)
			{
				var presets = hr.presets;
				var items = dropdownState.items;
				int l = presets.Count;
				if (items.Count != l)
				{
					UpdateDropdown();
				}
				else
				{
					for (int i = 0; i < l; i++)
					{
						if (presets[i].name != items[i].text)
						{
							UpdateDropdown();
							break;
						}
					}
				}
			}
		}

		// 
		private void UpdateDropdown()
		{
			if (hr == null)
			{
				dropdownState.items.Clear();
				dropdownState.selected = -1;
				return;
			}

			// Update items
			var presets = hr.presets;
			int l = presets.Count;
			dropdownState.items.Clear();
			for (int i = 0; i < l; i++)
			{
				HighlightingPreset preset = presets[i];
				GUIContent item = new GUIContent(preset.name);
				dropdownState.items.Add(item);
			}

			// Find and select currently active preset
			for (int i = 0; i < l; i++)
			{
				HighlightingPreset preset = presets[i];
				if (hr.downsampleFactor == preset.downsampleFactor &&
					hr.iterations == preset.iterations &&
					hr.blurMinSpread == preset.blurMinSpread &&
					hr.blurSpread == preset.blurSpread &&
					hr.blurIntensity == preset.blurIntensity &&
					hr.blurDirections == preset.blurDirections)
				{
					dropdownState.selected = i;
					break;
				}
			}
		}
		#endregion
	}
}