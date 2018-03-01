using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ObstacleMovement))]
public class ObstacleMovementEditor : Editor
{
    private ObstacleMovement script = null;
    SerializedProperty e_bob;
    SerializedProperty e_bobSpeed;
    SerializedProperty e_bobStrength;
    SerializedProperty e_sway;
    SerializedProperty e_swaySpeed;
    SerializedProperty e_swayStrength;
    SerializedProperty e_teeter;
    SerializedProperty e_teeterSpeed;
    SerializedProperty e_maxTeeter;
    SerializedProperty e_rightLeft;
    SerializedProperty e_rightLeftSpeed;
    SerializedProperty e_maxRightLeftDist;
    SerializedProperty e_upDown;
    SerializedProperty e_upDownSpeed;
    SerializedProperty e_maxUpDownDist;
    SerializedProperty e_flipX;
    SerializedProperty e_flipY;

    private void OnEnable()
    {
        script = (ObstacleMovement)target;
        e_bob = serializedObject.FindProperty("bob");
        e_bobSpeed = serializedObject.FindProperty("bobspeed");
        e_bobStrength = serializedObject.FindProperty("bobStrength");
        e_sway = serializedObject.FindProperty("sway");
        e_swaySpeed = serializedObject.FindProperty("swaySpeed");
        e_swayStrength = serializedObject.FindProperty("swayStrength");
        e_teeter = serializedObject.FindProperty("teeter");
        e_teeterSpeed = serializedObject.FindProperty("teeterSpeed");
        e_maxTeeter = serializedObject.FindProperty("maxTeeter");
        e_rightLeft = serializedObject.FindProperty("rightLeft");
        e_rightLeftSpeed = serializedObject.FindProperty("rightLeftSpeed");
        e_maxRightLeftDist = serializedObject.FindProperty("maxRightLeftDist");
        e_upDown = serializedObject.FindProperty("upDown");
        e_upDownSpeed = serializedObject.FindProperty("upDownSpeed");
        e_maxUpDownDist = serializedObject.FindProperty("maxUpDownDist");
        e_flipX = serializedObject.FindProperty("flipX");
        e_flipY = serializedObject.FindProperty("flipY");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("Object bobs up and down");
        EditorGUILayout.PropertyField(e_bob);
        if (script.bob)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed", GUILayout.Width(150));
            script.bobSpeed = EditorGUILayout.Slider(script.bobSpeed, 0, 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Strength", GUILayout.Width(150));
            script.bobStrength = EditorGUILayout.Slider(script.bobStrength, 0, 1);
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("Object sways back and forth");
        EditorGUILayout.PropertyField(e_sway);
        if (script.sway)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed", GUILayout.Width(150));
            script.swaySpeed = EditorGUILayout.Slider(script.swaySpeed, 0, 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Strength", GUILayout.Width(150));
            script.swayStrength = EditorGUILayout.Slider(script.swayStrength, 0, 1);
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("Object teeters from side to side");
        EditorGUILayout.PropertyField(e_teeter);
        if (script.teeter)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed", GUILayout.Width(150));
            script.teeterSpeed = EditorGUILayout.Slider(script.teeterSpeed, 0, 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Angle", GUILayout.Width(150));
            script.maxTeeter = EditorGUILayout.Slider(script.maxTeeter, 5, 90);
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("Object moves right and left");
        EditorGUILayout.PropertyField(e_rightLeft);
        if (script.rightLeft)
        {
            script.flipX = EditorGUILayout.Toggle("Flips Sprite on X Axis", script.flipX);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed", GUILayout.Width(150));
            script.rightLeftSpeed = EditorGUILayout.Slider(script.rightLeftSpeed, 0, 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance in units", GUILayout.Width(150));
            script.maxRightLeftDist = EditorGUILayout.Slider(script.maxRightLeftDist, 2, 10);
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("Object moves up and down");
        EditorGUILayout.PropertyField(e_upDown);
        if (script.upDown)
        {
            script.flipY = EditorGUILayout.Toggle("Flips Sprite on Y Axis", script.flipY);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed", GUILayout.Width(150));
            script.upDownSpeed = EditorGUILayout.Slider(script.upDownSpeed, 0, 10);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Distance in units", GUILayout.Width(150));
            script.maxUpDownDist = EditorGUILayout.Slider(script.maxUpDownDist, 2, 10);
            GUILayout.EndHorizontal();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif