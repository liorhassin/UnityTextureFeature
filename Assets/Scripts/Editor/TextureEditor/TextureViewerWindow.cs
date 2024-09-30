using UnityEngine;
using UnityEditor;

namespace Editor.TextureEditor
{
    /// <summary>
    /// Custom editor window for viewing textures.
    /// Provides functionality for zooming, panning, and displaying textures.
    /// </summary>
    public class TextureViewerWindow : EditorWindow
    {
        private Texture2D _texture;
        private float _zoom;
        private Vector2 _panOffset;
        private Vector2 _lastMousePosition;
        
        /// <summary>
        /// Creates and shows a new TextureViewerWindow with the selected texture.
        /// </summary>
        /// <param name="texture">The texture to be displayed in the viewer</param>
        public static void ShowWindow(Texture2D texture)
        {
            TextureViewerWindow window = GetWindow<TextureViewerWindow>("Texture Viewer");
            
            window._texture = texture;
            window._panOffset = Vector2.zero;
            
            // Calculating minimum zoom value in order to view entire texture(depending on the texture size)
            window._zoom = Mathf.Min(
                (TextureViewerUtility.WindowSize - 2 * TextureViewerUtility.BorderSize) / (float)texture.width,
                (TextureViewerUtility.WindowSize - 2 * TextureViewerUtility.BorderSize) / (float)texture.height
            );
            
            window.maxSize = new Vector2(TextureViewerUtility.WindowSize, TextureViewerUtility.WindowSize);
        }

        /// <summary>
        /// Handles the GUI rendering for the TextureViewerWindow.
        /// </summary>
        private void OnGUI()
        {
            HandleInput();
            DrawTextureViewer();
        }

        /// <summary>
        /// Processes user input events for zooming and panning.
        /// </summary>
        private void HandleInput()
        {
            Event e = Event.current;
            Rect viewRect = TextureViewerUtility.CalculateViewRect(new Rect(
                TextureViewerUtility.BorderSize, TextureViewerUtility.BorderSize,
                TextureViewerUtility.WindowSize, TextureViewerUtility.WindowSize));
            switch (e.type) {
                case EventType.ScrollWheel:
                    HandleZooming(e, viewRect);
                    break;
                case EventType.MouseDown:
                    _lastMousePosition = e.mousePosition;
                    e.Use();
                    break;
                case EventType.MouseDrag:
                    HandleDragging(e, viewRect);
                    break;
            }
        }

        /// <summary>
        /// Handles zooming functionality when the user scrolls the mouse wheel.
        /// </summary>
        /// <param name="e">The current event</param>
        /// <param name="viewRect">The rectangle representing the view area</param>
        private void HandleZooming(Event e, Rect viewRect) {
            
            float oldZoom = _zoom;
            if (!oldZoom.Equals(_zoom)) return;
            
            Vector2 mousePositionInTexture = TextureViewerUtility.
                GetMousePositionInTexture(e.mousePosition, viewRect, _texture, _zoom, _panOffset);
            
            _zoom = TextureViewerUtility.CalculateNewZoom(_zoom, e.delta.y, _texture, viewRect);
            
            _panOffset = TextureViewerUtility
                .CalculateNewPanOffset(mousePositionInTexture, e.mousePosition, viewRect, _texture, _zoom);
            
            e.Use();
            Repaint();
        }
        
        /// <summary>
        /// Handles panning functionality when the user drags the mouse.
        /// </summary>
        /// <param name="e">The current event</param>
        /// <param name="viewRect">The rectangle representing the view area</param>
        private void HandleDragging(Event e, Rect viewRect)
        {
            Vector2 delta = e.mousePosition - _lastMousePosition;
            _panOffset += delta;
            
            float contentWidth = _texture.width * _zoom;
            float contentHeight = _texture.height * _zoom;
            
            // Clamp pan offset to keep texture within view
            _panOffset.x = Mathf.Clamp(_panOffset.x, viewRect.width - contentWidth, 0);
            _panOffset.y = Mathf.Clamp(_panOffset.y, viewRect.height - contentHeight, 0);

            _lastMousePosition = e.mousePosition;
            e.Use();
        }

        /// <summary>
        /// Draws the texture in the viewer window, applying current zoom and pan settings.
        /// </summary>
        private void DrawTextureViewer()
        {
            Rect windowRect = new Rect(0, 0, TextureViewerUtility.WindowSize, TextureViewerUtility.WindowSize);
            Rect viewRect = TextureViewerUtility.CalculateViewRect(windowRect);

            // Calculate content size 
            float contentWidth = _texture.width * _zoom;
            float contentHeight = _texture.height * _zoom;

            // Apply pan offset to content rectangle area
            Rect contentRect = TextureViewerUtility.CalculateContentRect(viewRect, contentWidth, contentHeight, _panOffset);
            
            GUI.BeginClip(viewRect);
            
            GUI.DrawTexture(new Rect(contentRect.x - viewRect.x, contentRect.y - viewRect.y, contentRect.width, contentRect.height), 
                            _texture, ScaleMode.ScaleAndCrop);
            
            GUI.EndClip();
        }
    }
}