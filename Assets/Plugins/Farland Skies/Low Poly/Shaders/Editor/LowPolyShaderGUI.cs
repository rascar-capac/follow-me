using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

public class LowPolyShaderGUI : ShaderGUI
{
    // Sky
    private MaterialProperty _topColor;
    private MaterialProperty _middleColor;
    private MaterialProperty _bottomColor;
    private MaterialProperty _topExponent;
    private MaterialProperty _bottomExponent;
    // Stars
    private MaterialProperty _starsTint;
    private MaterialProperty _starsExtinction;
    private MaterialProperty _starsTwinklingSpeed;
    private MaterialProperty _starsTex;
    // Sun
    private MaterialProperty _sunSize;
    private MaterialProperty _sunHalo;
    private MaterialProperty _sunTint;
    private MaterialProperty _sunTex;
    // Moon 1
    private MaterialProperty _moon1Size;
    private MaterialProperty _moon1Halo;
    private MaterialProperty _moon1Tint;
    private MaterialProperty _moon1Tex;
    // Moon 2
    private MaterialProperty _moon2Size;
    private MaterialProperty _moon2Halo;
    private MaterialProperty _moon2Tint;
    private MaterialProperty _moon2Tex;
    // Clouds
    private MaterialProperty _cloudsTint;
    private MaterialProperty _cloudsRotation;
    private MaterialProperty _cloudsHeight;
    private MaterialProperty _cloudsTex;
    // General
    private MaterialProperty _exposure;

