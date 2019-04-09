#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor;
using HighlightingSystem;
using Highlighter = HighlightingSystem.Highlighter;

namespace HighlightingSystem.Demo
{
	[UnityEngine.Internal.ExcludeFromDocs]
	public class HighlightingUpgrade : EditorWindow
	{
		[MenuItem("Tools/Highlighting System/Upgrade Scene from v4.3 to v5.0", priority = 1)]
		static private void Initialize()
		{
			HighlightingUpgrade window = EditorWindow.GetWindow<HighlightingUpgrade>();
			window.Show();
		}

		// 
		void OnGUI()
		{
			if (GUILayout.Button("Upgrade Current Scene", GUILayout.Height(30f)))
			{
				var scene = SceneManager.GetActiveScene();
				var gameObjects = scene.GetRootGameObjects();
				for (int i = 0; i < gameObjects.Length; i++)
				{
					var go = gameObjects[i];
					UpgradeRecursively(go.transform);
				}
				EditorSceneManager.MarkSceneDirty(scene);
			}
		}

		#region Static Methods
		// 
		static public void Upgrade(Transform tr)
		{
			if (tr == null) { return; }

			UpgradeSpectrum(tr);
			UpgradeFlashing(tr);
			UpgradeConstant(tr);

			UpgradeOccluder(tr);
			UpgradeInteractive(tr);
			UpgradeBase(tr);
		}

		// 
		static private void UpgradeRecursively(Transform tr)
		{
			// Self
			Upgrade(tr);

			// Children
			for (int i = 0; i < tr.childCount; i++)
			{
				var child = tr.GetChild(i);
				UpgradeRecursively(child);
			}
		}

