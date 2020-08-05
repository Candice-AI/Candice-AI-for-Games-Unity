using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ViridaxGameStudios.AI
{
    public class TileGridGenerator : EditorWindow
    {
        GameObject tilePrefab;
        Transform rootTransform;
        Tile tile;
        int sizeX = 12;
        int sizeZ = 12;
        [MenuItem("Window/Viridax Game Studios/Tools/Tile Grid Generator")]
        public static void AddController()
        {
            ShowWindow();
        }

        static void ShowWindow()
        {
            EditorWindow window = GetWindow<TileGridGenerator>();
            window.titleContent = new GUIContent("Tile Grid Generator");
            window.Show();
        }

        private void OnGUI()
        {
            GUIContent label;
            label = new GUIContent("Size X", "");
            sizeX = EditorGUILayout.IntField(label, sizeX);
            label = new GUIContent("Size Z", "");
            sizeZ = EditorGUILayout.IntField(label, sizeZ);
            label = new GUIContent("Tile Prefab", "");
            tilePrefab = (GameObject)EditorGUILayout.ObjectField(label, tilePrefab, typeof(GameObject), true);
            label = new GUIContent("Root Transform", "The transform used as a reference to position all the tiles. Preferably an empty GameObject");
            rootTransform = (Transform)EditorGUILayout.ObjectField(label, rootTransform, typeof(Transform), true);
            if (GUILayout.Button("Generate"))
            {
                if(rootTransform != null && tilePrefab != null)
                {
                    GenerateTileGrid();
                }
                else
                {
                    EditorUtility.DisplayDialog("Tile Grid Generator", "You must select a Root Transform and Tile Prefab for the Grid. ", "Okay");
                }
                
            }
        }

        void GenerateTileGrid()
        {
            for(int i = 0; i < sizeZ; i++)
            {
                GameObject row = new GameObject("Row " + (i+1));
                row.transform.parent = rootTransform;
                row.transform.position = new Vector3(0, 0, i);
                float x = tilePrefab.transform.localScale.x;
                float z = tilePrefab.transform.localScale.z;
                
                for(int a = 0; a < sizeX; a++)
                {
                    GameObject tile = Instantiate(tilePrefab);
                    tile.name = "Tile " + (a + 1);
                    tile.transform.SetParent(row.transform);
                    tile.transform.position = new Vector3(a*x, 0, i*z);
                }
            }
        }
    }
}

