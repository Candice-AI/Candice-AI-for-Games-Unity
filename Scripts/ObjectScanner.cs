using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public enum SensorType
    {
        Sphere,
        Box,
    }
    public class ObjectScanner
    {
        Transform transform;
        public Action<DetectionResults> enemyDetectedCallback;
        List<GameObject> enemies;
        List<GameObject> allies;
        List<GameObject> players;
        public ObjectScanner(Transform transform, Action<DetectionResults> _enemyDetectedCallback)
        {
            this.transform = transform;
            enemyDetectedCallback = _enemyDetectedCallback;
            string[] className = (this.ToString()).Split('.');
            Debug.Log(className[className.Length - 1] + ": Initialised.");
        }
        public void ScanForObjects(DetectionRequest request)
        {
            Vector3 center = transform.position;
            enemies = new List<GameObject>();
            allies = new List<GameObject>();
            players = new List<GameObject>();

            bool is3D = request.is3D;
            float radius = request.radius;
            float height = request.height;
            float lineOfSight = request.lineOfSight;
            SensorType type = request.type;
            List<string> enemyTags = request.enemyTags;
            List<string> allyTags = request.allyTags;
            if (is3D)
            {
                //Array that will store all collided objects
                //Collider[] hitColliders = Physics.OverlapSphere(center, radius);

                Vector3 halfExtents = new Vector3(radius, height, radius);
                Collider[] hitColliders = null;
                if (type == SensorType.Sphere)
                {
                    hitColliders = Physics.OverlapSphere(center, radius);
                }
                else if(type == SensorType.Box)
                {
                    hitColliders = Physics.OverlapBox(center, halfExtents);
                }
                

                //Loop though each object
                foreach (Collider collider in hitColliders)
                {
                    GameObject go = collider.gameObject;
                    float distance = Vector3.Distance(center, go.transform.position);
                    float angle = Vector3.Angle(go.transform.position - center, transform.forward);
                    //Check if the object is in the enemy tag list
                    if (enemyTags.Contains(go.tag) && angle <= lineOfSight / 2)
                    {
                        enemies.Add(go);
                    }
                    else if (allyTags.Contains(go.tag) && angle <= lineOfSight / 2)
                    {
                        allies.Add(go);
                    }
                    if (go.tag.Equals("Player"))
                    {
                        players.Add(go);
                    }

                }
            }
            else
            {
                //Array that will store all collided objects
                Collider2D[] hitColliders = null;
                Vector2 center2D = new Vector2(center.x, center.y);
                if (type == SensorType.Sphere)
                {
                    hitColliders = Physics2D.OverlapCircleAll(center, radius);
                }
                else if (type == SensorType.Box)
                {
                    hitColliders =  Physics2D.OverlapBoxAll(center, new Vector2(radius, radius),360f);
                }
                

                //Loop though each object
                foreach (Collider2D collider in hitColliders)
                {
                    GameObject go = collider.gameObject;
                    float distance = Vector3.Distance(center, go.transform.position);
                    //float angle = Vector3.Angle(go.transform.position - aiController.transform.position, aiController.transform.forward);
                    //Check if the object is in the enemy tag list
                    if (enemyTags.Contains(go.tag))
                    {
                        enemies.Add(go);
                    }
                    if (allyTags.Contains(go.tag))
                    {
                        allies.Add(go);
                    }
                    if (go.tag.Equals("Player"))
                    {
                        players.Add(go);
                    }

                }
            }
            enemyDetectedCallback(new DetectionResults(players, enemies, allies));

        }
    }
    public struct DetectionRequest
    {
        public SensorType type;
        public List<string> enemyTags;
        public List<string> allyTags;
        public float radius;
        public float height;
        public float lineOfSight;
        public bool is3D;

        public DetectionRequest(SensorType type, List<string> enemyTags, List<string> allyTags, float radius, float height, float lineOfSight, bool is3D)
        {
            this.type = type;
            this.enemyTags = enemyTags;
            this.allyTags = allyTags;
            this.radius = radius;
            this.height = height;
            this.lineOfSight = lineOfSight;
            this.is3D = is3D;
        }
    }
    public struct DetectionResults
    {
        public List<GameObject> player;
        public List<GameObject> enemies;
        public List<GameObject> allies;

        public DetectionResults(List<GameObject> player, List<GameObject> enemies, List<GameObject> allies)
        {
            this.player = player;
            this.enemies = enemies;
            this.allies = allies;
        }
    }
}

