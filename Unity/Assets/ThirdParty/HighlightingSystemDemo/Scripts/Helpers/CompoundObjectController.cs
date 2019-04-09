using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;

namespace HighlightingSystem.Demo
{
	public class CompoundObjectController : MonoBehaviour
	{
		static private readonly GUIContent labelAddObject = new GUIContent("Add Object");
		static private readonly GUIContent labelRemoveObject = new GUIContent("Remove Object");
		static private readonly GUIContent labelChangeMaterial = new GUIContent("Change Material");
		static private readonly GUIContent labelChangeShader = new GUIContent("Change Shader");

		// Reference to meshes and shaders
		public Mesh[] meshes;
		public Shader[] shaders;

		// Cached transform component
		private Transform tr;
		
		// Cached list of child objects
		private List<GameObject> objects;
		
		private int shaderIndex = 0;

		private Highlighter h;

		#region MonoBehaviour
		// 
		void OnEnable()
		{
			UIManager.Register(DrawGUI, 5);
		}

		// 
		void OnDisable()
		{
			UIManager.Unregister(DrawGUI);
		}

		// 
		void Start()
		{
			tr = GetComponent<Transform>();
			objects = new List<GameObject>();
			h = GetComponent<Highlighter>();
		}
		#endregion

		#region Public Methods
		// 
		public void DrawGUI()
		{
			Camera cam = Camera.main;
			Vector3 anchor = cam.WorldToScreenPoint(transform.position + new Vector3(0f, 0f, -3f));

			if (anchor.z < 0f) { return; }

			var style = UI.skin.button;

			var size = style.CalcSize(labelAddObject);
			size = Vector2.Max(size, style.CalcSize(labelRemoveObject));
			size = Vector2.Max(size, style.CalcSize(labelChangeMaterial));
			size = Vector2.Max(size, style.CalcSize(labelChangeShader));
			size.x += 40f;

			size.y = 30f;

			Rect position = new Rect(anchor.x, anchor.y, size.x, size.y);

			// Center
			position.x -= 0.5f * size.x;
			position.y += 0.5f * (4f * size.y + 3f * UI.spacing.y);

			// 
			position.y = Screen.height - position.y;

			if (UI.Button(position, labelAddObject)) { AddObject(); }

			position.y += size.y + UI.spacing.y;
			if (UI.Button(position, labelRemoveObject)) { RemoveObject(); }

			position.y += size.y + UI.spacing.y;
			if (UI.Button(position, labelChangeMaterial)) { ChangeMaterial(); }

			position.y += size.y + UI.spacing.y;
			if (UI.Button(position, labelChangeShader)) { ChangeShader(); }
		}
		#endregion

		#region Private Methods
		// 
		private void AddObject()
		{
			Mesh m = meshes[Random.Range(0, meshes.Length)];
			GameObject o = new GameObject("SubObject");
			MeshFilter mf = o.AddComponent<MeshFilter>();
			mf.mesh = m;
			MeshCollider mc = o.AddComponent<MeshCollider>();
			mc.sharedMesh = m;
			MeshRenderer mr = o.AddComponent<MeshRenderer>();
			mr.material = new Material(shaders[0]);
			Transform t = o.GetComponent<Transform>();
			t.parent = tr;
			t.localPosition = Random.insideUnitSphere * 2f;
			objects.Add(o);
			
			// Reinitialize highlighting materials, because child objects have changed
			h.SetDirty();
		}

		// 
		private void ChangeMaterial()
		{
			if (objects.Count < 1) { AddObject(); }

			shaderIndex++;
			if (shaderIndex >= shaders.Length) { shaderIndex = 0; }
			Shader shader = shaders[shaderIndex];

			foreach (GameObject obj in objects)
			{
				Renderer renderer = obj.GetComponent<Renderer>();
				renderer.material = new Material(shader);
			}
			
			// Reinitialize highlightable materials, because material(s) have changed
			h.SetDirty();
		}

		// 
		private void ChangeShader()
		{
			if (objects.Count < 1) { AddObject(); }

			shaderIndex++;
			if (shaderIndex >= shaders.Length) { shaderIndex = 0; }
			Shader shader = shaders[shaderIndex];

			foreach (GameObject obj in objects)
			{
				Renderer renderer = obj.GetComponent<Renderer>();
				renderer.material.shader = shader;
			}
			
			// Reinitialize highlightable materials, because shader(s) have changed
			h.SetDirty();
		}

		// 
		private void RemoveObject()
		{
			if (objects.Count < 1) { return; }
			
			GameObject toRemove = objects[objects.Count - 1];
			objects.Remove(toRemove);
			Destroy(toRemove);
			
			// Reinitialize highlighting materials, because child objects have changed
			h.SetDirty();
		}
		#endregion
	}
}