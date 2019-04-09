using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HighlightingSystem
{
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Highlighting System/Highlighting Blitter", 3)]
	public class HighlightingBlitter : MonoBehaviour
	{
		protected List<HighlightingBase> renderers = new List<HighlightingBase>();

		#region MonoBehaviour
		// 
		protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			bool oddEven = true;
			for (int i = 0; i < renderers.Count; i++)
			{
				HighlightingBase renderer = renderers[i];
				if (oddEven)
				{
					renderer.Blit(src, dst);
				}
				else
				{
					renderer.Blit(dst, src);
				}

				oddEven = !oddEven;
			}

			// Additional blit because final result should be in dst RenderTexture
			if (oddEven)
			{
				Graphics.Blit(src, dst);
			}
		}
		#endregion

		#region Public Methods
		// 
		[UnityEngine.Internal.ExcludeFromDocs]
		public virtual void Register(HighlightingBase renderer)
		{
			if (!renderers.Contains(renderer))
			{
				renderers.Add(renderer);
			}

			enabled = renderers.Count > 0;
		}
		
		// 
		[UnityEngine.Internal.ExcludeFromDocs]
		public virtual void Unregister(HighlightingBase renderer)
		{
			int index = renderers.IndexOf(renderer);
			if (index != -1)
			{
				renderers.RemoveAt(index);
			}

			enabled = renderers.Count > 0;
		}
		#endregion
	}
}