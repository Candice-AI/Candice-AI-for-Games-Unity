using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.AI;
using CandiceAIforGames.AI.Pathfinding;

namespace CandiceAIforGames.AI
{
    public class SetupAssistant_Menu : EditorWindow
    {
        private GameObject obj;
        private bool attachRigidBody = false;
        private bool attachCollider = false;
        private bool attachAnimator = false;
        private int colliderTypeIndex = 0;
        private int colliderTypeIndex2D = 0;
        private string[] arrColliderTypes = { "Capsule", "Box", "Sphere", "Mesh" };
        private string[] arrColliderTypes2D = { "Capsule", "Box", "Circle" };
        string[] arrAttackTypes = { "Melee", "Range" };
        private AttackType attackType;
        float m_AttackRange = 7f;
        private bool attachNavAgent = false;
        bool is3D = true;
        private bool hasAnims;
        bool isEnemy;
        private GameObject attackProjectile;
        private Transform spawnPosition;

        private GameObject rig;
        private GameObject healthCanvasPrefab;
        private Transform cam;
        private bool enableRagdoll = false;
        private bool ragdollOnDeath = true;
        bool hasAttackAnimation = true;
        float movementSpeed = 7f;

        //string[] moveTypes = {"Normal", "Tile Based"};
        Rect headerRect;
        Rect mainRect;
        GUIStyle guiStyle;
        bool drawingController = false;
        private MovementType moveType;
        private PathfindSource pathfindSource;

        [MenuItem("Window/Candice AI for Games/AI/Candice Setup Assistant")]
        public static void SetupAssistant()
        {
            ShowWindow();
        }
        static void ShowWindow()
        {
            EditorWindow window = GetWindow<SetupAssistant_Menu>();
            window.titleContent = new GUIContent("Candice Setup Assistant");
            window.minSize = new Vector2(600f, 150f);
            window.maxSize = new Vector2(600f, 150f);
            window.Show();
        }


        private void OnGUI()
        {
            if (drawingController)
            {
                DrawControllerSetup();
            }
            else
            {
                Texture candiceLogo = Resources.Load<Texture2D>("CandiceLogo");
                GUIContent content = new GUIContent(candiceLogo, "Setup the Candice AI Manager");
                Texture aiController = Resources.Load<Texture2D>("Controller");
                GUILayout.BeginHorizontal();
                GUIStyle style = new GUIStyle();
                style.fontSize = 14;
                GUILayout.Label("  AI Manager", style);
                GUILayout.Label("  AI Controller", style);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(content))
                {
                    AddManager();
                }
                content = new GUIContent(aiController, "Setup the Candice AI Controller");
                if (GUILayout.Button(content))
                {
                    AddController();
                }
                GUILayout.EndHorizontal();

            }

        }

