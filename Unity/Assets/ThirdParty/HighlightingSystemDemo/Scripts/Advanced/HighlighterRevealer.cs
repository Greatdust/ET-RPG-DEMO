using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;

namespace HighlightingSystem.Demo
{
	// Revealer for GameObjects with Highlighter components
	[DisallowMultipleComponent]
	public class HighlighterRevealer : MonoBehaviour
	{
		#region Static Fields
		static private Collider[] sColliders = new Collider[1024];
		#endregion

		#region Inspector Fields
		public float radius = 2f;
		public LayerMask layerMask = -1;
		#endregion

		#region Private Fields
		private Transform tr;
		#endregion

		#region Radius Visualization
		public Mesh sphereMesh;
		public Material sphereMaterial;

		// 
		public void DrawGizmo()
		{
			if (sphereMesh != null && sphereMaterial != null)
			{
				float s = radius * 2f;
				Matrix4x4 m = Matrix4x4.TRS(tr.position, Quaternion.identity, new Vector3(s, s, s));
				Graphics.DrawMesh(sphereMesh, m, sphereMaterial, 0);
			}
		}
		#endregion

		#region MonoBehaviour
		// 
		void Awake()
		{
			tr = GetComponent<Transform>();
		}

		// 
		void OnEnable()
		{
			HighlighterRevealerManager.Register(this);
		}

		// 
		void OnDisable()
		{
			HighlighterRevealerManager.Unregister(this);
		}

		// 
		void OnValidate()
		{
			if (radius < 0.0001f) { radius = 0.0001f; }
		}

		// 
		void OnDrawGizmos()
		{
			if (enabled)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(transform.position, radius);
			}
		}
		#endregion

		#region Public Methods
		// 
		public void UpdateNow()
		{
			DrawGizmo();

			// Collect Highlighter components in radius
			var current = HighlighterRevealerManager.current;
			var l = Physics.OverlapSphereNonAlloc(tr.position, radius, sColliders, layerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < l; i++)
			{
				var highlighter = sColliders[i].GetComponentInParent<Highlighter>();
				if (highlighter != null && !current.Contains(highlighter))
				{
					current.Add(highlighter);
				}

				// Unreference collider
				sColliders[i] = null;
			}
		}
		#endregion
	}
}