    //---------------------------------------------------------------------
    // Public
    //---------------------------------------------------------------------

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        FindProperties(properties);
        ShaderPropertiesGUI(materialEditor);
    }

    //---------------------------------------------------------------------
    // Helpers
    //---------------------------------------------------------------------

    private void FindProperties(MaterialProperty[] properties)
    {
        // Sky
        _topColor = FindProperty("_TopColor", properties);
        _middleColor = FindProperty("_MiddleColor", properties);
        _bottomColor = FindProperty("_BottomColor", properties);
        _topExponent = FindProperty("_TopExponent", properties);
        _bottomExponent = FindProperty("_BottomExponent", properties);
        // Stars
        _starsTint = FindProperty("_StarsTint", properties);
        _starsExtinction = FindProperty("_StarsExtinction", properties);
        _starsTwinklingSpeed = FindProperty("_StarsTwinklingSpeed", properties);
        _starsTex = FindProperty("_StarsTex", properties);
        // Sun
        _sunSize = FindProperty("_SunSize", properties);
        _sunHalo = FindProperty("_SunHalo", properties);
        _sunTint = FindProperty("_SunTint", properties);
        _sunTex = FindProperty("_SunTex", properties);
        // Moon 1
        _moon1Size = FindProperty("_Moon1Size", properties);
        _moon1Halo = FindProperty("_Moon1Halo", properties);
        _moon1Tint = FindProperty("_Moon1Tint", properties);
        _moon1Tex = FindProperty("_Moon1Tex", properties);
        // Moon 2
        _moon2Size = FindProperty("_Moon2Size", properties);
        _moon2Halo = FindProperty("_Moon2Halo", properties);
        _moon2Tint = FindProperty("_Moon2Tint", properties);
        _moon2Tex = FindProperty("_Moon2Tex", properties);
        // Clouds
        _cloudsTint = FindProperty("_CloudsTint", properties);
        _cloudsRotation = FindProperty("_CloudsRotation", properties);
        _cloudsHeight = FindProperty("_CloudsHeight", properties);
        _cloudsTex = FindProperty("_CloudsTex", properties);
        // General
        _exposure = FindProperty("_Exposure", properties);
    }

    private void ShaderPropertiesGUI(MaterialEditor materialEditor)
    {
        materialEditor.SetDefaultGUIWidths();
        var targetMat = materialEditor.target as Material;

        // Sky
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Sky", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        materialEditor.ShaderProperty(_topColor, "Color Top");
        materialEditor.ShaderProperty(_middleColor, "Color Middle");
        materialEditor.ShaderProperty(_bottomColor, "Color Bottom");
        EditorGUILayout.Space();
        materialEditor.ShaderProperty(_topExponent, "Exponent Top");
        materialEditor.ShaderProperty(_bottomExponent, "Exponent Bottom");
        EditorGUILayout.Space();

        // Stars
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("Stars", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        var starsOn = KeywordCheckbox(targetMat, "STARS_OFF");
        EditorGUILayout.EndHorizontal();

        if (starsOn)
        {
            materialEditor.ShaderProperty(_starsTint, "Stars Tint");
            materialEditor.ShaderProperty(_starsExtinction, "Stars Extinction");
            materialEditor.ShaderProperty(_starsTwinklingSpeed, "Stars Twinkling Speed");
            materialEditor.ShaderProperty(_starsTex, "Stars Cubemap");
        }
        EditorGUILayout.Space();

        // Sun
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("Sun", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        var sunOn = KeywordCheckbox(targetMat, "SUN_OFF");
        EditorGUILayout.EndHorizontal();

        if (sunOn)
        {
            materialEditor.ShaderProperty(_sunSize, "Sun Size");
            materialEditor.ShaderProperty(_sunHalo, "Sun Halo");
            materialEditor.ShaderProperty(_sunTint, "Sun Tint");
            materialEditor.ShaderProperty(_sunTex, "Sun Texture");
        }
        EditorGUILayout.Space();

        // Moon 1
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("Moon 1", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        var moon1On = KeywordCheckbox(targetMat, "MOON1_OFF");
        EditorGUILayout.EndHorizontal();

        if (moon1On)
        {
            materialEditor.ShaderProperty(_moon1Size, "Moon 1 Size");
            materialEditor.ShaderProperty(_moon1Halo, "Moon 1 Halo");
            materialEditor.ShaderProperty(_moon1Tint, "Moon 1 Tint");
            materialEditor.ShaderProperty(_moon1Tex, "Moon 1 Texture");
        }
        EditorGUILayout.Space();

        // Moon 2
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("Moon 2", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        var moon2On = KeywordCheckbox(targetMat, "MOON2_OFF");
        EditorGUILayout.EndHorizontal();

        if (moon2On)
        {
            materialEditor.ShaderProperty(_moon2Size, "Moon 2 Size");
            materialEditor.ShaderProperty(_moon2Halo, "Moon 2 Halo");
            materialEditor.ShaderProperty(_moon2Tint, "Moon 2 Tint");
            materialEditor.ShaderProperty(_moon2Tex, "Moon 2 Texture");
        }
        EditorGUILayout.Space();

        // Clouds
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField("Clouds", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        var cloudsOn = KeywordCheckbox(targetMat, "CLOUDS_OFF");
        EditorGUILayout.EndHorizontal();

        if (cloudsOn)
        {
            materialEditor.ShaderProperty(_cloudsTint, "Clouds Tint");
            materialEditor.ShaderProperty(_cloudsRotation, "Clouds Rotation");
            materialEditor.ShaderProperty(_cloudsHeight, "Clouds Height");
            materialEditor.ShaderProperty(_cloudsTex, "Clouds Cubemap");
        }
        EditorGUILayout.Space();

        // General
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        materialEditor.ShaderProperty(_exposure, "Exposure");

        EditorGUILayout.Space();
    }

    [SuppressMessage("ReSharper", "InvertIf")]
    private static bool KeywordCheckbox(Material material, string keyword)
    {
        var keywordValue = Array.IndexOf(material.shaderKeywords, keyword) != -1;
        EditorGUI.BeginChangeCheck();
        var checkboxValue = EditorGUILayout.Toggle(!keywordValue, GUILayout.Width(60f));
        if (EditorGUI.EndChangeCheck())
        {
            if (checkboxValue)
                material.DisableKeyword(keyword);
            else
                material.EnableKeyword(keyword);
        }

        return checkboxValue;
    }
}