using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System;

namespace ViridaxGameStudios.AI
{
    [CustomEditor(typeof(AIController))]
    public class AIController_Editor : Editor
    {
        #region variables
        private AIController character;
        private SerializedObject soTarget;

        string[] arrAttackTypes = {"Melee", "Range"};
        string[] arrSettingsTabs = { "General", "Key Mapping", "Relationships" };
        //string[] arrTabs = { "AI Settings", "Stats", "Detection", "Movement", "Combat", "Animation" };
        GUIContent[] arrTabs = new GUIContent[6];

        private int tabIndex;
        private int settingTabIndex;
        private int enemyTagCount;
        private int patrolPointCount;
        private int allyTagCount;
        GUIStyle guiStyle = new GUIStyle();
        bool showPatrolPoints = false;
        bool showAllyTags = false;
        bool showEnemyTags = false;
        bool showStrength = true;

        #endregion

        #region Main Methods
        void OnEnable()
        {
            //Store a reference to the AI Controller script
            character = (AIController)target;
            soTarget = new SerializedObject(character);
            
            guiStyle.fontSize = 14;
            guiStyle.fontStyle = FontStyle.Bold;

            arrTabs[0] = new GUIContent("Settings",(Texture2D)Resources.Load("Settings"));
            arrTabs[1] = new GUIContent("  Stats",(Texture2D)Resources.Load("Stats"));
            arrTabs[2] = new GUIContent("Detection", (Texture2D)Resources.Load("Detection"));
            arrTabs[3] = new GUIContent("Movement", (Texture2D)Resources.Load("Movement"));
            arrTabs[4] = new GUIContent("Combat", (Texture2D)Resources.Load("Combat"));
            arrTabs[5] = new GUIContent("Animation", (Texture2D)Resources.Load("Animation"));

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
            
            tabIndex = GUILayout.SelectionGrid(tabIndex, arrTabs, 3);
            
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
                    DrawStatsGUI();

                    break;
                case 2:
                    DrawDetectionGUI();
                    break;
                case 3:
                    DrawMovementGUI();
                    break;
                case 4:
                    DrawCombatGUI();
                    break;
                case 5:
                    DrawAnimationGUI();
                    break;
            }
            if (EditorGUI.EndChangeCheck())
            {

            }
            GUILayout.EndVertical();

        }

        

