using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HighlightingSystem.Demo
{
	#if UNITY_EDITOR
	[CustomEditor(typeof(CameraFlyController))]
	public class CameraFlyControllerEditor : Editor
	{
		static private readonly GUIContent labelSetupAxes = new GUIContent("Setup Input Axes");

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button(labelSetupAxes, GUILayout.Height(30f)))
			{
				SetupControllerAxes();
			}
		}

		private struct Axis
		{
			public string m_Name;
			public string descriptiveName;
			public string descriptiveNegativeName;
			public string negativeButton;
			public string positiveButton;
			public string altNegativeButton;
			public string altPositiveButton;
			public float gravity;
			public float dead;
			public float sensitivity;
			public bool snap;
			public bool invert;
			public int type;  // Enum
			public int axis;  // Enum
			public int joyNum;    // Enum

			// 
			public void Serialize(SerializedProperty p)
			{
				p.FindPropertyRelative("m_Name").stringValue = m_Name;
				p.FindPropertyRelative("descriptiveName").stringValue = descriptiveName;
				p.FindPropertyRelative("descriptiveNegativeName").stringValue = descriptiveNegativeName;
				p.FindPropertyRelative("negativeButton").stringValue = negativeButton;
				p.FindPropertyRelative("positiveButton").stringValue = positiveButton;
				p.FindPropertyRelative("altNegativeButton").stringValue = altNegativeButton;
				p.FindPropertyRelative("altPositiveButton").stringValue = altPositiveButton;
				p.FindPropertyRelative("gravity").floatValue = gravity;
				p.FindPropertyRelative("dead").floatValue = dead;
				p.FindPropertyRelative("sensitivity").floatValue = sensitivity;
				p.FindPropertyRelative("snap").boolValue = snap;
				p.FindPropertyRelative("invert").boolValue = invert;
				p.FindPropertyRelative("type").intValue = type;
				p.FindPropertyRelative("axis").intValue = axis;
				p.FindPropertyRelative("joyNum").intValue = joyNum;
			}
		}

		static public void SetupControllerAxes()
		{
			List<Axis> axes = new List<Axis>();
			Axis axis = new Axis();

			// JoystickAxis commons
			axis.dead = 0f;
			axis.sensitivity = 1f;
			axis.snap = false;
			axis.invert = false;
			axis.type = 2;
			axis.joyNum = 0;

			// JoystickAxis1
			axis.m_Name = CameraFlyController.axisJoystickAxis1;
			axis.axis = 0;
			axes.Add(axis);

			// JoystickAxis2
			axis.m_Name = CameraFlyController.axisJoystickAxis2;
			axis.axis = 1;
			axes.Add(axis);

			// JoystickAxis4
			axis.m_Name = CameraFlyController.axisJoystickAxis4;
			axis.axis = 3;
			axes.Add(axis);

			// JoystickAxis5
			axis.m_Name = CameraFlyController.axisJoystickAxis5;
			axis.axis = 4;
			axes.Add(axis);

			// Mouse commons
			axis.dead = 0f;
			axis.sensitivity = 0.1f;
			axis.snap = false;
			axis.invert = false;
			axis.type = 1;
			axis.joyNum = 0;

			// Mouse X
			axis.m_Name = CameraFlyController.axisMouseX;
			axis.axis = 0;
			axes.Add(axis);

			// Mouse Y
			axis.m_Name = CameraFlyController.axisMouseY;
			axis.axis = 1;
			axes.Add(axis);

			SetupAxes(axes);
		}

		static private void SetupAxes(List<Axis> samples)
		{
			string path = "ProjectSettings/InputManager.asset";
			Object inputManagerFile = AssetDatabase.LoadAllAssetsAtPath(path)[0];
			if (inputManagerFile == null)
			{
				Debug.LogErrorFormat("Failed to load asset at path '{0}'", path);
				return;
			}

			SerializedObject inputManagerAsset = new SerializedObject(inputManagerFile);

			HashSet<int> toIgnore = new HashSet<int>();
			SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");
			for (int i = 0, l = axesProperty.arraySize; i < l; i++)
			{
				SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(i);
				string name = axisProperty.FindPropertyRelative("m_Name").stringValue;
				int sampleIndex = samples.FindIndex(delegate(Axis obj) { return obj.m_Name == name; });
				if (sampleIndex != -1)
				{
					// Replace axis
					Axis sample = samples[sampleIndex];
					sample.Serialize(axisProperty);
					toIgnore.Add(sampleIndex);
				}
			}

			for (int i = 0, l = samples.Count; i < l; i++)
			{
				if (toIgnore.Contains(i)) { continue; }

				// Add axis
				Axis sample = samples[i];
				int arrayIndex = axesProperty.arraySize;
				axesProperty.InsertArrayElementAtIndex(arrayIndex);
				SerializedProperty p = axesProperty.GetArrayElementAtIndex(arrayIndex);
				sample.Serialize(p);
			}

			inputManagerAsset.ApplyModifiedProperties();
		}
	}
	#endif

	public class CameraFlyController : MonoBehaviour
	{
		#region Constants
		public const string axisJoystickAxis1 = "JoystickAxis1";
		public const string axisJoystickAxis2 = "JoystickAxis2";
		public const string axisJoystickAxis4 = "JoystickAxis4";
		public const string axisJoystickAxis5 = "JoystickAxis5";
		public const string axisMouseX = "Mouse X";
		public const string axisMouseY = "Mouse Y";

		private float keyboardMoveSpeed = 4f;

		private float joystickAxisThresholdMin = 0.2f;
		private float joystickAxisThresholdMax = 1f;

		private float joystickMoveSpeedXMin = 0f;
		private float joystickMoveSpeedXMax = 5f;
		private float joystickMoveSpeedY = 4f;
		private float joystickMoveSpeedZMin = 0f;
		private float joystickMoveSpeedZMax = 5f;

		private float joystickRotationSpeedMin = 0f;
		private float joystickRotationSpeedMax = 126f;

		static private readonly KeyCode[] keysForward = new KeyCode[] { KeyCode.W, KeyCode.UpArrow };
		static private readonly KeyCode[] keysBackward = new KeyCode[] { KeyCode.S, KeyCode.DownArrow };

		static private readonly KeyCode[] keysLeft = new KeyCode[] { KeyCode.A, KeyCode.LeftArrow };
		static private readonly KeyCode[] keysRight = new KeyCode[] { KeyCode.D, KeyCode.RightArrow };

		static private readonly KeyCode[] keysUp = new KeyCode[] { KeyCode.E, KeyCode.Space };
		static private readonly KeyCode[] keysDown = new KeyCode[] { KeyCode.Q, KeyCode.C };

		static private readonly KeyCode[] keysRun = new KeyCode[] { KeyCode.LeftShift, KeyCode.RightShift };

		static private readonly KeyCode[] joystickUp = new KeyCode[] { KeyCode.Joystick1Button5, KeyCode.Joystick2Button5, KeyCode.Joystick3Button5, KeyCode.Joystick4Button5 };
		static private readonly KeyCode[] joystickDown = new KeyCode[] { KeyCode.Joystick1Button4, KeyCode.Joystick2Button4, KeyCode.Joystick3Button4, KeyCode.Joystick4Button4 };
		#endregion

		#region PrivateFields
		private Transform tr;
		private bool rmbDownInRect;
		private Vector3 mpStart;
		private Vector3 originalRotation;
		private float t;
		#endregion

		private Vector3 mousePosition
		{
			get
			{
				Camera cam = GetComponent<Camera>();
				return cam == null ? Vector3.Scale(Input.mousePosition, new Vector3(1f/Screen.width, 1f/Screen.height, 1f)) : cam.ScreenToViewportPoint(Input.mousePosition);
			}
		}

		// 
		void Awake()
		{
			tr = GetComponent<Transform>();
		}

		// 
		void OnEnable()
		{
			t = Time.realtimeSinceStartup;
		}

		// 
		void Update()
		{
			float timeNow = Time.realtimeSinceStartup;
			float dT = timeNow - t;
			t = timeNow;

			HandleMouseInput(dT);
			HandleControllerInput(dT);
		}

		// 
		void HandleMouseInput(float dT)
		{
			Vector3 mp = mousePosition;

			// Unlock cursor on mouse move
			if (InputHelper.GetAxis(axisMouseX) != 0f || InputHelper.GetAxis(axisMouseY) != 0f)
			{
				Cursor.lockState = CursorLockMode.None;
			}

			bool rmbDown = Input.GetMouseButtonDown(1);
			bool rmbHeld = Input.GetMouseButton(1);
			bool mouseInCameraRect = mp.x >= 0f && mp.x < 1f && mp.y >= 0f && mp.y < 1f;

			rmbDownInRect = (rmbDownInRect && rmbHeld) || (mouseInCameraRect && rmbDown);

			// Movement
			if (rmbDownInRect || (!rmbHeld && mouseInCameraRect))
			{
				Vector3 moveDirection = new Vector3(InputHelper.GetKey(keysRight, keysLeft), InputHelper.GetKey(keysUp, keysDown), InputHelper.GetKey(keysForward, keysBackward));
				float speedMultiplier = InputHelper.GetKey(keysRun) ? 2f : 1f;
				tr.position += tr.TransformDirection(moveDirection * keyboardMoveSpeed * speedMultiplier * dT);
			}

			// Rotation
			if (rmbDownInRect)
			{
				// Right Mouse Button Down
				if (rmbDown)
				{
					originalRotation = tr.localEulerAngles;
					mpStart = mp;
				}

				// Right Mouse Button Hold
				if (rmbHeld)
				{
					Vector2 offs = new Vector2((mp.x - mpStart.x), (mpStart.y - mp.y));
					tr.localEulerAngles = originalRotation + new Vector3(offs.y * 360f, offs.x * 360f, 0f);
				}
			}
		}

		// 
		void HandleControllerInput(float dT)
		{
			float axis1 = InputHelper.GetAxis(axisJoystickAxis1);
			float axis2 = InputHelper.GetAxis(axisJoystickAxis2);
			float axis4 = InputHelper.GetAxis(axisJoystickAxis4);
			float axis5 = InputHelper.GetAxis(axisJoystickAxis5);
			int vertical = InputHelper.GetKey(joystickUp, joystickDown);
			bool run = Input.GetKey(KeyCode.JoystickButton8);

			Vector3 moveDirection = new Vector3
			(
				InputHelper.Remap(axis1, joystickAxisThresholdMin, joystickAxisThresholdMax, joystickMoveSpeedXMin, joystickMoveSpeedXMax), 
				vertical * joystickMoveSpeedY, 
				InputHelper.Remap(-axis2, joystickAxisThresholdMin, joystickAxisThresholdMax, joystickMoveSpeedZMin, joystickMoveSpeedZMax) 
			);

			Vector3 rotation = new Vector3
			(
				InputHelper.Remap(axis5, joystickAxisThresholdMin, joystickAxisThresholdMax, joystickRotationSpeedMin, joystickRotationSpeedMax), 
				InputHelper.Remap(axis4, joystickAxisThresholdMin, joystickAxisThresholdMax, joystickRotationSpeedMin, joystickRotationSpeedMax), 
				0f 
			);

			bool anyInput = moveDirection.sqrMagnitude > 0f || rotation.sqrMagnitude > 0f || vertical != 0 || run;

			// Lock cursor on gamepad input
			if (anyInput)
			{
				Cursor.lockState = CursorLockMode.Locked;
			}

			float speedMultiplier = run ? 2f : 1f;
			tr.position += tr.TransformDirection(moveDirection * speedMultiplier * dT);
			tr.localEulerAngles += rotation * dT;
		}
	}

	static public class InputHelper
	{
		// 
		static public bool GetKey(KeyCode[] keys)
		{
			if (keys != null)
			{
				for (int i = 0, l = keys.Length; i < l; i++)
				{
					if (Input.GetKey(keys[i])) { return true; }
				}
			}
			return false;
		}

		// 
		static public bool GetKeyDown(KeyCode[] keys)
		{
			if (keys != null)
			{
				for (int i = 0, l = keys.Length; i < l; i++)
				{
					if (Input.GetKeyDown(keys[i])) { return true; }
				}
			}
			return false;
		}

		// 
		static public int GetKey(KeyCode[] keysPositive, KeyCode[] keysNegative)
		{
			int result = 0;

			if (GetKey(keysPositive)) { result++; }
			if (GetKey(keysNegative)) { result--; }

			return result;
		}

		// 
		static public int GetKeyDown(KeyCode[] keysPositive, KeyCode[] keysNegative)
		{
			int result = 0;

			if (GetKeyDown(keysPositive)) { result++; }
			if (GetKeyDown(keysNegative)) { result--; }

			return result;
		}

		// 
		static public float GetAxis(string axisName)
		{
			if (HasAxis(axisName)) { return Input.GetAxisRaw(axisName); }
			else { return 0f; }
		}

		// There is currently (Unity 5.5.0f3) no other way in Unity to check if a specific axis is defined in Input Manager settings or not
		static private Dictionary<string, bool> cachedHasAxis = new Dictionary<string, bool>();
		static public bool HasAxis(string axisName)
		{
			bool value;
			if (cachedHasAxis.TryGetValue(axisName, out value))
			{
				return value;
			}

			try { Input.GetAxisRaw(axisName); }
			catch
			{
				cachedHasAxis.Add(axisName, false);
				return false;
			}

			// No exception occured - axis defined in Input settings
			cachedHasAxis.Add(axisName, true);
			return true;
		}

		// 
		static public float Remap(float x, float xMin, float xMax, float yMin, float yMax)
		{
			float sign;
			float abs;
			if (x >= 0f)
			{
				sign = 1f;
				abs = x;
			}
			else
			{
				sign = -1f;
				abs = -x;
			}

			if (abs < xMin) { return 0f; }
			if (abs > xMax) { return xMax; }

			float t = (abs - xMin) / (xMax - xMin);
			return sign * Mathf.Lerp(yMin, yMax, t);
		}
	}
}