using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace HighlightingSystem.Demo
{
	[Serializable]
	public class RaycastEvent : UnityEvent<RaycastHit> { }

	[RequireComponent(typeof(Camera))]
	public class RaycastController : MonoBehaviour
	{
		public enum UpdateEvent
		{
			Update, 
			LateUpdate, 
			OnPreCull, 
			OnPreRender, 
			OnPostRender, 
		}

		#region Inspector Fields
		// 
		public UpdateEvent updateEvent = UpdateEvent.LateUpdate;

		// Which layers targeting ray must hit (-1 = everything)
		public LayerMask layerMask = -1;

		// Targeting ray length (-1 = infinity)
		public float rayLength = -1f;

		// Events to trigger on hover
		public RaycastEvent onHover;
		#endregion

		#region Private Fields
		// Camera component reference
		private Camera cam;
		#endregion

		#region MonoBehaviour
		// 
		void Awake()
		{
			cam = GetComponent<Camera>();
		}

		// 
		void Update()
		{
			PerformRaycast(UpdateEvent.Update);
		}

		// 
		void LateUpdate()
		{
			PerformRaycast(UpdateEvent.LateUpdate);
		}

		// 
		void OnPreCull()
		{
			PerformRaycast(UpdateEvent.OnPreCull);
		}

		// 
		void OnPreRender()
		{
			PerformRaycast(UpdateEvent.OnPreRender);
		}

		// 
		void OnPostRender()
		{
			PerformRaycast(UpdateEvent.OnPostRender);
		}
		#endregion

		#region Private Methods
		// 
		private void PerformRaycast(UpdateEvent currentEvent)
		{
			if (currentEvent != updateEvent) { return; }

			if (cam == null) { return; }

			if (onHover == null) { return; }

			RaycastHit hitInfo;

			// Create a ray from mouse coords
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);

			// Targeting raycast
			if (Physics.Raycast(ray, out hitInfo, rayLength >= 0f ? rayLength : Mathf.Infinity, layerMask.value))
			{
				onHover.Invoke(hitInfo);
			}
		}
		#endregion
	}
}