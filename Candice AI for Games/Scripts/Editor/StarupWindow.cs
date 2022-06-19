using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace CandiceAIforGames.AI
{
    public class StartupWindow : EditorWindow
    {
        Rect headerRect;
        Rect reviewRect;
        Rect patreonRect;
        Rect closeRect;
        public bool shouldLoad = true;
        public StartupWindow()
        {

        }
        private void OnGUI()
        {
            GUIStyle style = new GUIStyle();
            GUIContent label = new GUIContent();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Texture candiceLogo = Resources.Load<Texture2D>("CandiceLogoWithText");
            GUILayout.Label(candiceLogo);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(6);
            style.normal.textColor = EditorStyles.label.normal.textColor;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            label = new GUIContent("Thanks for downloading Candice AI for Games.\nIf you like this Plugin, please leave a review on the Asset store.");
            style.wordWrap = true;

            //style.normal.textColor = Color.gray;
            style.fontSize = 12;
            GUILayout.Label(label, style);
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(" Leave Review"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/slug/148441");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            style.fontSize = 14;
            GUILayout.Label("Support", style);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Texture logoDiscord = Resources.Load<Texture2D>("discord");
            if (GUILayout.Button(logoDiscord))
            {
                Application.OpenURL("https://discord.gg/GUtK6EH");
            }
            Texture logoFB = Resources.Load<Texture2D>("facebook");
            if (GUILayout.Button(logoFB))
            {
                Application.OpenURL("https://www.facebook.com/KandakeAI/");
            }
            Texture logoEmail = Resources.Load<Texture2D>("email");
            if (GUILayout.Button(logoEmail))
            {
                Application.OpenURL("mailto:support@candiceai.co.za?subject=Candice AI for Games");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(6);
            /*
            GUILayout.Label("Documentation & Tutorials", style);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Texture logoWeb = Resources.Load<Texture2D>("web");
            if(GUILayout.Button(logoWeb))
            {
                Application.OpenURL("");
            }
            Texture logoYoutube = Resources.Load<Texture2D>("youtube");
            if(GUILayout.Button(logoYoutube))
            {
                Application.OpenURL("https://www.youtube.com/channel/UC4mEN2a8tXhL32W1ll_h8EQ");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            */
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close"))
            {
                if (!shouldLoad)
                {
                    CandiceAutorun.SaveToFile("0");
                }
                else
                {
                    CandiceAutorun.SaveToFile("1");
                }
                Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            shouldLoad = EditorGUILayout.Toggle("Show on startup", shouldLoad);
            GUILayout.EndHorizontal();
        }

    }
}

