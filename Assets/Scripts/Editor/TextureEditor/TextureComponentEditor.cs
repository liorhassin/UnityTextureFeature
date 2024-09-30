using UnityEngine;
using UnityEditor;

namespace Editor.TextureEditor
{
    /// <summary>
    /// Used to define the GUI of custom Texture Component Unity Editor.
    /// </summary>
    [CustomEditor(typeof(TextureComponent))]
    public class TextureComponentEditor : UnityEditor.Editor
    {
        private TextureComponent _textureComponent;
        private bool _hasTexture;

        private void OnEnable()
        {
            _textureComponent = (TextureComponent)target;
            UpdateTextureState();
        }

        /// <summary>
        /// Defining how the GUI looks and what it contains
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("2D Texture", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            _textureComponent.selectedTexture = (Texture2D)EditorGUILayout.ObjectField(
                _textureComponent.selectedTexture, typeof(Texture2D), false);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateTextureState();
            }

            if (!_hasTexture) return;
            
            bool buttonCreated = GUILayout.Button(("Open Texture Viewer"));
            if (buttonCreated) TextureViewerWindow.ShowWindow(_textureComponent.selectedTexture);

        }

        /// <summary>
        /// Update boolean if texture has already been selected or not
        /// </summary>
        private void UpdateTextureState()
        {
            _hasTexture = _textureComponent.selectedTexture != null;
        }
    }
}