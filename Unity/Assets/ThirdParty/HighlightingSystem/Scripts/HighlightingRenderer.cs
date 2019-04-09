using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace HighlightingSystem
{
	[AddComponentMenu("Highlighting System/Highlighting Renderer", 2)]
	public class HighlightingRenderer : HighlightingBase
	{
		#region Static Fields
		static public readonly List<HighlightingPreset> defaultPresets = new List<HighlightingPreset>()
		{
			new HighlightingPreset() { name = "Default",	fillAlpha = 0f,		downsampleFactor = 4,	iterations = 2,	blurMinSpread = 0.65f,	blurSpread = 0.25f, blurIntensity = 0.3f,	blurDirections = BlurDirections.Diagonal }, 
			new HighlightingPreset() { name = "Wide",		fillAlpha = 0f,		downsampleFactor = 4,	iterations = 4,	blurMinSpread = 0.65f,	blurSpread = 0.25f, blurIntensity = 0.3f,	blurDirections = BlurDirections.Diagonal }, 
			new HighlightingPreset() { name = "Strong",		fillAlpha = 0f,		downsampleFactor = 4,	iterations = 2,	blurMinSpread = 0.5f,	blurSpread = 0.15f,	blurIntensity = 0.325f,	blurDirections = BlurDirections.Diagonal }, 
			new HighlightingPreset() { name = "Speed",		fillAlpha = 0f,		downsampleFactor = 4,	iterations = 1,	blurMinSpread = 0.75f,	blurSpread = 0f,	blurIntensity = 0.35f,	blurDirections = BlurDirections.Diagonal }, 
			new HighlightingPreset() { name = "Quality",	fillAlpha = 0f,		downsampleFactor = 2,	iterations = 3,	blurMinSpread = 0.5f,	blurSpread = 0.5f,	blurIntensity = 0.28f,	blurDirections = BlurDirections.Diagonal }, 
			new HighlightingPreset() { name = "Solid 1px",	fillAlpha = 0f,		downsampleFactor = 1,	iterations = 1,	blurMinSpread = 1f,		blurSpread = 0f,	blurIntensity = 1f,		blurDirections = BlurDirections.All }, 
			new HighlightingPreset() { name = "Solid 2px",	fillAlpha = 0f,		downsampleFactor = 1,	iterations = 2,	blurMinSpread = 1f,		blurSpread = 0f,	blurIntensity = 1f,		blurDirections = BlurDirections.All }
		};
		#endregion

		#region Public Fields
		public ReadOnlyCollection<HighlightingPreset> presets
		{
			get
			{
				if (_presetsReadonly == null)
				{
					_presetsReadonly = _presets.AsReadOnly();
				}
				return _presetsReadonly;
			}
		}
		#endregion

		#region Private Fields
		[SerializeField]
		private List<HighlightingPreset> _presets = new List<HighlightingPreset>(defaultPresets);

		private ReadOnlyCollection<HighlightingPreset> _presetsReadonly;
		#endregion

		#region Public Methods
		// Get stored preset by name
		public bool GetPreset(string name, out HighlightingPreset preset)
		{
			for (int i = 0; i < _presets.Count; i++)
			{
				HighlightingPreset p = _presets[i];
				if (p.name == name)
				{
					preset = p;
					return true;
				}
			}
			preset = new HighlightingPreset();
			return false;
		}

		// Add (store) preset
		public bool AddPreset(HighlightingPreset preset, bool overwrite)
		{
			for (int i = 0; i < _presets.Count; i++)
			{
				HighlightingPreset p = _presets[i];
				if (p.name == preset.name)
				{
					if (overwrite)
					{
						_presets[i] = preset;
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			_presets.Add(preset);
			return true;
		}

		// Find stored preset by name and remove it
		public bool RemovePreset(string name)
		{
			for (int i = 0; i < _presets.Count; i++)
			{
				HighlightingPreset p = _presets[i];
				if (p.name == name)
				{
					_presets.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		// Find stored preset by name and apply it's settings
		public bool LoadPreset(string name)
		{
			HighlightingPreset preset;
			if (GetPreset(name, out preset))
			{
				ApplyPreset(preset);
			}
			return false;
		}

		// Apply preset settings
		public void ApplyPreset(HighlightingPreset preset)
		{
			downsampleFactor = preset.downsampleFactor;
			iterations = preset.iterations;
			blurMinSpread = preset.blurMinSpread;
			blurSpread = preset.blurSpread;
			blurIntensity = preset.blurIntensity;
			blurDirections = preset.blurDirections;
		}

		// 
		public void ClearPresets()
		{
			_presets.Clear();
		}
		#endregion
	}
}