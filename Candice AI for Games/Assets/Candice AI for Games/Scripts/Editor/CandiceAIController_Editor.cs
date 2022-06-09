using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace CandiceAIforGames.AI.Editors
{
    [CustomEditor(typeof(CandiceAIController))]
    public class CandiceAIController_Editor : Editor
    {

        private CandiceAIController character;
        private SerializedObject soTarget;
        private SerializedProperty soBehaviorTree;

        GUIContent[] arrTabs = new GUIContent[4];
        GUIContent[] arrTabsSettings = new GUIContent[2];
        string[] arrSettingsTabs = { "General", "Relationships" };
        private int tabIndex;
        private int settingTabIndex;
        GUIStyle guiStyle = new GUIStyle();
        private bool showTags = true;
        bool showAllyTags = true;
        bool showEnemyTags = true;
        private int tagCount = 0;
        private int enemyTagCount;
        private int allyTagCount;
        void OnEnable()
        {
            //Store a reference to the AI Controller script
            character = (CandiceAIController)target;
            soTarget = new SerializedObject(character);
            soBehaviorTree = soTarget.FindProperty("behaviorTree");
            guiStyle.fontSize = 14;
            guiStyle.fontStyle = FontStyle.Bold;

            arrTabs[0] = new GUIContent("Settings", (Texture2D)Resources.Load("Settings"));
            arrTabs[1] = new GUIContent("Detection", (Texture2D)Resources.Load("Detection"));
            arrTabs[2] = new GUIContent("Movement", (Texture2D)Resources.Load("Movement"));
            arrTabs[3] = new GUIContent("Combat", (Texture2D)Resources.Load("Combat"));

            arrTabsSettings[0] = new GUIContent("General");
            arrTabsSettings[1] = new GUIContent("Relationships");

        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            GUIStyle style = new GUIStyle();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            style.normal.textColor = Color.red;
            Texture2D image = (Texture2D)Resources.Load("CandiceLogo");
            GUIContent label = new GUIContent(image);
            GUILayout.Label(label);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            label = new GUIContent("Candice AI Controller");
            guiStyle.normal.textColor = EditorStyles.label.normal.textColor;
            GUILayout.Label(label, guiStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
            EditorGUI.BeginChangeCheck();

            //tabIndex = GUILayout.Toolbar(tabIndex, arrTabs);

            tabIndex = GUILayout.SelectionGrid(tabIndex, arrTabs, 4);

            if (EditorGUI.EndChangeCheck())
            {
                GUI.FocusControl(null);
            }
            GUILayout.Space(8);
            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();
            switch (tabIndex)
            {
                case 0:
                    DrawAISettingGUI();
                    break;
                case 1:
                    DrawDetectionGUI();

                    break;
                case 2:
                    DrawMovementGUI();
                    break;
                case 3:
                    DrawCombatGUI();
                    break;
            }
            if (EditorGUI.EndChangeCheck())
            {

            }

            GUILayout.EndVertical();

        }

        void DrawAISettingGUI()
        {

            EditorGUI.BeginChangeCheck();

            //tabIndex = GUILayout.Toolbar(tabIndex, arrTabs);
            settingTabIndex = GUILayout.SelectionGrid(settingTabIndex, arrTabsSettings, 2);

            if (EditorGUI.EndChangeCheck())
            {
                GUI.FocusControl(null);
            }
            GUILayout.Space(8);
            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();
            switch (settingTabIndex)
            {
                case 0:
                    DrawGeneralGUI();
                    break;
                case 1:
                    DrawRelationshipGUI();
                    break;
            }
            GUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
            }





        }

        private void DrawGeneralGUI()
        {
            GUIContent label;
            label = new GUIContent("Agent ID", "The unique ID of the agent. Automatically generated at runtime.");
            EditorGUILayout.TextField(label, character.AgentID.ToString());
            label = new GUIContent("Max Hit Points", ".");
            character.MaxHitPoints = EditorGUILayout.FloatField(label, character.MaxHitPoints);
            label = new GUIContent("Current Hit Points", "");
            character.HitPoints = EditorGUILayout.FloatField(label, character.HitPoints);
            label = new GUIContent("Is 3D", "Uncheck if this character is in 2D space.");
            character.Is3D = EditorGUILayout.Toggle(label, character.Is3D);
            label = new GUIContent("Health Bar", "");
            //character.healthBar = (HealthBarScript)EditorGUILayout.ObjectField(label, character.healthBar, typeof(HealthBarScript), true);
            label = new GUIContent("Behavior Tree", "");
            EditorGUILayout.PropertyField(soBehaviorTree,label);
            //character.BehaviorTree = (CandiceBehaviorTree)EditorGUILayout.ObjectField(label, character.BehaviorTree, typeof(CandiceBehaviorTree), true);
            label = new GUIContent("Stop Behavior Tree", "Stops the current active behavior tree from executing.");
            //character._stopBehaviorTree = EditorGUILayout.Toggle(label, character._stopBehaviorTree);
            EditorGUILayout.Vector3Field("Move Point", character.MovePoint);
            EditorGUILayout.ObjectField("Main Target", character.MainTarget, typeof(GameObject), true);
            EditorGUILayout.ObjectField("Attack Target", character.AttackTarget, typeof(GameObject), true);
        }
        void DrawRelationshipGUI()
        {
            GUIContent label = new GUIContent("Faction", "The faction that this character belongs to.");
            //character.faction = (Faction)EditorGUILayout.ObjectField(label, character.faction, typeof(Faction), true);
            label = new GUIContent("Allies:", "All the Tags that the character will consider as an ally. NOTE: The default reaction is to follow.");
            EditorGUILayout.LabelField(label, guiStyle);
            showAllyTags = EditorGUILayout.Foldout(showAllyTags, label);
            if (showAllyTags)
            {
                allyTagCount = character.AllyTags.Count;
                allyTagCount = EditorGUILayout.IntField("Size: ", allyTagCount);

                if (allyTagCount != character.AllyTags.Count)
                {
                    int i = 0;
                    while (allyTagCount > character.AllyTags.Count)
                    {
                        character.AllyTags.Add("Ally" + i);
                        i++;
                    }
                    while (allyTagCount < character.AllyTags.Count)
                    {
                        character.AllyTags.RemoveAt(character.AllyTags.Count - 1);
                    }
                }

                for (int i = 0; i < character.AllyTags.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Element " + i);
                    string tag = "";
                    tag = EditorGUILayout.TagField(character.AllyTags[i]);

                    if (character.EnemyTags.Contains(tag))
                    {
                        EditorUtility.DisplayDialog("AI Controller", "Tag '" + tag + "' already added to enemy tags", "OK");
                    }
                    else
                    {
                        character.AllyTags[i] = tag;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
            //Enemy Relationships
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            label = new GUIContent("Enemies", "All the Tags that the character will consider as an enemy. NOTE: The default reaction is to attack.");
            EditorGUILayout.LabelField(label, guiStyle);
            showEnemyTags = EditorGUILayout.Foldout(showEnemyTags, label);
            if (showEnemyTags)
            {
                enemyTagCount = character.EnemyTags.Count;
                enemyTagCount = EditorGUILayout.IntField("Size", enemyTagCount);

                if (enemyTagCount != character.EnemyTags.Count)
                {
                    int i = 0;
                    while (enemyTagCount > character.EnemyTags.Count)
                    {
                        character.EnemyTags.Add("Enemy" + i);
                        i++;
                    }
                    while (enemyTagCount < character.EnemyTags.Count)
                    {
                        character.EnemyTags.RemoveAt(character.EnemyTags.Count - 1);
                    }
                }

                for (int i = 0; i < character.EnemyTags.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Element " + i);
                    string tag = "";
                    tag = EditorGUILayout.TagField(character.EnemyTags[i]);

                    if (character.AllyTags.Contains(tag))
                    {
                        EditorUtility.DisplayDialog("AI Controller", "Tag '" + tag + "' already added to ally tags", "OK");
                    }
                    else
                    {
                        character.EnemyTags[i] = tag;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
        }

        void DrawDetectionGUI()
        {
            GUIContent label;
            GUILayout.Label("Detection Settings", guiStyle);

            label = new GUIContent("Sensor Type", "The sensor type this agent will use. Note: Sphere sensor also works with 2D.");
            character.SensorType = (SensorType)EditorGUILayout.EnumPopup(label, character.SensorType);

            label = new GUIContent("Detection Radius", "The radius which the character can detect other objects.");
            character.DetectionRadius = EditorGUILayout.FloatField(label, character.DetectionRadius);
            label = new GUIContent("Detection Lines", "The amount of raycast lines the agent will emit in order to detect obstacles, evenly distrubuted from the center.");
            character.DetectionLines = EditorGUILayout.IntField(label, character.DetectionLines);
            label = new GUIContent("Detection Height", "The height at which the agent can detect objects.");
            character.DetectionHeight = EditorGUILayout.FloatField(label, character.DetectionHeight);
            label = new GUIContent("Line of Sight", "The angle, in degrees, in front of the agent where the it will be able to see objects.");
            character.LineOfSight = EditorGUILayout.FloatField(label, character.LineOfSight);

            label = new GUIContent("Detection Tags:", "All the Tags that the agent will detect.");
            showTags = EditorGUILayout.Foldout(showTags, label);

            if (showTags)
            {
                tagCount = character.ObjectTags.Count;
                tagCount = EditorGUILayout.IntField("Size: ", tagCount);

                if (tagCount != character.ObjectTags.Count)
                {
                    int i = 0;
                    while (tagCount > character.ObjectTags.Count)
                    {
                        character.ObjectTags.Add("Tag " + i);
                        i++;
                    }
                    while (tagCount < character.ObjectTags.Count)
                    {
                        character.ObjectTags.RemoveAt(character.ObjectTags.Count - 1);
                    }
                }
                for (int i = 0; i < character.ObjectTags.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Element " + i);
                    string tag = "";
                    tag = EditorGUILayout.TagField(character.ObjectTags[i]);

                    if (character.ObjectTags.Contains(tag))
                    {
                        //EditorUtility.DisplayDialog("AI Controller", "Tag '" + tag + "' already added to object tags list", "OK");
                    }
                    else
                    {
                        character.ObjectTags[i] = tag;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }



            }
            EditorGUILayout.Space();
            GUILayout.Label("Obstacle Avoidance Settings", guiStyle);
            label = new GUIContent("OA Perception Mask", "Layers that the agent must avoid while moving");
            LayerMask tempMask = EditorGUILayout.MaskField(label, InternalEditorUtility.LayerMaskToConcatenatedLayersMask(character.PerceptionMask), InternalEditorUtility.layers);
            character.PerceptionMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
            label = new GUIContent("OA Distance", "The maximum distance that the agent will start to avoid detected objects.");
            character.ObstacleAvoidaceDistance = EditorGUILayout.FloatField(label, character.ObstacleAvoidaceDistance);
            label = new GUIContent("OA AOE", "The Area Of Effect around the agent that must not touch obstacles");
            character.ObstacleAvoidanceAOE = EditorGUILayout.FloatField(label, character.ObstacleAvoidanceAOE);


        }

        void DrawMovementGUI()
        {
            //Detection and Head Look Settings
            GUILayout.Label("General Movement Settings", guiStyle);
            GUIContent label;
            EditorGUILayout.Vector3Field("Move Target", character.MovePoint);
            EditorGUILayout.ObjectField("Main Target", character.MainTarget, typeof(GameObject), true);
            label = new GUIContent("Base Movement Speed", "The base speed at which the agent will move at.");
            character.MoveSpeed = EditorGUILayout.FloatField(label, character.MoveSpeed);
            label = new GUIContent("Base Rotation Speed", "The base speed at which the agent rotate to face its target.");
            character.RotationSpeed = EditorGUILayout.FloatField(label, character.RotationSpeed);
            label = new GUIContent("Waypoint:", "The current Waypoint that the agent will follow.");
            character.Waypoint = (CandiceWaypoint)EditorGUILayout.ObjectField(label, character.Waypoint, typeof(CandiceWaypoint), true);
            GUILayout.Space(16);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Candice Pathfind Settings", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            label = new GUIContent("Turn Speed", "The speed the agent will turn between waypoints by when pathfinding.");
            character._turnSpeed = EditorGUILayout.FloatField(label, character._turnSpeed);
            label = new GUIContent("Turn Distance", "The ditance the agent will start to turn while moving to the next node.");
            character._turnDist = EditorGUILayout.FloatField(label, character._turnDist);
            label = new GUIContent("Stopping Distance", "How far away the agent will start to come to a halt.");
            character._stoppingDist = EditorGUILayout.FloatField(label, character._stoppingDist);
            label = new GUIContent("Minimum Path Update Time", "Minimum time it will take for the agent before attempting to request a new updated path from Candice.");
            character._minPathUpdateTime = EditorGUILayout.FloatField(label, character._minPathUpdateTime);
            label = new GUIContent("Path Update Move Threshold", "Minimum distance the target can move by before requesting a new Updated path from Candice.");
            character._pathUpdateMoveThreshold = EditorGUILayout.FloatField(label, character._pathUpdateMoveThreshold);
            character.DrawAgentPath = EditorGUILayout.Toggle("Draw Agent Path", character.DrawAgentPath);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            
        }

        void DrawCombatGUI()
        {
            GUILayout.Label("Attack Settings", guiStyle);
            GUIContent label;
            label = new GUIContent("Attack Range", "The range that the AI will start attacking enemies.");
            character.AttackRange = EditorGUILayout.FloatField(label, character.AttackRange);
            label = new GUIContent("Attack Projectile", "The projectile that the agent will fire.");
            character.Projectile = (GameObject)EditorGUILayout.ObjectField(label, character.Projectile, typeof(GameObject), true);
            label = new GUIContent("Projectile Spawn Position", "The point where the projectile will spawn. e.g the hand for a spell, or the bow for an arrow.");
            character.ProjectileSpawnPos = (Transform)EditorGUILayout.ObjectField(label, character.ProjectileSpawnPos, typeof(Transform), true);
            label = new GUIContent("Attack Damage", "The damage per attack that the agent will deal.");
            character.AttackDamage = EditorGUILayout.FloatField(label, character.AttackDamage);
            character.DamageAngle = EditorGUILayout.Slider("Damage Angle:", character.DamageAngle, 0, 360f);
            label = new GUIContent("Has Attack Animation", "Whether or not this agent has an attack animation.");
            character.HasAttackAnimation = EditorGUILayout.Toggle(label, character.HasAttackAnimation);
            label = new GUIContent("Attacks Per Second", "How many attacks per second the agent will deal");
            character.AttacksPerSecond = EditorGUILayout.FloatField(label, character.AttacksPerSecond);
        }

        
        void OnSceneGUI()
        {
            if (character != null)
            {
                //Call the necessary methods to draw the discs and handles on the editor
                if (character.Is3D)
                {
                    Color color = Color.cyan;
                    color.a = 0.25f;
                    character.DetectionRadius = DrawDiscs(color, character.transform.position, Vector3.up, -character.transform.forward, character.DetectionRadius, "Detection Radius", float.MaxValue);
                    color = Color.blue;
                    color.a = 0.25f;
                    character.LineOfSight = DrawArcs(color, character.transform.position, Vector3.up, character.transform.forward, character.transform.forward, character.LineOfSight, character.DetectionRadius, "Line of Sight");
                    color = Color.magenta;
                    color.a = 0.15f;
                    character.AttackRange = DrawDiscs(color, character.transform.position, Vector3.up, -character.transform.right, character.AttackRange, "Attack Range", character.DetectionRadius);
                    color = new Color(1f, 0f, 0f, 0.75f);//Red
                    character.DamageAngle = DrawArcs(color, character.transform.position, Vector3.up, character.transform.forward, character.transform.right, character.DamageAngle, character.AttackRange, "Damage Angle");

                }
                else
                {
                    Color color = new Color(1f, 0f, 0f, 0.15f);//Red
                    character.DetectionRadius = DrawDiscs(color, character.transform.position, Vector3.forward, character.transform.up, character.DetectionRadius, "Detection Radius", float.MaxValue);
                    color = new Color(0f, 0f, 1f, 0.35f);//Blue
                    character.AttackRange = DrawDiscs(color, character.transform.position, Vector3.forward, character.transform.right, character.AttackRange, "Attack Range", character.DetectionRadius);
                    color = new Color(1f, 0f, 0f, 0.75f);//Red
                    character.DamageAngle = DrawArcs(color, character.transform.position, Vector3.forward, character.transform.up, character.transform.up, character.DamageAngle, character.AttackRange, "Damage Angle");
                    color = new Color(0f, 1f, 0f, 0.35f);//Green
                    character.LineOfSight = DrawArcs(color, character.transform.position, Vector3.forward, character.transform.up, character.transform.forward, character.LineOfSight, character.DetectionRadius, "Line of Sight");
                }

            }

        }

        protected float DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, float radius, float maxValue)
        {
            //
            //Method Name : void DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius)
            //Purpose     : This method draws the necessary discs and slider handles in the editor to adjust the attack range and detection radius.
            //Re-use      : none
            //Input       : Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius
            //Output      : none
            //
            //Draw the disc that will represent the detection radius
            Handles.color = color;
            Handles.DrawSolidDisc(center, normal, character.DetectionRadius);
            Handles.color = new Color(1f, 1f, 0f, 0.75f);
            Handles.DrawWireDisc(center, normal, radius);

            //Create Slider handles to adjust detection radius properties
            color.a = 0.5f;
            Handles.color = color;
            radius = Handles.ScaleSlider(radius, character.transform.position, direction, Quaternion.identity, radius, 1f);
            radius = Mathf.Clamp(radius, 1f, maxValue);
            return radius;


        }

        protected float DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, float radius, string label, float maxValue)
        {
            //
            //Method Name : void DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius, string label)
            //Purpose     : Overloaded method of DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius)
            //              that adds the necessary labels. 
            //Re-use      : DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius)
            //Input       : Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius, string label
            //Output      : none
            //

            radius = DrawDiscs(color, center, normal, direction, radius, maxValue);
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 12;
            labelStyle.normal.textColor = new Color(color.r, color.g, color.b, 1);
            labelStyle.alignment = TextAnchor.UpperCenter;
            Handles.Label(character.transform.position + (direction * radius), label, labelStyle);
            return radius;
        }

        protected float DrawArcs(Color color, Vector3 center, Vector3 normal, Vector3 direction, Vector3 sliderDirection, float angle, float radius, string label)
        {
            //
            //Method Name : void DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius)
            //Purpose     : This method draws the necessary discs and slider handles in the editor to adjust the attack range and detection radius.
            //Re-use      : none
            //Input       : Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius
            //Output      : none
            //
            //Draw the disc that will represent the detection radius

            Handles.color = color;
            Vector3 newDirection = character.transform.forward - (character.transform.right);
            Handles.DrawSolidArc(center, normal, direction, angle / 2, radius);
            Handles.DrawSolidArc(center, normal, direction, -angle / 2, radius);
            Handles.color = new Color(1f, 1f, 0f, 0.75f);
            Handles.DrawWireArc(center, normal, newDirection, angle, radius);

            //Create Slider handles to adjust detection radius properties
            color.a = 0.5f;
            Handles.color = color;
            angle = Handles.ScaleSlider(angle, character.transform.position, sliderDirection, Quaternion.identity, radius, 1f);
            angle = Mathf.Clamp(angle, 1f, 360);

            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 12;
            labelStyle.normal.textColor = new Color(color.r, color.g, color.b, 1);
            labelStyle.alignment = TextAnchor.UpperCenter;
            Handles.Label(character.transform.position + (sliderDirection * radius), label, labelStyle);
            return angle;
        }
    }
}