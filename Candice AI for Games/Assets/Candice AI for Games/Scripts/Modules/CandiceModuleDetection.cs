using CandiceAIforGames.AI.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    public class CandiceModuleDetection:CandiceBaseModule
    {
        Transform transform;
        public Action<CandiceDetectionResults> objectDetectedCallback;
        List<GameObject> objects;

        public CandiceModuleDetection(Transform transform, Action<CandiceDetectionResults> _objectDetectedCallback, string moduleName = "CandiceModuleDetection") : base(moduleName)
        {
            this.transform = transform;
            objectDetectedCallback = _objectDetectedCallback;
            Utils.Utils.LogClassInitialisation(this);
        }

        public void ScanForObjects(CandiceDetectionRequest request)
        {
            Vector3 center = transform.position;

            bool is3D = request.is3D;
            float radius = request.radius;
            float height = request.height;
            float lineOfSight = request.lineOfSight;
            SensorType type = request.type;
            //Array that will store all collided objects
            //Collider[] hitColliders = Physics.OverlapSphere(center, radius);

            Vector3 halfExtents = new Vector3(radius, height, radius);
            Collider[] hitColliders = null;
            if (type == SensorType.Sphere)
            {
                hitColliders = Physics.OverlapSphere(center, radius);
            }

            Dictionary<string, List<GameObject>> detectedObjects = new Dictionary<string, List<GameObject>>();
            //Loop though each object
            foreach (Collider collider in hitColliders)
            {
                GameObject go = collider.gameObject;
                float distance = Vector3.Distance(center, go.transform.position);
                float angle = Vector3.Angle(go.transform.position - center, transform.forward);
                if (angle <= lineOfSight / 2)
                {
                    CompareTags(go, request.detectionTags, ref detectedObjects);
                }
                
                

            }
            objectDetectedCallback(new CandiceDetectionResults(detectedObjects));
        }
        public void ScanForObjects2D(CandiceDetectionRequest request)
        {
            Vector3 center = transform.position;

            bool is3D = request.is3D;
            float radius = request.radius;
            float height = request.height;
            float lineOfSight = request.lineOfSight;
            SensorType type = request.type;

            //Array that will store all collided objects
            Collider2D[] hitColliders = null;
            Vector2 center2D = new Vector2(center.x, center.y);
            if (type == SensorType.Sphere)
            {
                hitColliders = Physics2D.OverlapCircleAll(center, radius);
            }
            Dictionary<string, List<GameObject>> detectedObjects = new Dictionary<string, List<GameObject>>();
            //Loop through each object
            foreach (Collider2D collider in hitColliders)
            {
                GameObject go = collider.gameObject;
                float distance = Vector3.Distance(center, go.transform.position);
                float angle = Vector2.Angle((new Vector2(go.transform.position.x, go.transform.position.y) - center2D), transform.forward);
                //Check if the object is in the enemy tag list
                if (angle <= lineOfSight / 2)
                {
                    CompareTags(go, request.detectionTags, ref detectedObjects);
                }

            }
            objectDetectedCallback(new CandiceDetectionResults(detectedObjects));
        }
        public void AvoidObstacles(Transform Target, Vector3 movePoint, Transform transform, float size, float movementSpeed, bool is3D, float distance,int lines, LayerMask perceptionMask)
        {
            //
            //Method Name : void Move(Transform Target, Transform transform, float size)
            //Purpose     : This method moves the agent while avoiding immediate obstacles.
            //Re-use      : none
            //Input       : Transform Target, Transform transform, float size
            //Output      : void
            //
            if (!is3D)
            {
                AvoidObstacles2D(Target, transform, size, movementSpeed, distance);
                return;
            }
            bool obstacleHit = false;
            Vector3 dir = (transform.forward).normalized;
            RaycastHit hit;

            Vector3 center = transform.position;

            Vector3[] oaPoints = new Vector3[lines];
            float step = size / lines;
            float currentPos = transform.position.x - size;

            for(int i = 0; i < lines;i++)
            {
                oaPoints[i] = transform.position;

                oaPoints[i].x = currentPos;
                currentPos += step*2;
            }
            for (int i = 0; i < oaPoints.Length;i++)
            {
                Vector3 point = oaPoints[i];
                if (i >= oaPoints.Length / 2 && i <= oaPoints.Length / 2 + 1)
                {
                    distance = distance * 1.5f;
                }
                if (Physics.Raycast(point, transform.forward, out hit, distance))
                {
                    if (hit.transform != transform && hit.transform != Target.transform)
                    {
                        if (HasLayer(perceptionMask, hit.transform.gameObject.layer))
                        {
                            Debug.DrawLine(point, hit.point, Color.red);
                            dir += hit.normal * 90;

                            obstacleHit = true;
                        }
                        /*foreach (LayerMask region in walkableRegions)
                        {
                            int id = Convert.ToInt32(Mathf.Log(region.value, 2));

                            if(id == hit.transform.gameObject.layer)
                            {
                                Debug.DrawLine(point, hit.point, Color.red);
                                dir += hit.normal * 90;

                                obstacleHit = true;
                            }
                        }*/

                    }
                }
            }
            if (obstacleHit)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, movementSpeed * Time.deltaTime);
            }
            else
            {
                movePoint = new Vector3(movePoint.x, transform.position.y, movePoint.z);
                dir = (movePoint - transform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, movementSpeed * Time.deltaTime);
            }
            
        }

        public static bool HasLayer(LayerMask layerMask, int layer)
        {
            if (layerMask == (layerMask | (1 << layer)))
            {
                return true;
            }

            return false;
        }


        private bool compareObjToTags(GameObject obj, List<string> tags)
        {
            //Check if the object is in the tag list
            bool hasMatch = false;

            for (int i = 0; i < tags.Count; i++)
            {
                if(obj.tag.Equals(tags[i]))
                {
                    hasMatch = true;
                    i = tags.Count;
                }
            }

            return hasMatch;
        }

        public void AvoidObstacles2D(Transform Target, Transform transform, float size, float movementSpeed, float distance)
        {
            Vector2 dir = (Target.position - transform.position).normalized;
            RaycastHit2D hit;
            if ((hit = Physics2D.Raycast(transform.position, transform.forward, distance)))
            {
                Debug.Log("OA 2D");
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    dir += hit.normal * 50;
                }
            }

            Vector3 left = transform.position;
            Vector3 right = transform.position;

            left.x -= size;
            right.x += size;
            if ((hit = Physics2D.Raycast(left, transform.forward, distance)))
            {
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(left, hit.point, Color.red);
                    dir += hit.normal * 50;

                }
            }

            if ((hit = Physics2D.Raycast(right, transform.forward, distance)))
            {
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(right, hit.point, Color.red);
                    dir += hit.normal * 50;
                }
            }
            //Quaternion rot = Quaternion.LookRotation(dir);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
            //transform.position += new Vector3(dir.x, dir.y) * movementSpeed * Time.deltaTime;


        }
        public void CompareTags(GameObject go, List<string> detectionTags, ref Dictionary<string, List<GameObject>> detectedObjects)
        {
            objects = new List<GameObject>();

            if (detectionTags.Contains(go.tag))
            {
                if(detectedObjects.ContainsKey(go.tag))
                {
                    detectedObjects[go.tag].Add(go);
                }
                else
                {
                    List<GameObject> objects = new List<GameObject>();
                    objects.Add(go);
                    detectedObjects.Add(go.tag, objects);
                }
            }
        }

    }
    
    public struct CandiceDetectionRequest
    {
        public SensorType type;
        public List<string> detectionTags;
        public float radius;
        public float height;
        public float lineOfSight;
        public bool is3D;

        public CandiceDetectionRequest(SensorType type, List<string> detectionTags, float radius, float height, float lineOfSight, bool is3D)
        {
            this.type = type;
            this.detectionTags = detectionTags;
            this.radius = radius;
            this.height = height;
            this.lineOfSight = lineOfSight;
            this.is3D = is3D;
        }
    }
    public struct CandiceDetectionResults
    {
        public Dictionary<string,List<GameObject>> objects;

        public CandiceDetectionResults(Dictionary<string, List<GameObject>> objects)
        {
            this.objects = objects;
        }
    }
}

