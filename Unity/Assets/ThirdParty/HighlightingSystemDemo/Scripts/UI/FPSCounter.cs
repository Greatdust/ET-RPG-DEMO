using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HighlightingSystem.Demo
{
	public class FPSCounter : MonoBehaviour
	{
		#region Constants
		private const int updateFrames = 100;
		#endregion

		#region Public Fields
		public TextAnchor anchor = TextAnchor.LowerRight;
		#endregion

		#region Private Fields
		private GUIContent content = new GUIContent();
		private Font font;

		private List<float> frameTimes = new List<float>(updateFrames);
		private float sum = 0f;

		private float i = 0f;
		#endregion

		// 
		void OnEnable()
		{
			UIManager.Register(DrawGUI, 3);
		}

		// 
		void OnDisable()
		{
			UIManager.Unregister(DrawGUI);
		}

		// 
		void Update()
		{
			float t = Time.deltaTime;
			sum += t;
			frameTimes.Add(t);

			int l = frameTimes.Count;
			if (l == updateFrames)
			{
				frameTimes.Sort();

				int n = updateFrames / 2;
				
				float medianDeltaTime;
				// even
				if (updateFrames - n * 2 == 0)
				{
					medianDeltaTime = (frameTimes[n - 1] + frameTimes[n]) * 0.5f;
				}
				// odd
				else
				{
					medianDeltaTime = frameTimes[n];
				}
				
				float avg = ((float)l / sum);			// average fps value
				float med = 1f / medianDeltaTime;		// half of the frames were above this fps value

				i++;

				content.text = string.Format("FPS:\n{0:f2} (average)\n{1:f2} (median)", avg, med);

				frameTimes.Clear();
				sum = 0f;
			}
		}

		// 
		public void DrawGUI()
		{
			UI.Content(content, anchor);
		}
	}
}