		// 
		static private void UpgradeSpectrum(Transform tr)
		{
			#pragma warning disable 0612
			var spectrum = tr.GetComponent<HighlighterSpectrum>();
			#pragma warning restore 0612
			if (spectrum == null) { return; }

			bool random = spectrum.random;
			float velocity = spectrum.velocity;
			bool seeThrough = spectrum.seeThrough;

			Undo.DestroyObjectImmediate(spectrum);

			var highlighter = EnsureHighlighter(tr);
			highlighter.tween = true;
			highlighter.tweenGradient = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(1f, 0f, 0f, 1f), 0f / 6f), 
					new GradientColorKey(new Color(1f, 1f, 0f, 1f), 1f / 6f), 
					new GradientColorKey(new Color(0f, 1f, 0f, 1f), 2f / 6f), 
					new GradientColorKey(new Color(0f, 1f, 1f, 1f), 3f / 6f), 
					new GradientColorKey(new Color(0f, 0f, 1f, 1f), 4f / 6f), 
					new GradientColorKey(new Color(1f, 0f, 1f, 1f), 5f / 6f), 
					new GradientColorKey(new Color(1f, 0f, 0f, 1f), 6f / 6f), 
				}, 
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(1f, 0f), 
					new GradientAlphaKey(1f, 1f), 
				}
			};
			if (velocity <= 0f) { Debug.LogError("HighlighterSpectrum velocity <= 0f", tr); }
			highlighter.tweenDuration = 1f / velocity;
			highlighter.tweenDelay = random ? -Random.value * highlighter.tweenDuration : 0f;
			highlighter.tweenEasing = Easing.Linear;
			highlighter.tweenLoop = LoopMode.Loop;
			highlighter.tweenRepeatCount = -1;
			highlighter.tweenReverse = false;
			highlighter.tweenUseUnscaledTime = false;
			highlighter.overlay = seeThrough;

			Debug.Log("Upgraded HighlighterSpectrum", highlighter);
		}

		// 
		static private void UpgradeFlashing(Transform tr)
		{
			#pragma warning disable 0612
			var flashing = tr.GetComponent<HighlighterFlashing>();
			#pragma warning restore 0612
			if (flashing == null) { return; }

			Color startColor = flashing.flashingStartColor;
			Color endColor = flashing.flashingEndColor;
			float delay = flashing.flashingDelay;
			float frequency = flashing.flashingFrequency;
			bool seeThrough = flashing.seeThrough;

			Undo.DestroyObjectImmediate(flashing);

			var highlighter = EnsureHighlighter(tr);
			highlighter.tween = true;
			highlighter.tweenGradient = new Gradient()
			{
				colorKeys = new GradientColorKey[]
				{
					new GradientColorKey(new Color(startColor.r, startColor.g, startColor.b, 1f), 0f), 
					new GradientColorKey(new Color(endColor.r, endColor.g, endColor.b, 1f), 1f)
				}, 
				alphaKeys = new GradientAlphaKey[]
				{
					new GradientAlphaKey(startColor.a, 0f), 
					new GradientAlphaKey(endColor.a, 1f), 
				}
			};
			highlighter.tweenDuration = 1f / frequency;
			highlighter.tweenDelay = delay;
			highlighter.tweenEasing = Easing.Linear;
			highlighter.tweenLoop = LoopMode.PingPong;
			highlighter.tweenRepeatCount = -1;
			highlighter.tweenReverse = false;
			highlighter.tweenUseUnscaledTime = false;
			highlighter.overlay = seeThrough;

			Debug.Log("Upgraded HighlighterFlashing", highlighter);
		}

		// 
		static private void UpgradeConstant(Transform tr)
		{
			#pragma warning disable 0612
			var constant = tr.GetComponent<HighlighterConstant>();
			#pragma warning restore 0612
			if (constant == null) { return; }

			var color = constant.color;
			var seeThrough = constant.seeThrough;

			Undo.DestroyObjectImmediate(constant);

			var highlighter = EnsureHighlighter(tr);
			highlighter.constant = true;
			highlighter.constantColor = color;
			highlighter.overlay = seeThrough;

			Debug.Log("Upgraded HighlighterConstant", highlighter);
		}

		// 
		static private void UpgradeOccluder(Transform tr)
		{
			#pragma warning disable 0612
			var occluder = tr.GetComponent<HighlighterOccluder>();
			#pragma warning restore 0612
			if (occluder == null) { return; }

			bool seeThrough = occluder.seeThrough;

			Undo.DestroyObjectImmediate(occluder);

			if (seeThrough)
			{
				var highlighter = EnsureHighlighter(tr);
				highlighter.occluder = true;

				Debug.Log("Upgraded HighlighterOccluder", highlighter);
			}
			else
			{
				// Do nothing
				Debug.Log("Upgraded HighlighterOccluder (Removed)", tr);
			}
		}

		// 
		static private void UpgradeInteractive(Transform tr)
		{
			#pragma warning disable 0612
			var interactive = tr.GetComponent<HighlighterInteractive>();
			#pragma warning restore 0612
			if (interactive == null) { return; }

			bool seeThrough = interactive.seeThrough;

			Undo.DestroyObjectImmediate(interactive);

			var highlighter = EnsureHighlighter(tr);
			highlighter.overlay = seeThrough;

			Debug.Log("Upgraded HighlighterInteractive", highlighter);
		}

		// 
		static private void UpgradeBase(Transform tr)
		{
			#pragma warning disable 0612
			var @base = tr.GetComponent<HighlighterBase>();
			#pragma warning restore 0612
			if (@base == null) { return; }

			bool seeThrough = @base.seeThrough;

			Undo.DestroyObjectImmediate(@base);

			var highlighter = EnsureHighlighter(tr);
			highlighter.overlay = seeThrough;

			Debug.Log("Upgraded HighlighterBase", highlighter);
		}

		// 
		static private Highlighter EnsureHighlighter(Transform tr)
		{
			var highlighter = tr.GetComponent<Highlighter>();
			if (highlighter == null)
			{
				highlighter = Undo.AddComponent<Highlighter>(tr.gameObject);
			}
			return highlighter;
		}
		#endregion
	}
}
#endif