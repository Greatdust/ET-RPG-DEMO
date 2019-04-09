using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighlightingSystem.Demo
{
	public class UI
	{
		public class DropdownState
		{
			public List<GUIContent> items = new List<GUIContent>();
			public int selected = -1;
			public float itemMinHeight = 30f;
			public float maxHeight = 150f;
			public Vector2 scrollPosition = new Vector2(0f, 0f);
			public bool expanded = false;
		}

		#region Static Public Fields
		static public readonly GUIContent labelArrowUp = new GUIContent(GetTexture(Color.white, 9, 6, new int[] { 0, 254, 124, 56, 16, 0 }));
		static public readonly GUIContent labelArrowDown = new GUIContent(GetTexture(Color.white, 9, 6, new int[] { 0, 16, 56, 124, 254, 0 }));
		static public readonly GUIContent labelArrowLeft = new GUIContent(GetTexture(Color.white, 6, 9, new int[] { 0, 16, 24, 28, 30, 28, 24, 16, 0 }));
		static public readonly GUIContent labelArrowRight = new GUIContent(GetTexture(Color.white, 6, 9, new int[] { 0, 2, 6, 14, 30, 14, 6, 2, 0 }));

		static public Color textColor = new Color(1f, 1f, 1f, 1f);
		static public Color backgroundColor = new Color(0f, 0f, 0f, 0.3f);
		static public Vector2 screenBorder = new Vector2(20f, 20f);
		static public Vector2 spacing = new Vector2(6f, 6f);

		static public GUISkin skin;
		#endregion

		#region Static Private Fields
		static private readonly int sDropdownItemHash = "DropdownItem".GetHashCode();
		static private readonly GUIContent sContent = new GUIContent();

		static private Texture2D textureNormal = GetTexture(4, 4, backgroundColor);
		static private Texture2D textureHover = GetTexture(4, 4, new Color(0.259f, 0.651f, 0.875f, 0.75f));
		static private Texture2D textureActive = GetTexture(4, 4, new Color(0.259f, 0.651f, 0.875f, 0.5f));

		static private Texture2D textureOnNormal = GetTexture(4, 4, new Color(0.5f, 0.5f, 0.5f, backgroundColor.a));
		static private Texture2D textureOnHover = GetTexture(4, 4, new Color(0.259f, 0.651f, 0.875f, 0.75f));
		static private Texture2D textureOnActive = GetTexture(4, 4, new Color(0.259f, 0.651f, 0.875f, 0.5f));
		#endregion

		// Ctor
		static UI()
		{
			Font font = Resources.Load<Font>("monospace");
			if (font != null)
			{
				font.material.mainTexture.filterMode = FilterMode.Point;
			}

			// Fallback to Unity built-in font
			if (font == null)
			{
				font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			}

			skin = ScriptableObject.CreateInstance<GUISkin>();

			skin.customStyles = new GUIStyle[]
			{
				new GUIStyle()
				{
					name = "dropdown", 
					alignment = TextAnchor.MiddleLeft, 
					padding = new RectOffset(8, 23, 8, 8), 
					clipping = TextClipping.Clip, 
					normal = new GUIStyleState() { background = textureNormal, textColor = textColor }, 
					hover = new GUIStyleState() { background = textureHover, textColor = textColor }, 
					active = new GUIStyleState() { background = textureActive, textColor = textColor }, 
				}, 
				new GUIStyle()
				{
					name = "dropdownitem", 
					font = font, 
					wordWrap = true, 
					alignment = TextAnchor.MiddleLeft, 
					padding = new RectOffset(8, 8, 8, 8), 

					normal = new GUIStyleState() { background = textureNormal, textColor = textColor }, 
					hover = new GUIStyleState() { background = textureHover, textColor = textColor }, 
					active = new GUIStyleState() { background = textureActive, textColor = textColor }, 

					onNormal = new GUIStyleState() { background = textureOnNormal, textColor = textColor }, 
					onHover = new GUIStyleState() { background = textureOnHover, textColor = textColor }, 
					onActive = new GUIStyleState() { background = textureOnActive, textColor = textColor }, 
				}, 
				new GUIStyle()
				{
					name = "dropdownbutton", 
					alignment = TextAnchor.MiddleRight, 
					padding = new RectOffset(8, 8, 8, 8), 
					normal = new GUIStyleState() { textColor = textColor }, 
				}, 
			};

			skin.label = new GUIStyle()
			{
				font = font, 
				richText = true, 
				wordWrap = true, 
				padding = new RectOffset(10, 10, 10, 10), 
				normal = new GUIStyleState() { background = textureNormal, textColor = textColor }
			};

			skin.button = new GUIStyle()
			{
				font = font, 
				padding = new RectOffset(6, 6, 6, 6), 
				alignment = TextAnchor.MiddleCenter, 
				normal = new GUIStyleState() { background = textureNormal, textColor = textColor }, 
				hover = new GUIStyleState() { background = textureHover, textColor = textColor }, 
				active = new GUIStyleState() { background = textureActive, textColor = textColor }, 
			};

			skin.verticalScrollbar = new GUIStyle()
			{
				normal = new GUIStyleState() { background = textureNormal }, 
				fixedWidth = 20f, 
				//margin = new RectOffset(1, 0, 0, 0), 
			}; 

			skin.verticalScrollbarThumb = new GUIStyle()
			{
				padding = new RectOffset(0, 0, 10, 10), 

				normal = new GUIStyleState() { background = textureNormal }, 
				hover = new GUIStyleState() { background = textureHover }, 
				active = new GUIStyleState() { background = textureActive }, 
			};

			skin.font = font;
		}

		// 
		static public bool Button(Rect position, GUIContent label)
		{
			return GUI.Button(position, label, skin.button);
		}

		// 
		static public int Dropdown(Rect position, DropdownState state)
		{
			int result = state.selected;

			GUIContent content = state.selected >= 0 && state.selected < state.items.Count ? state.items[state.selected] : GUIContent.none;
			if (GUI.Button(position, content, skin.GetStyle("dropdown")))
			{
				state.expanded = !state.expanded;
			}

			GUI.Label(position, state.expanded ? labelArrowUp : labelArrowDown, skin.GetStyle("dropdownbutton"));

			if (state.expanded)
			{
				GUIStyle verticalScrollbar = GUI.skin.verticalScrollbar;
				Rect viewRect = new Rect(0f, 0f, position.width, 0f);
				var itemStyle = skin.GetStyle("dropdownitem");
				bool scrollbar = false;
				for (int i = 0; i < state.items.Count; i++)
				{
					viewRect.height += Mathf.Max(itemStyle.CalcHeight(state.items[i], viewRect.width), state.itemMinHeight);
				}
				if (viewRect.height > state.maxHeight)
				{
					scrollbar = true;

					// Recalculate viewRect height for new width without scrollbar
					viewRect.width = position.width - (verticalScrollbar.fixedWidth + verticalScrollbar.margin.left);
					viewRect.height = 0f;
					for (int i = 0; i < state.items.Count; i++)
					{
						viewRect.height += Mathf.Max(itemStyle.CalcHeight(state.items[i], viewRect.width), state.itemMinHeight);
					}
				}

				// float clipWidth = viewRect.width;
				float clipHeight = Mathf.Min(viewRect.height, state.maxHeight);

				Event evt = Event.current;

				Rect scrollRect = new Rect(position.x, position.y + position.height, position.width, clipHeight);

				// Override scroll delta
				if (evt.rawType == EventType.ScrollWheel && scrollRect.Contains(evt.mousePosition))
				{
					evt.delta = new Vector2(evt.delta.x, ((evt.delta.y > 0f ? 1f : -1f) * state.itemMinHeight) / 20f);
				}

				// Manually draw VerticalScrollbar to fix String.Concat garbage every frame in GUI.VerticalScrollbar
				if (scrollbar)
				{
					var scrollbarRect = new Rect(scrollRect.x + scrollRect.width - verticalScrollbar.fixedWidth, scrollRect.y, verticalScrollbar.fixedWidth, scrollRect.height);
					state.scrollPosition.y = GUI.VerticalScrollbar(scrollbarRect, state.scrollPosition.y, clipHeight, 0f, viewRect.height);
				}
				state.scrollPosition = GUI.BeginScrollView(scrollRect, state.scrollPosition, viewRect, GUIStyle.none, GUIStyle.none);

				// Clip y Min Max
				Vector2 clipY = new Vector2(state.scrollPosition.y, state.scrollPosition.y + clipHeight);

				float y = 0f;

				for (int i = 0; i < state.items.Count; i++)
				{
					GUIContent item = state.items[i];
					float h = Mathf.Max(itemStyle.CalcHeight(item, viewRect.width), state.itemMinHeight);

					bool selected = i == state.selected;
					if (DropdownItem(new Rect(0f, y, viewRect.width, h), clipY, item, selected, itemStyle))
					{
						result = i;
						state.expanded = false;
					}
					y += h;
				}

				GUI.EndScrollView(true);

				// Handle click outside of the scrollRect and Escape button
				if ((evt.rawType == EventType.MouseUp && !scrollRect.Contains(evt.mousePosition)) || (evt.rawType == EventType.KeyDown && evt.keyCode == KeyCode.Escape))
				{
					state.expanded = false;
				}
			}

			return result;
		}

		// 
		static private bool DropdownItem(Rect position, Vector2 clipY, GUIContent content, bool on, GUIStyle style)
		{
			int id = GUIUtility.GetControlID(sDropdownItemHash, FocusType.Passive, position);

			float yMin = Mathf.Max(position.y, clipY[0]);
			float yMax = Mathf.Min(position.y + position.height, clipY[1]);
			Rect clippedRect = new Rect(position.x, yMin, position.width, yMax - yMin);
			bool isMouseOver = clippedRect.height > 0f && clippedRect.Contains(Event.current.mousePosition);

			switch (Event.current.GetTypeForControl(id))
			{
				case EventType.MouseDown:
					if (isMouseOver)
					{
						GUIUtility.hotControl = id;
						Event.current.Use();
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == id)
					{
						Event.current.Use();
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == id)
					{
						GUIUtility.hotControl = 0;
						Event.current.Use();

						if (isMouseOver)
						{
							GUI.changed = true;
							return true;
						}
					}
					break;
				case EventType.Repaint:
					// Draw only if visible
					if (clippedRect.height > 0f)
					{
						bool isHotControl = GUIUtility.hotControl == id;
						style.Draw(position, content, isMouseOver && (isHotControl || GUIUtility.hotControl == 0), isHotControl, on, false);
					}
					break;
			}
			return false;
		}

		// 
		static public void Content(string text, TextAnchor anchor)
		{
			sContent.text = text;
			Content(sContent, anchor);
		}

		// 
		static public void Content(GUIContent content, TextAnchor anchor)
		{
			if (string.IsNullOrEmpty(content.text) && content.image == null) { return; }

			Vector2 size = skin.label.CalcSize(content);

			Rect position = new Rect(0f, 0f, size.x, size.y);
			position.position = Position(size, anchor);

			// Label
			GUI.Label(position, content, skin.label);
		}

		// 
		static public Vector2 Position(Vector2 size, TextAnchor anchor)
		{
			Vector2 position = new Vector2();

			// Horizontally
			float left = screenBorder.x;
			float center = 0.5f * (Screen.width - size.x);
			float right = Screen.width - screenBorder.x - size.x;

			// Vertically
			float upper = screenBorder.y;
			float middle = 0.5f * (Screen.height - size.y);
			float lower = Screen.height - screenBorder.y - size.y;

			switch (anchor)
			{
				case TextAnchor.UpperLeft:      position.y = upper;  position.x = left;   break;
				case TextAnchor.UpperCenter:    position.y = upper;  position.x = center; break;
				case TextAnchor.UpperRight:     position.y = upper;  position.x = right;  break;

				case TextAnchor.MiddleLeft:     position.y = middle; position.x = left;   break;
				case TextAnchor.MiddleCenter:   position.y = middle; position.x = center; break;
				case TextAnchor.MiddleRight:    position.y = middle; position.x = right;  break;

				case TextAnchor.LowerLeft:      position.y = lower;  position.x = left;   break;
				case TextAnchor.LowerCenter:    position.y = lower;  position.x = center; break;
				case TextAnchor.LowerRight:     position.y = lower;  position.x = right;  break;
			}

			return position;
		}

		// 
		static private Texture2D GetTexture(int width, int height, Color color)
		{
			Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					tex.SetPixel(x, y, color);
				}
			}
			tex.Apply();
			tex.hideFlags = HideFlags.HideAndDontSave;
			return tex;
		}

		// 
		static public Texture2D GetTexture(Color color, int width, int height, int[] data)
		{
			Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
			for (int y = 0; y < height; y++)
			{
				for (int x = 0, xmax = Mathf.Min(width, 32); x < xmax; x++)
				{
					tex.SetPixel(x, y, ((data[y] & (1 << x)) != 0) ? color : Color.clear);
				}
			}
			tex.Apply();
			tex.hideFlags = HideFlags.HideAndDontSave;
			return tex;
		}
	}
}