        void DrawControllerSetup()
        {
            float width = Screen.width / 2 + Screen.width / 4 + Screen.width / 32;
            headerRect = new Rect(0, 0, width, 300f);
            mainRect = new Rect(0, headerRect.yMax, width, 500f);
            guiStyle = new GUIStyle();
            guiStyle.fontSize = 28;
            guiStyle.fontStyle = FontStyle.Bold;

            GUILayout.BeginVertical();//1

            GUILayout.Space(4);
            GUILayout.BeginArea(headerRect);

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Texture2D image = (Texture2D)Resources.Load("CandiceLogoWithText");
            GUIContent label = new GUIContent(image);
            GUILayout.Label(label);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            is3D = EditorGUILayout.Toggle("Is 3D", is3D);
            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.Space(2);
            GUILayout.BeginArea(mainRect);

            //Check if the object has a rigidbody
            GUILayout.BeginVertical("box");
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                GUILayout.Label("No Rigidbody detected. One will be automatically added");
                attachRigidBody = true;
            }
            else
            {
                GUILayout.Label("Rigidbody detected.");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            //Check if the object has a Collider
            Collider col = obj.GetComponent<Collider>();
            if (col == null)
            {
                GUILayout.Label("No Collider detected. One will be automatically added");
                if (is3D)
                    colliderTypeIndex = EditorGUILayout.Popup("Collider Type", colliderTypeIndex, arrColliderTypes);
                else
                    colliderTypeIndex2D = EditorGUILayout.Popup("Collider Type", colliderTypeIndex2D, arrColliderTypes2D);
                attachCollider = true;
            }
            else
            {
                EditorGUILayout.LabelField("Collider detected.");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            //Check if the object has a Animator
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null)
            {
                GUILayout.Label("No Animator detected. One will be automatically added");
                attachAnimator = true;
            }
            else
            {
                EditorGUILayout.LabelField("Animator detected.");
            }
            GUILayout.EndVertical();
            GUILayout.Space(16);
            label = new GUIContent();
            label = new GUIContent("Movement Speed", "The speed at which the agent will move at.");
            movementSpeed = EditorGUILayout.FloatField(label, movementSpeed);
            GUILayout.Space(8);


            label = new GUIContent("Rig", "The rig that contains all the bones of the character.");
            rig = (GameObject)EditorGUILayout.ObjectField(label, rig, typeof(GameObject), true);
            
            GUILayout.Space(8);

            label = new GUIContent("Attack Type", "");
            attackType = (AttackType)EditorGUILayout.EnumPopup(label, attackType);
            label = new GUIContent("Attack Range", "The range that the AI will start attacking enemies.");
            m_AttackRange = EditorGUILayout.FloatField(label, m_AttackRange);
            if (attackType == AttackType.Range)
            {
                label = new GUIContent("Attack Projectile", "The projectile that the agent will fire.");
                attackProjectile = (GameObject)EditorGUILayout.ObjectField(label, attackProjectile, typeof(GameObject), true);

                label = new GUIContent("Projectile Spawn Position", "The point where the projectile will spawn..");
                spawnPosition = (Transform)EditorGUILayout.ObjectField(label, spawnPosition, typeof(Transform), true);
            }
            label = new GUIContent("Has Attack Animation", "Whether or not this agent has an attack animation.");
            hasAttackAnimation = EditorGUILayout.Toggle(label, hasAttackAnimation);
            label = new GUIContent("Is Enemy", "Whether or not this agent is an enemy of the player.");
            isEnemy = EditorGUILayout.Toggle(label, isEnemy);
            guiStyle.fontSize = 10;
            GUILayout.Space(4);
            GUILayout.Label("*Note: You can still edit these, and other settings on the AI Controller component after it is created.", guiStyle);
            GUILayout.Space(4);
            if (GUILayout.Button("Add Controller"))
            {
                AttachAIControllerScript();
            }
            GUILayout.EndVertical();//1


            GUILayout.EndArea();
        }
        void AttachAIControllerScript()
        {
            //Assign AI Script to the GameObject
            if (attachRigidBody)
            {
                if (is3D)
                    obj.AddComponent<Rigidbody>();
                else
                    obj.AddComponent<Rigidbody2D>();
            }

            if (attachAnimator)
            {
                Animator a = obj.AddComponent<Animator>();
                a.applyRootMotion = false;
            }

            if (attachCollider)
            {
                if (is3D)
                {
                    switch (colliderTypeIndex)
                    {
                        case 0:
                            obj.AddComponent<CapsuleCollider>();
                            break;
                        case 1:
                            obj.AddComponent<BoxCollider>();
                            break;
                        case 2:
                            obj.AddComponent<SphereCollider>();
                            break;
                    }
                }
                else
                {
                    switch (colliderTypeIndex2D)
                    {
                        case 0:
                            obj.AddComponent<CapsuleCollider2D>();
                            break;
                        case 1:
                            obj.AddComponent<BoxCollider2D>();
                            break;
                        case 2:
                            obj.AddComponent<CircleCollider2D>();
                            break;
                    }
                }

            }
            obj.AddComponent<CandiceBehaviorTreeMono>();
            CandiceAIController aiScript = null;
            aiScript = obj.AddComponent<CandiceAIController>();
            aiScript.Is3D = is3D;
            aiScript.MoveSpeed = movementSpeed;
            if (isEnemy)
                aiScript.EnemyTags.Add("Player");
            aiScript.Rig = rig;
            aiScript.EnableRagdoll = enableRagdoll;

            aiScript.AttackType = attackType;
            aiScript.AttackRange = m_AttackRange;
            aiScript.Projectile = attackProjectile;
            aiScript.ProjectileSpawnPos = spawnPosition;
            aiScript.HasAttackAnimation = hasAttackAnimation;

            Selection.activeGameObject = obj;
            EditorUtility.DisplayDialog("Candice AI for Games", "Controller Setup Complete", "OK");
            Close();
        }

        
        public void AddController()
        {

            GameObject[] selectedGO = Selection.gameObjects;
            if (selectedGO.Length == 1)
            {
                obj = selectedGO[0];
                drawingController = true;
                this.minSize = new Vector2(650f, 700f);
                this.maxSize = new Vector2(650f, 700f);
                this.Repaint();
            }
            else
            {
                EditorUtility.DisplayDialog("Candice AI Controller", "You need to select 1 GameObject", "OK");
            }
        }
        public static void AddManager()
        {
            if (!ManagerExists())
            {
                GameObject go = GameObject.Find("Candice AI Manager");
                if (go != null)
                {

                }
                else
                {
                    go = new GameObject("Candice AI Manager");
                    go.AddComponent<CandiceAIManager>();
                    go.AddComponent<CandiceGrid>();
                }

            }
            else
            {
                EditorUtility.DisplayDialog("Candice AI for Games", "Candice AI Manager" + " already exists.", "OK");
            }
        }
        static bool ManagerExists()
        {
            bool isExists = false;
            GameObject[] arrGO = FindObjectsOfType<GameObject>();
            int count = 0;
            while (!isExists && count < arrGO.Length)
            {
                GameObject go = arrGO[count];
                CandiceAIManager aiManager = go.GetComponent<CandiceAIManager>();
                if (aiManager != null)
                {
                    isExists = true;
                }
                count++;
            }
            return isExists;
        }
    }
}