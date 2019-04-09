using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using HighlightingSystem;

namespace HighlightingSystem.Demo
{
	public class HighlightingPresetLoop : MonoBehaviour
	{
		public HighlightingRenderer highlightingRenderer;

		[Range(0.1f, 2f)]
		public float interval = 1f;

		public List<HighlightingPreset> customPresets = new List<HighlightingPreset>()
		{
			new HighlightingPreset() { name = "Wide",	    downsampleFactor = 4,	iterations = 4,	blurMinSpread = 0.65f,	blurSpread = 0.25f, blurIntensity = 0.3f,	blurDirections = BlurDirections.Diagonal }, 
			new HighlightingPreset() { name = "Solid 2px",	downsampleFactor = 1,	iterations = 2,	blurMinSpread = 1f,		blurSpread = 0f,	blurIntensity = 1f,		blurDirections = BlurDirections.All }, 
		};

		// 
		IEnumerator Start()
		{
			highlightingRenderer.ClearPresets();
			for (int i = 0; i < customPresets.Count; i++)
			{
				highlightingRenderer.AddPreset(customPresets[i], false);
			}

			int index = 0;
			while (true)
			{
				ReadOnlyCollection<HighlightingPreset> presets = highlightingRenderer.presets;
				if (index >= presets.Count)
				{
					index = 0;
				}
				HighlightingPreset preset = presets[index];
				highlightingRenderer.LoadPreset(preset.name);
				index++;

				yield return new WaitForSeconds(interval);
			}
		}
	}
}