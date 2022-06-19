using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CandiceAIforGames.AI
{
    [InitializeOnLoad]
    public class CandiceAutorun
    {
        static string storagePath;
        static CandiceAutorun()
        {
            EditorApplication.update += Update;
            storagePath = Application.persistentDataPath + "/candiceAutorun.txt";
        }
        static void Update()
        {
            EditorApplication.update -= Update;
            double time = EditorApplication.timeSinceStartup;
            if (time < 40)
            {
                object obj = LoadFromFile();

                if (obj == null)
                {
                    LaunchStartupWindow();
                }
                else
                {
                    int i = Convert.ToInt32(obj.ToString());
                    if (i == 1)
                    {
                        LaunchStartupWindow();
                    }
                    else
                    {

                    }
                }
            }
        }
        static void LaunchStartupWindow()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorWindow window = EditorWindow.GetWindow<StartupWindow>();
                window.titleContent = new GUIContent("Candice AI for Games");
                window.minSize = new Vector2(700, 550);
                window.maxSize = new Vector2(700, 550);
                window.Show();
            }
        }


        public static bool SaveToFile(string data)
        {

            bool isSaved = false;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                if (File.Exists(storagePath))
                {
                    File.Delete(storagePath);
                }
                FileStream file = File.Create(storagePath);
                bf.Serialize(file, data);
                file.Close();
                isSaved = true;
            }
            catch (Exception e)
            {
                Debug.Log("AUTORUN ERROR: " + e.Message);
            }
            return isSaved;

        }

        public static object LoadFromFile()
        {
            object obj = null;
            try
            {
                if (File.Exists(storagePath))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(storagePath, FileMode.Open);
                    obj = bf.Deserialize(file);
                    file.Close();
                }
            }
            catch (Exception e)
            {
                Debug.Log("CANDICE AUTORUN ERROR: " + e.Message);
            }
            return obj;
        }
    }
}

