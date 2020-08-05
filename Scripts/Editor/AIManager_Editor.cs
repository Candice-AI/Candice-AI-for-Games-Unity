using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace ViridaxGameStudios.AI
{
    [CustomEditor(typeof(CandiceAIManager))]
    public class AIManager_Editor : Editor
    {
        private CandiceAIManager manager;
        private SerializedObject soTarget;
        GUIStyle guiStyle = new GUIStyle();

        void OnEnable()
        {
            //Store a reference to the AI Controller script
            manager = (CandiceAIManager)target;
            soTarget = new SerializedObject(manager);

            guiStyle.fontSize = 14;
            guiStyle.fontStyle = FontStyle.Bold;


        }
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Texture candiceLogo = Resources.Load<Texture2D>("CandiceLogo");
            GUILayout.Label(candiceLogo);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIContent label = new GUIContent("Candice AI Manager");
            GUILayout.Label(label, guiStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
            manager.enableDebug = EditorGUILayout.Toggle("Enable Debug Mode", manager.enableDebug);
            manager.grid = (Grid)EditorGUILayout.ObjectField("Grid", manager.grid, typeof(Grid), true);
            GUILayout.Label("Agent Count: " + manager.agentCount);



        }
    }
}

