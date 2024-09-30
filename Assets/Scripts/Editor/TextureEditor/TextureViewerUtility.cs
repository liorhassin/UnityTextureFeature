using UnityEngine;

namespace Editor.TextureEditor
{
    /// <summary>
    /// Static Util class, Contains useful calculation methods and windows parameters.
    /// </summary>
    public static class TextureViewerUtility
    {
        public const int WindowSize = 500;
        public const int BorderSize = 20;
        private const float ZoomSpeed = 0.1f;
        private const float MaxZoom = 10f;
        
        /// <summary>
        /// Calculates new zoom value based on current zoom and scroll input.
        /// </summary>
        /// <param name="currentZoom">Current zoom level before applying the new zoom calculation</param>
        /// <param name="scrollDelta">Positive for zoom out, negative for zoom in</param>
        /// <param name="texture">The texture being viewed</param>
        /// <param name="viewRect">The rectangle of the viewing area</param>
        /// <returns>The new zoom level, clamped between a minimum and maximum value(fit to view)</returns>
        public static float CalculateNewZoom(float currentZoom, float scrollDelta, Texture2D texture, Rect viewRect)
        {
            float newZoom = currentZoom - scrollDelta * ZoomSpeed;
            
            // Making sure minimum zoom is perfect fit with window size
            float minZoom = Mathf.Min(
                viewRect.width / texture.width,
                viewRect.height / texture.height
            );
            
            return Mathf.Clamp(newZoom, minZoom, MaxZoom);
        }
        
        /// <summary>
        /// Simple view calculation method, Using given rect to calculate view size.
        /// </summary>
        /// <param name="windowRect">Rectangle of a window you wish to calculate view rectangle for</param>
        /// <returns>The rectangle that represents the viewing area</returns>
        public static Rect CalculateViewRect(Rect windowRect)
        {
            return new Rect(BorderSize, BorderSize, windowRect.width - 2 * BorderSize, windowRect.height - 2 * BorderSize);
        }

        /// <summary>
        /// Calculates the inner content rectangle for the texture view, considering the view offset displacement.
        /// </summary>
        /// <param name="viewRect">The rectangle of the viewing area</param>
        /// <param name="contentWidth">Expected texture view width</param>
        /// <param name="contentHeight">Expected texture view height</param>
        /// <param name="panOffset">Displacement of view area</param>
        /// <returns>The rectangle that represents the primary window viewing area</returns>
        public static Rect CalculateContentRect(Rect viewRect, float contentWidth, float contentHeight, Vector2 panOffset)
        {
            float x = viewRect.x + panOffset.x;
            float y = viewRect.y + panOffset.y;
            
            return new Rect(x, y, contentWidth, contentHeight);
        }

        /// <summary>
        /// Helper function to calculate mouse position relative to texture viewing area.
        /// </summary>
        /// <param name="mousePosition">Current mouse position in parent window</param>
        /// <param name="viewRect">The rectangle of the viewing area</param>
        /// <param name="texture">The texture being viewed</param>
        /// <param name="zoom">Current zoom value</param>
        /// <param name="panOffset">Displacement of view area</param>
        /// <returns>The mouse position relative to the texture view area</returns>
        public static Vector2 GetMousePositionInTexture(Vector2 mousePosition, Rect viewRect, Texture2D texture, float zoom, Vector2 panOffset)
        {
            float contentWidth = texture.width * zoom;
            float contentHeight = texture.height * zoom;
            Rect contentRect = CalculateContentRect(viewRect, contentWidth, contentHeight, panOffset);

            Vector2 mousePositionInContent = mousePosition - new Vector2(contentRect.x, contentRect.y);
            
            return new Vector2(
                mousePositionInContent.x / contentWidth * texture.width,
                mousePositionInContent.y / contentHeight * texture.height
            );
        }

        /// <summary>
        /// Calculate the displacement of the texture view.
        /// </summary>
        /// <param name="mousePositionInTexture">Relative to texture view</param>
        /// <param name="mousePosition">Relative to parent window</param>
        /// <param name="viewRect">The rectangle of the viewing area</param>
        /// <param name="texture">The texture being viewed</param>
        /// <param name="newZoom">New zoom to calculate displacement</param>
        /// <returns>The new calculation of displacement for texture view</returns>
        public static Vector2 CalculateNewPanOffset(Vector2 mousePositionInTexture, Vector2 mousePosition, Rect viewRect, Texture2D texture, float newZoom)
        {
            Vector2 mousePositionInView = mousePosition - viewRect.position;

            // Calculate the mouse position relative to texture size
            Vector2 relativeMousePosition = new Vector2(
                mousePositionInTexture.x / texture.width,
                mousePositionInTexture.y / texture.height
            );
            
            Vector2 newContentSize = new Vector2(texture.width * newZoom, texture.height * newZoom);

            // Calculate where the mouse should be in the new content space
            Vector2 targetMousePositionInContent = new Vector2(
                relativeMousePosition.x * newContentSize.x,
                relativeMousePosition.y * newContentSize.y
            );

            // Calculate the new pan offset
            Vector2 newPanOffset = mousePositionInView - targetMousePositionInContent;

            // Ensure the texture stays within the view bounds
            newPanOffset.x = Mathf.Clamp(newPanOffset.x, viewRect.width - newContentSize.x, 0);
            newPanOffset.y = Mathf.Clamp(newPanOffset.y, viewRect.height - newContentSize.y, 0);

            return newPanOffset;
        }


    }
}