        #region DRAW TAB REGION
        void DrawAISettingGUI()
        {
            
            EditorGUI.BeginChangeCheck();

            //tabIndex = GUILayout.Toolbar(tabIndex, arrTabs);
            settingTabIndex = GUILayout.SelectionGrid(settingTabIndex, arrSettingsTabs, 3);

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
                    DrawKeyMapGUI();

                    break;
                case 2:
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
            EditorGUILayout.TextField(label, character.agentID.ToString());
            label = new GUIContent("Is 3D", "Uncheck if this character is in 2D space.");
            character.is3D = EditorGUILayout.Toggle(label, character.is3D);
            label = new GUIContent("Has Animations", "Whether or not this agent has any animations (e.g move, jump, attack...)");
            character.hasAnimations = EditorGUILayout.Toggle(label, character.hasAnimations);
            label = new GUIContent("Health Bar", "");
            character.healthBar = (HealthBarScript)EditorGUILayout.ObjectField(label, character.healthBar, typeof(HealthBarScript), true);
            label = new GUIContent("Behavior Tree", "");
            character.behaviorTree = (BehaviorTree)EditorGUILayout.ObjectField(label, character.behaviorTree, typeof(BehaviorTree), true);
            label = new GUIContent("Stop Behavior Tree", "Stops the current active behavior tree from executing.");
            character.stopBehaviorTree = EditorGUILayout.Toggle(label, character.stopBehaviorTree);
            label = new GUIContent("Is Player Controlled", "Whether or not this AI is controlled by the player.");
            character.isPlayerControlled = EditorGUILayout.Toggle(label, character.isPlayerControlled);
            label = new GUIContent("Camera", "The Main Camera GameObject.");
            character.cam = (Camera)EditorGUILayout.ObjectField(label, character.cam, typeof(Camera), true);
            label = new GUIContent("Rig", "The rig that contains all the bones of the character. This is a prerequisite for enabling ragdoll.");
            character.rig = (GameObject)EditorGUILayout.ObjectField(label, character.rig, typeof(GameObject), true);
            label = new GUIContent("Enable ragdoll", "Enable ragdoll from the start.");
            character.enableRagdoll = EditorGUILayout.Toggle(label, character.enableRagdoll);
            label = new GUIContent("Enable Ragdoll on Death", "Enable ragdoll when the character dies.");
            character.enableRagdollOnDeath = EditorGUILayout.Toggle(label, character.enableRagdollOnDeath);
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
                allyTagCount = character.allyTags.Count;
                allyTagCount = EditorGUILayout.IntField("Size: ", allyTagCount);

                if (allyTagCount != character.allyTags.Count)
                {
                    int i = 0;
                    while (allyTagCount > character.allyTags.Count)
                    {
                        character.allyTags.Add("Ally" + i);
                        i++;
                    }
                    while (allyTagCount < character.allyTags.Count)
                    {
                        character.allyTags.RemoveAt(character.allyTags.Count - 1);
                    }
                }

                for (int i = 0; i < character.allyTags.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Element " + i);
                    string tag = "";
                    tag = EditorGUILayout.TagField(character.allyTags[i]);

                    if (character.enemyTags.Contains(tag))
                    {
                        EditorUtility.DisplayDialog("AI Controller", "Tag '" + tag + "' already added to enemy tags", "OK");
                    }
                    else
                    {
                        character.allyTags[i] = tag;
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
                enemyTagCount = character.enemyTags.Count;
                enemyTagCount = EditorGUILayout.IntField("Size", enemyTagCount);

                if (enemyTagCount != character.enemyTags.Count)
                {
                    int i = 0;
                    while (enemyTagCount > character.enemyTags.Count)
                    {
                        character.enemyTags.Add("Enemy" + i);
                        i++;
                    }
                    while (enemyTagCount < character.enemyTags.Count)
                    {
                        character.enemyTags.RemoveAt(character.enemyTags.Count - 1);
                    }
                }

                for (int i = 0; i < character.enemyTags.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Element " + i);
                    string tag = "";
                    tag = EditorGUILayout.TagField(character.enemyTags[i]);

                    if (character.allyTags.Contains(tag))
                    {
                        EditorUtility.DisplayDialog("AI Controller", "Tag '" + tag + "' already added to ally tags", "OK");
                    }
                    else
                    {
                        character.enemyTags[i] = tag;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
        }
        void DrawStatsGUI()
        {
            GUIContent label = new GUIContent("Overview Stats", "");
            GUIContent label2 = new GUIContent("", "");
            GUILayout.Label(label, guiStyle);

            label = new GUIContent("Base Hit Points", "The base hit points of the character after all modifiers are added.");
            character.statHitPoints.baseValue = EditorGUILayout.FloatField(label, character.statHitPoints.baseValue);

            label = new GUIContent("Max Hit Points", "The maximum hit points of the character after all modifiers are added.");
            label2 = new GUIContent(character.statHitPoints.value.ToString());
            EditorGUILayout.LabelField(label, label2);
            label = new GUIContent("Current Hit Points", "The current hit points of the character.");
            label2 = new GUIContent(character.currentHP.ToString());

            EditorGUILayout.LabelField(label, label2);
            EditorGUILayout.Space();
            label = new GUIContent("Base Attack Damage", "");
            character.statAttackDamage.baseValue = EditorGUILayout.FloatField(label, character.statAttackDamage.baseValue);

            label = new GUIContent("Base Movement Speed", "");
            character.statMoveSpeed.baseValue = EditorGUILayout.FloatField(label, character.statMoveSpeed.baseValue);
            EditorGUILayout.Space();

            //label = new GUIContent("Stat Multiplier", "This is used to calculate the attack damage based on your Strength, Intelligence and Faith. This variable can be changed during runtime.");
            //character.m_StatMultiplier = EditorGUILayout.Slider(label, character.m_StatMultiplier, 1f, 10);
            GUILayout.Label("Attributes", guiStyle);
            label = new GUIContent(character.statStrength.name);
            character.statStrength.Draw();
            character.statIntelligence.Draw();
            character.statFaith.Draw();
            character.statHitPoints.Draw();
            character.statAttackDamage.Draw();
            character.statAttackRange.Draw();
            character.statMoveSpeed.Draw();
        }

        
        #endregion

        void DrawMovementGUI()
        {

            //Detection and Head Look Settings
            GUILayout.Label("General Movement Settings", guiStyle);
            GUIContent label;
            EditorGUILayout.ObjectField("Move Target", character.moveTarget, typeof(GameObject), true);
            EditorGUILayout.ObjectField("Main Target", character.mainTarget, typeof(GameObject), true);
            label = new GUIContent("Base Movement Speed", "The base speed at which the agent will move at.");
            character.statMoveSpeed.baseValue = EditorGUILayout.FloatField(label, character.statMoveSpeed.baseValue);
            GUILayout.Space(16);
            label = new GUIContent("Movement Type", "Choose the movement type that this AI agent will use.");
            character.moveType = (MovementType)EditorGUILayout.EnumPopup(label, character.moveType);
            label = new GUIContent("Pathfind Source", "Choose the pathfind source that this AI agent will use.");
            character.pathfindSource = (PathfindSource)EditorGUILayout.EnumPopup(label, character.pathfindSource);
            if (character.pathfindSource == PathfindSource.Candice)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical("box");
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Candice Pathfind Settings", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                label = new GUIContent("Turn Speed", "The speed the agent will turn between waypoints by when pathfinding.");
                character.turnSpeed = EditorGUILayout.FloatField(label, character.turnSpeed);
                label = new GUIContent("Turn Distance", "The ditance the agent will start to turn while moving to the next node.");
                character.turnDist = EditorGUILayout.FloatField(label, character.turnDist);
                label = new GUIContent("Stopping Distance", "How far away the agent will start to come to a halt.");
                character.stoppingDist = EditorGUILayout.FloatField(label, character.stoppingDist);
                label = new GUIContent("Minimum Path Update Time", "Minimum time it will take for the agent before attempting to request a new updated path from Candice.");
                character.minPathUpdateTime = EditorGUILayout.FloatField(label, character.minPathUpdateTime);
                label = new GUIContent("Path Update Move Threshold", "Minimum distance the target can move by before requesting a new Updated path from Candice.");
                character.pathUpdateMoveThreshold = EditorGUILayout.FloatField(label, character.pathUpdateMoveThreshold);
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(16);
            GUILayout.Label("Head Look Settings", guiStyle);
            label = new GUIContent("Enable Head Look:", "Allow the agent to dynamically look at objects.");
            character.enableHeadLook = EditorGUILayout.Toggle(label, character.enableHeadLook);
            label = new GUIContent("Head Look target: ");
            character.headLookTarget = (GameObject)EditorGUILayout.ObjectField(label, character.headLookTarget, typeof(GameObject), true);
            label = new GUIContent("Head Look Intensity:", "How quickly the agent will turn their head to look at objects.");
            character.headLookIntensity = EditorGUILayout.Slider(label, character.headLookIntensity, 0f, 1f);

            //Patrol Settings
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            //GUIContent label = new GUIContent("Patrol Settings");

            //EditorGUILayout.LabelField();
            EditorGUILayout.LabelField("Patrol/Waypoint Settings", guiStyle);
            label = new GUIContent("Patrol Type", "");
            character.patrolType = (PatrolType)EditorGUILayout.EnumPopup(label, character.patrolType);
            character.isPatrolling = EditorGUILayout.Toggle("Is Patrolling", character.isPatrolling);
            label = new GUIContent("Patrol In Order:", "Whether or not the character should patrol each point in order of the list. False will allow the character to patrol randomly.");
            if(character.patrolType == PatrolType.PatrolPoints)
            {
                character.patrolInOrder = EditorGUILayout.Toggle(label, character.patrolInOrder);
                label = new GUIContent("Patrol Points:", "The points in the gameworld where you want the character to patrol. They can be anything, even empty gameObjects. Note: Ensure each patrol point is tagged as 'PatrolPoint'");

                patrolPointCount = character.patrolPoints.Count;
                showPatrolPoints = EditorGUILayout.Foldout(showPatrolPoints, label);
                if (showPatrolPoints)
                {
                    label = new GUIContent("Size:");
                    patrolPointCount = EditorGUILayout.IntField(label, patrolPointCount);

                    if (patrolPointCount != character.patrolPoints.Count)
                    {
                        while (patrolPointCount > character.patrolPoints.Count)
                        {
                            character.patrolPoints.Add(null);
                        }
                        while (patrolPointCount < character.patrolPoints.Count)
                        {
                            character.patrolPoints.RemoveAt(character.patrolPoints.Count - 1);
                        }
                    }
                    //EditorGUILayout.Space();
                    for (int i = 0; i < character.patrolPoints.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Element " + i);
                        character.patrolPoints[i] = (GameObject)EditorGUILayout.ObjectField(character.patrolPoints[i], typeof(GameObject), true);
                        EditorGUILayout.EndHorizontal();
                        //EditorGUILayout.Space();
                    }
                }
            }
            else if(character.patrolType == PatrolType.Waypoints)
            {
                label = new GUIContent("Waypoint:", "The first Waypoint that the agent will follow.");

                character.waypoint = (Waypoint)EditorGUILayout.ObjectField(label, character.waypoint, typeof(Waypoint), true);
            }
            
            /*
            */
        }
        void DrawDetectionGUI()
        {
            GUIContent label;
            GUILayout.Label("Detection Settings", guiStyle);

            label = new GUIContent("Sensor Type", "The sensor type this agent will use. Note: Sphere sensor also works with 2D.");
            character.sensorType = (SensorType) EditorGUILayout.EnumPopup(label, character.sensorType);

            label = new GUIContent("Perception Mask", "Layers that the agent must ignore");
            LayerMask tempMask = EditorGUILayout.MaskField(label, InternalEditorUtility.LayerMaskToConcatenatedLayersMask(character.perceptionMask), InternalEditorUtility.layers);
            character.perceptionMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
            label = new GUIContent("Detection Radius", "The radius which the character can detect other objects.");
            character.m_DetectionRadius = EditorGUILayout.FloatField(label, character.m_DetectionRadius);
            label = new GUIContent("Detection Height", "The height at which the agent can detect objects.");
            character.m_DetectionHeight = EditorGUILayout.FloatField(label, character.m_DetectionHeight);
            label = new GUIContent("Line of Sight", "The area where the agent will be able to see objects.");
            character.m_LineOfSight = EditorGUILayout.FloatField(label, character.m_LineOfSight);

            GUILayout.Label("Obstacle Avoidance Settings", guiStyle);
            label = new GUIContent("OA Distance", "The maximum distance that the agent will start to avoid detected objects.");
            character.oaDistance = EditorGUILayout.FloatField(label, character.oaDistance);
            label = new GUIContent("OA AOE", "the Area Of Effect around the agent that must not touch obstacles");
            character.oaAOE = EditorGUILayout.FloatField(label, character.oaAOE);
        }
        void DrawKeyMapGUI()
        {
            GUIContent label = new GUIContent("Click to Move", "If selected, click the area to move the agent. If disabled, normal keys will be used (e.g WASD and arrow keys)");
            character.clickToMove = EditorGUILayout.Toggle(label, character.clickToMove);      
            character.keyAttack = (KeyCode)EditorGUILayout.EnumFlagsField("Attack 1", character.keyAttack);
            character.keyAttack2 = (KeyCode)EditorGUILayout.EnumFlagsField("Attack 2", character.keyAttack2);
            character.special1 = (KeyCode) EditorGUILayout.EnumFlagsField("Special 1", character.special1);
            character.special2 = (KeyCode) EditorGUILayout.EnumFlagsField("Special 2", character.special2);
            character.special3 = (KeyCode) EditorGUILayout.EnumFlagsField("Special 3", character.special3);
        }
        
        
        
        void DrawCombatGUI()
        {
            GUILayout.Label("Attack Settings", guiStyle);
            GUIContent label = new GUIContent("Attack Type", "");
            character.attackType = EditorGUILayout.Popup(label, character.attackType, arrAttackTypes);
            label = new GUIContent("Attack Range", "The range that the AI will start attacking enemies.");
            character.statAttackRange.baseValue = EditorGUILayout.FloatField(label, character.statAttackRange.baseValue);
            label = new GUIContent("Attack Projectile", "The projectile that the agent will fire.");
            character.attackProjectile = (GameObject)EditorGUILayout.ObjectField(label, character.attackProjectile, typeof(GameObject), true);

            label = new GUIContent("Projectile Spawn Position", "The point where the projectile will spawn. e.g the hand for a spell, or the bow for an arrow.");
            character.spawnPosition = (Transform)EditorGUILayout.ObjectField(label, character.spawnPosition, typeof(Transform), true);

            //character.m_DamageAngle = EditorGUILayout.Slider("Damage Angle:", character.m_DamageAngle, 0, 360f);
            label = new GUIContent("Has Attack Animation", "Whether or not this agent has an attack animation.");
            character.hasAttackAnimation = EditorGUILayout.Toggle(label, character.hasAttackAnimation);
            label = new GUIContent("Attack Speed", "How many attacks per second the agent will deal");
            character.attacksPerSecond = EditorGUILayout.FloatField(label, character.attacksPerSecond);
            label = new GUIContent("Auto Attack", "");
            character.autoAttack = EditorGUILayout.Toggle(label, character.autoAttack);
            
            


            }
        private void DrawAnimationGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUIContent label = new GUIContent("Under Construction");
            guiStyle.normal.textColor = EditorStyles.label.normal.textColor;
            GUILayout.Label(label, guiStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
            
        void OnSceneGUI()
        {
            if(character != null)
            {
                //Call the necessary methods to draw the discs and handles on the editor
                if(character.is3D)
                {
                    Color color = Color.cyan;
                    color.a = 0.25f;
                    DrawDiscs(color, character.transform.position, Vector3.up, -character.transform.forward, ref character.m_DetectionRadius, "Detection Radius", float.MaxValue);
                    color = Color.blue;
                    color.a = 0.25f;
                    DrawArcs(color, character.transform.position, Vector3.up, character.transform.forward, character.transform.forward, ref character.m_LineOfSight, ref character.m_DetectionRadius, "Line of Sight");
                    color = Color.magenta;
                    color.a = 0.15f;
                    DrawDiscs(color, character.transform.position, Vector3.up, -character.transform.right, ref character.statAttackRange.baseValue, "Attack Range", character.m_DetectionRadius);
                    color = new Color(1f, 0f, 0f, 0.75f);//Red
                    DrawArcs(color, character.transform.position, Vector3.up, character.transform.forward, character.transform.right, ref character.statDamageAngle.baseValue, ref character.statAttackRange.baseValue, "Damage Angle");
                    
                }
                else
                {
                    Color color = new Color(1f, 0f, 0f, 0.15f);//Red
                    DrawDiscs(color, character.transform.position, Vector3.forward, character.transform.up, ref character.m_DetectionRadius, "Detection Radius", float.MaxValue);
                    color = new Color(0f, 0f, 1f, 0.35f);//Blue
                    DrawDiscs(color, character.transform.position, Vector3.forward, character.transform.right, ref character.statAttackRange.baseValue, "Attack Range", character.m_DetectionRadius);
                    //color = new Color(1f, 0f, 0f, 0.75f);//Red
                    //DrawArcs(color, character.transform.position, Vector3.forward, character.transform.up, character.transform.up, ref character.m_DamageAngle, ref character.m_AttackRange, "Damage Angle");
                    //color = new Color(0f, 1f, 0f, 0.35f);//Green
                    //DrawArcs(color, character.transform.position, Vector3.forward, character.transform.up, character.transform.forward, ref character.m_LineOfSight, ref character.m_DetectionRadius, "Line of Sight");
                }
                
            }
            
        }

        protected void DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius, float maxValue)
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
            Handles.DrawSolidDisc(center, normal, radius);
            Handles.color = new Color(1f, 1f, 0f, 0.75f);
            Handles.DrawWireDisc(center, normal, radius);

            //Create Slider handles to adjust detection radius properties
            color.a = 0.5f;
            Handles.color = color;
            radius = Handles.ScaleSlider(radius, character.transform.position, direction, Quaternion.identity, radius, 1f);
            radius = Mathf.Clamp(radius, 1f, maxValue);

            

        }
        
        protected void DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius, string label, float maxValue)
        {
            //
            //Method Name : void DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius, string label)
            //Purpose     : Overloaded method of DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius)
            //              that adds the necessary labels. 
            //Re-use      : DrawDiscs(Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius)
            //Input       : Color color, Vector3 center, Vector3 normal, Vector3 direction, ref float radius, string label
            //Output      : none
            //

            DrawDiscs(color, center, normal, direction, ref radius, maxValue);
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 12;
            labelStyle.normal.textColor = new Color(color.r, color.g, color.b, 1);
            labelStyle.alignment = TextAnchor.UpperCenter;
            Handles.Label(character.transform.position + (direction * radius), label, labelStyle);
        }

        protected void DrawArcs(Color color, Vector3 center, Vector3 normal, Vector3 direction, Vector3 sliderDirection, ref float angle, ref float radius, string label)
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
            Handles.DrawSolidArc(center, normal, direction, angle/2, radius);
            Handles.DrawSolidArc(center, normal, direction, -angle/2, radius);
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
        }
        #endregion
    }//end class
}//end namespace

