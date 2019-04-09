using System;
using UnityEngine;

namespace HighlightingSystem
{
	[UnityEngine.Internal.ExcludeFromDocs]
	static public class MaterialExtensions
	{
		// Helper method to enable or disable keyword on material
		static public void SetKeyword(this Material material, string keyword, bool state)
		{
			if (state) { material.EnableKeyword(keyword); }
			else { material.DisableKeyword(keyword); }
		}
	}
}