using System;
using Borodar.FarlandSkies.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Borodar.FarlandSkies.LowPoly
{
    [CustomEditor(typeof(SkyboxController))]
    public class SkyboxControllerEditor : Editor
    {
        // Skybox
        private SerializedProperty _skyboxMaterial;
        // Sky
        private SerializedProperty _skyTopColor;
        private SerializedProperty _skyMiddleColor;
        private SerializedProperty _skyBottomColor;
        private SerializedProperty _skyTopExponent;
        private SerializedProperty _skyBottomExponent;
        // Stars
        private SerializedProperty _starsEnabled;
        private SerializedProperty _starsCubemap;
        private SerializedProperty _starsTint;
        private SerializedProperty _starsExtinction;
        private SerializedProperty _starsTwinklingSpeed;
        // Sun
        private SerializedProperty _sunEnabled;
        private SerializedProperty _sunTexture;
        private SerializedProperty _sunLight;
        private SerializedProperty _sunTint;
        private SerializedProperty _sunSize;
        private SerializedProperty _sunHalo;
        private SerializedProperty _sunFlare;
        private SerializedProperty _sunFlareBrightness;
        // Moon 1
        private SerializedProperty _moon1Enabled;
        private SerializedProperty _moon1Texture;
        private SerializedProperty _moon1Light;
        private SerializedProperty _moon1Tint;
        private SerializedProperty _moon1Size;
        private SerializedProperty _moon1Halo;
        private SerializedProperty _moon1Flare;
        private SerializedProperty _moon1FlareBrightness;
        // Moon 2
        private SerializedProperty _moon2Enabled;
        private SerializedProperty _moon2Texture;
        private SerializedProperty _moon2Light;
        private SerializedProperty _moon2Tint;
        private SerializedProperty _moon2Size;
        private SerializedProperty _moon2Halo;
        private SerializedProperty _moon2Flare;
        private SerializedProperty _moon2FlareBrightness;
        // Clouds
        private SerializedProperty _cloudsEnabled;
        private SerializedProperty _cloudsCubemap;
        private SerializedProperty _cloudsTint;
        private SerializedProperty _cloudsHeight;
        private SerializedProperty _cloudsRotation;
        // General
        private SerializedProperty _exposure;
        private SerializedProperty _adjustFogColor;

        // Editor presets
        private SkyboxControllerPresets _presets;
        // Icons
        private GUIContent _starsIcon;
        private GUIContent _skyIcon;
        private GUIContent _sunIcon;
        private GUIContent _moonIcon;
        private GUIContent _cloudsIcon;
        private GUIContent _generalIcon;
        // Labels
        private GUIContent _emptyGUIContent;

        //---------------------------------------------------------------------
        // Public
        //---------------------------------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            CustomGUILayout();
            serializedObject.ApplyModifiedProperties();
        }

        //---------------------------------------------------------------------
        // Protected
        //---------------------------------------------------------------------

        protected void OnEnable()
        {
            // Skybox
            _skyboxMaterial = serializedObject.FindProperty("SkyboxMaterial");
            // Sky
            _skyTopColor = serializedObject.FindProperty("_topColor");
            _skyMiddleColor = serializedObject.FindProperty("_middleColor");
            _skyBottomColor = serializedObject.FindProperty("_bottomColor");
            _skyTopExponent = serializedObject.FindProperty("_topExponent");
            _skyBottomExponent = serializedObject.FindProperty("_bottomExponent");
            // Stars
            _starsEnabled = serializedObject.FindProperty("_starsEnabled");
            _starsCubemap = serializedObject.FindProperty("_starsCubemap");
            _starsTint = serializedObject.FindProperty("_starsTint");
            _starsExtinction = serializedObject.FindProperty("_starsExtinction");
            _starsTwinklingSpeed = serializedObject.FindProperty("_starsTwinklingSpeed");
            // Sun
            _sunEnabled = serializedObject.FindProperty("_sunEnabled");
            _sunLight = serializedObject.FindProperty("_sunLight");
            _sunTexture = serializedObject.FindProperty("_sunTexture");
            _sunTint = serializedObject.FindProperty("_sunTint");
            _sunSize = serializedObject.FindProperty("_sunSize");
            _sunHalo = serializedObject.FindProperty("_sunHalo");
            _sunFlare = serializedObject.FindProperty("_sunFlare");
            _sunFlareBrightness = serializedObject.FindProperty("_sunFlareBrightness");
            // Moon 1
            _moon1Enabled = serializedObject.FindProperty("_moon1Enabled");
            _moon1Texture = serializedObject.FindProperty("_moon1Texture");
            _moon1Light = serializedObject.FindProperty("_moon1Light");
            _moon1Tint = serializedObject.FindProperty("_moon1Tint");
            _moon1Size = serializedObject.FindProperty("_moon1Size");
            _moon1Halo = serializedObject.FindProperty("_moon1Halo");
            _moon1Flare = serializedObject.FindProperty("_moon1Flare");
            _moon1FlareBrightness = serializedObject.FindProperty("_moon1FlareBrightness");
            // Moon 2
            _moon2Enabled = serializedObject.FindProperty("_moon2Enabled");
            _moon2Texture = serializedObject.FindProperty("_moon2Texture");
            _moon2Light = serializedObject.FindProperty("_moon2Light");
            _moon2Tint = serializedObject.FindProperty("_moon2Tint");
            _moon2Size = serializedObject.FindProperty("_moon2Size");
            _moon2Halo = serializedObject.FindProperty("_moon2Halo");
            _moon2Flare = serializedObject.FindProperty("_moon2Flare");
            _moon2FlareBrightness = serializedObject.FindProperty("_moon2FlareBrightness");
            // Clouds
            _cloudsEnabled = serializedObject.FindProperty("_cloudsEnabled");
            _cloudsCubemap = serializedObject.FindProperty("_cloudsCubemap");
            _cloudsTint = serializedObject.FindProperty("_cloudsTint");
            _cloudsHeight = serializedObject.FindProperty("_cloudsHeight");
            _cloudsRotation = serializedObject.FindProperty("_cloudsRotation");
            // General
            _exposure = serializedObject.FindProperty("_exposure");
            _adjustFogColor = serializedObject.FindProperty("_adjustFogColor");

            // Presets
            _presets = FarlandSkiesEditorUtility.LoadFromAsset<SkyboxControllerPresets>("Low Poly/Data/SkyboxControllerPresets.asset");

            // Labels
            _emptyGUIContent = new GUIContent(" ");

            // Icons
            var skinFolder = (EditorGUIUtility.isProSkin) ? "Professional/" : "Personal/";

            var starsTex = FarlandSkiesEditorUtility.LoadEditorIcon(skinFolder + "Star_16.png");
            var skyTex = FarlandSkiesEditorUtility.LoadEditorIcon(skinFolder + "Sky_16.png");
            var sunTex = FarlandSkiesEditorUtility.LoadEditorIcon(skinFolder + "Sun_16.png");
            var moonTex = FarlandSkiesEditorUtility.LoadEditorIcon(skinFolder + "Moon_16.png");
            var cloudsTex = FarlandSkiesEditorUtility.LoadEditorIcon(skinFolder + "Cloud_16.png");
            var generalTex = FarlandSkiesEditorUtility.LoadEditorIcon(skinFolder + "Tag_16.png");

            _starsIcon = new GUIContent(starsTex);
            _skyIcon = new GUIContent(skyTex);
            _sunIcon = new GUIContent(sunTex);
            _moonIcon = new GUIContent(moonTex);
            _cloudsIcon = new GUIContent(cloudsTex);
            _generalIcon = new GUIContent(generalTex);
        }

        //---------------------------------------------------------------------
        // Helpers
        //---------------------------------------------------------------------

        private void CustomGUILayout()
        {
            // Rate now
            RateMeDialog.DrawRateDialog(AssetInfo.ASSET_NAME, AssetInfo.ASSET_STORE_ID);

            // Skybox
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_skyboxMaterial);
            EditorGUILayout.Space();

            SkyGUILayout();
            EditorGUILayout.Space();

            StarsGUILayout();
            EditorGUILayout.Space();

            SunGUILayout();
            EditorGUILayout.Space();

            Moon1GUILayout();
            EditorGUILayout.Space();

            Moon2GUILayout();
            EditorGUILayout.Space();

            CloudsGUILayout();
            EditorGUILayout.Space();

            GeneralGUILayout();
            EditorGUILayout.Space();
        }

        private void SkyGUILayout()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField(_skyIcon, GUILayout.Width(16f));
            EditorGUILayout.LabelField("Sky", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(_skyTopColor);
            EditorGUILayout.PropertyField(_skyMiddleColor);
            EditorGUILayout.PropertyField(_skyBottomColor);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_skyTopExponent);
            EditorGUILayout.PropertyField(_skyBottomExponent);
        }

        private void StarsGUILayout()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField(_starsIcon, GUILayout.Width(16f));
            EditorGUILayout.LabelField("Stars", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            var enabledBefore = _starsEnabled.boolValue;
            EditorGUILayout.PropertyField(_starsEnabled, GUIContent.none, GUILayout.Width(16f));
            EditorGUILayout.EndHorizontal();

            if (_starsEnabled.boolValue)
            {
                StarsCubemapPopup(enabledBefore);
                EditorGUILayout.PropertyField(_starsTint);
                EditorGUILayout.PropertyField(_starsExtinction);
                EditorGUILayout.PropertyField(_starsTwinklingSpeed);
            }
            else
            {
                SkyboxController.Instance.StarsCubemap = null;
            }
        }

        private void SunGUILayout()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField(_sunIcon, GUILayout.Width(16f));
            EditorGUILayout.LabelField("Sun", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            var enabledBefore = _sunEnabled.boolValue;
            EditorGUILayout.PropertyField(_sunEnabled, GUIContent.none, GUILayout.Width(16f));
            EditorGUILayout.EndHorizontal();

            if (_sunEnabled.boolValue)
            {
                SunTexturePopup(enabledBefore);
                EditorGUILayout.PropertyField(_sunLight);
                EditorGUILayout.PropertyField(_sunTint);
                EditorGUILayout.PropertyField(_sunSize);
                EditorGUILayout.PropertyField(_sunHalo);
                EditorGUILayout.PropertyField(_sunFlare);
                EditorGUILayout.PropertyField(_sunFlareBrightness);
            }
            else
            {
                SkyboxController.Instance.SunTexture = null;
            }
        }

        private void Moon1GUILayout()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField(_moonIcon, GUILayout.Width(16f));
            EditorGUILayout.LabelField("Moon 1", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            var enabledBefore = _moon1Enabled.boolValue;
            EditorGUILayout.PropertyField(_moon1Enabled, GUIContent.none, GUILayout.Width(16f));
            EditorGUILayout.EndHorizontal();

            if (_moon1Enabled.boolValue)
            {
                Moon1TexturePopup(enabledBefore);
                EditorGUILayout.PropertyField(_moon1Light);
                EditorGUILayout.PropertyField(_moon1Tint);
                EditorGUILayout.PropertyField(_moon1Size);
                EditorGUILayout.PropertyField(_moon1Halo);
                EditorGUILayout.PropertyField(_moon1Flare);
                EditorGUILayout.PropertyField(_moon1FlareBrightness);
            }
            else
            {
                SkyboxController.Instance.Moon1Texture = null;
            }
        }

        private void Moon2GUILayout()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField(_moonIcon, GUILayout.Width(16f));
            EditorGUILayout.LabelField("Moon 2", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            var enabledBefore = _moon2Enabled.boolValue;
            EditorGUILayout.PropertyField(_moon2Enabled, GUIContent.none, GUILayout.Width(16f));
            EditorGUILayout.EndHorizontal();

            if (_moon2Enabled.boolValue)
            {
                Moon2TexturePopup(enabledBefore);
                EditorGUILayout.PropertyField(_moon2Light);
                EditorGUILayout.PropertyField(_moon2Tint);
                EditorGUILayout.PropertyField(_moon2Size);
                EditorGUILayout.PropertyField(_moon2Halo);
                EditorGUILayout.PropertyField(_moon2Flare);
                EditorGUILayout.PropertyField(_moon2FlareBrightness);
            }
            else
            {
                SkyboxController.Instance.Moon2Texture = null;
            }
        }

        private void CloudsGUILayout()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField(_cloudsIcon, GUILayout.Width(16f));
            EditorGUILayout.LabelField("Clouds", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            var enabledBefore = _cloudsEnabled.boolValue;
            EditorGUILayout.PropertyField(_cloudsEnabled, GUIContent.none, GUILayout.Width(16f));
            EditorGUILayout.EndHorizontal();

            if (_cloudsEnabled.boolValue)
            {
                CloudsCubemapPopup(enabledBefore);
                EditorGUILayout.PropertyField(_cloudsTint);
                EditorGUILayout.PropertyField(_cloudsHeight);
                EditorGUILayout.PropertyField(_cloudsRotation);
            }
            else
            {
                SkyboxController.Instance.CloudsCubemap = null;
            }
        }

        private void GeneralGUILayout()
        {
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField(_generalIcon, GUILayout.Width(16f));
            EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(_exposure);
            EditorGUILayout.PropertyField(_adjustFogColor);
        }

        // Custom Popups

        private void StarsCubemapPopup(bool enabledBefore)
        {
            var cubemaps = _presets.StarsCubemaps;
            var cubemapNames = _presets.StarsCubemapNames;

            var currentCubemap = _starsCubemap.objectReferenceValue as Cubemap;
            if (!enabledBefore && cubemaps.Length > 0) currentCubemap = cubemaps[0];

            var cubemapIndex = Array.IndexOf(cubemaps, currentCubemap);
            if (cubemapIndex < 0) cubemapIndex = cubemaps.Length;

            cubemapIndex = EditorGUILayout.Popup("Stars Cubemap", cubemapIndex, cubemapNames);

            if (cubemapIndex < cubemaps.Length)
            {
                _starsCubemap.objectReferenceValue = cubemaps[cubemapIndex];
            }
            else
            {
                if (ArrayUtility.Contains(cubemaps, currentCubemap)) _starsCubemap.objectReferenceValue = null;
                EditorGUILayout.PropertyField(_starsCubemap, _emptyGUIContent);
            }
        }

        private void SunTexturePopup(bool enabledBefore)
        {
            var textures = _presets.SunTextures;
            var textureNames = _presets.SunTextureNames;

            var currentTexture = _sunTexture.objectReferenceValue as Texture2D;
            if (!enabledBefore && textures.Length > 0) currentTexture = textures[0];

            var textureIndex = Array.IndexOf(textures, currentTexture);
            if (textureIndex < 0) textureIndex = textures.Length;

            textureIndex = EditorGUILayout.Popup("Sun Texture", textureIndex, textureNames);

            if (textureIndex < textures.Length)
            {
                _sunTexture.objectReferenceValue = textures[textureIndex];
            }
            else
            {
                if (ArrayUtility.Contains(textures, currentTexture)) _sunTexture.objectReferenceValue = null;
                EditorGUILayout.PropertyField(_sunTexture, _emptyGUIContent);
            }
        }

        private void Moon1TexturePopup(bool enabledBefore)
        {
            var textures = _presets.MoonTextures;
            var textureNames = _presets.MoonTextureNames;

            var currentTexture = _moon1Texture.objectReferenceValue as Texture2D;
            if (!enabledBefore && textures.Length > 0) currentTexture = textures[0];

            var textureIndex = Array.IndexOf(textures, currentTexture);
            if (textureIndex < 0) textureIndex = textures.Length;

            textureIndex = EditorGUILayout.Popup("Moon 1 Texture", textureIndex, textureNames);

            if (textureIndex < textures.Length)
            {
                _moon1Texture.objectReferenceValue = textures[textureIndex];
            }
            else
            {
                if (ArrayUtility.Contains(textures, currentTexture)) _moon1Texture.objectReferenceValue = null;
                EditorGUILayout.PropertyField(_moon1Texture, _emptyGUIContent);
            }
        }

        private void Moon2TexturePopup(bool enabledBefore)
        {
            var textures = _presets.MoonTextures;
            var textureNames = _presets.MoonTextureNames;

            var currentTexture = _moon2Texture.objectReferenceValue as Texture2D;
            if (!enabledBefore && textures.Length > 0) currentTexture = textures[0];

            var textureIndex = Array.IndexOf(textures, currentTexture);
            if (textureIndex < 0) textureIndex = textures.Length;

            textureIndex = EditorGUILayout.Popup("Moon 2 Texture", textureIndex, textureNames);

            if (textureIndex < textures.Length)
            {
                _moon2Texture.objectReferenceValue = textures[textureIndex];
            }
            else
            {
                if (ArrayUtility.Contains(textures, currentTexture)) _moon2Texture.objectReferenceValue = null;
                EditorGUILayout.PropertyField(_moon2Texture, _emptyGUIContent);
            }
        }

        private void CloudsCubemapPopup(bool enabledBefore)
        {
            var cubemaps = _presets.CloudsCubemaps;
            var cubemapNames = _presets.CloudsCubemapNames;

            var currentCubemap = _cloudsCubemap.objectReferenceValue as Cubemap;
            if (!enabledBefore && cubemaps.Length > 0) currentCubemap = cubemaps[0];

            var cubemapIndex = Array.IndexOf(cubemaps, currentCubemap);
            if (cubemapIndex < 0) cubemapIndex = cubemaps.Length;

            cubemapIndex = EditorGUILayout.Popup("Clouds Cubemap", cubemapIndex, cubemapNames);

            if (cubemapIndex < cubemaps.Length)
            {
                _cloudsCubemap.objectReferenceValue = cubemaps[cubemapIndex];
            }
            else
            {
                if (ArrayUtility.Contains(cubemaps, currentCubemap)) _cloudsCubemap.objectReferenceValue = null;
                EditorGUILayout.PropertyField(_cloudsCubemap, _emptyGUIContent);
            }
        }
    }
}