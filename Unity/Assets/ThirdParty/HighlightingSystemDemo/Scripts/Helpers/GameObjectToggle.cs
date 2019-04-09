using UnityEngine;
using System.Collections;
using HighlightingSystem;

namespace HighlightingSystem.Demo
{
	public class GameObjectToggle : MonoBehaviour
	{
		public GameObject target;
		public float delayMin = 1f;
		public float delayMax = 1f;
		
		// 
		void Start()
		{
			StartCoroutine(ToggleRoutine());
		}

		// 
		void OnValidate()
		{
			if (delayMin < 0f) { delayMin = 0f; }
			if (delayMax < 0f) { delayMax = 0f; }
			if (delayMin > delayMax) { delayMin = delayMax; }
		}

		// 
		IEnumerator ToggleRoutine()
		{
			while (true)
			{
				yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
				Toggle();
			}
		}
		
		// 
		void Toggle()
		{
			if (target != null)
			{
				target.SetActive(!target.activeSelf);
			}
		}
	}
}