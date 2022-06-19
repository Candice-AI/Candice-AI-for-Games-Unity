using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CandiceAIforGames.AI.Pathfinding;

namespace CandiceAIforGames.AI
{
    [CustomEditor(typeof(CandiceAIManager))]
    public class CandiceAIManager_Editor : Editor
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
            guiStyle.normal.textColor = EditorStyles.label.normal.textColor;
            GUILayout.Label(label, guiStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
            GUILayout.Label("Agent Count: " + (manager.agents.Count));
            manager.DrawGridGizmos = EditorGUILayout.Toggle("Draw Grid Gizmos", manager.DrawGridGizmos);
            manager.DrawAllAgentPaths = EditorGUILayout.Toggle("Draw All Agent Paths", manager.DrawAllAgentPaths);
            manager.grid = (CandiceGrid)EditorGUILayout.ObjectField("Grid", manager.grid, typeof(CandiceGrid), true);




        }
    }
}

