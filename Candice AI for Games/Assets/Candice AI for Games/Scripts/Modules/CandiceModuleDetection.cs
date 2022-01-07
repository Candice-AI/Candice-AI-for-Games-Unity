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
            else if (type == SensorType.Box)
            {
                hitColliders = Physics.OverlapBox(center, halfExtents);
            }

            Dictionary<string, List<GameObject>> detectedObjects = new Dictionary<string, List<GameObject>>();
            //Loop though each object
            foreach (Collider collider in hitColliders)
            {
                GameObject go = collider.gameObject;
                float distance = Vector3.Distance(center, go.transform.position);
                float angle = Vector3.Angle(go.transform.position - center, transform.forward);
                //Check if the object is in the enemy tag list
                CompareTags(go, request, ref detectedObjects);

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
            else if (type == SensorType.Box)
            {
                hitColliders = Physics2D.OverlapBoxAll(center, new Vector2(radius, radius), 360f);
            }

            Dictionary<string, List<GameObject>> detectedObjects = new Dictionary<string, List<GameObject>>();
            //Loop through each object
            foreach (Collider2D collider in hitColliders)
            {
                GameObject go = collider.gameObject;
                float distance = Vector3.Distance(center, go.transform.position);
                //float angle = Vector3.Angle(go.transform.position - aiController.transform.position, aiController.transform.forward);
                //Check if the object is in the enemy tag list
                CompareTags(go, request,ref detectedObjects);

            }
            objectDetectedCallback(new CandiceDetectionResults(detectedObjects));
        }
        public void CompareTags(GameObject go,CandiceDetectionRequest request,ref Dictionary<string, List<GameObject>> detectedObjects)
        {
            objects = new List<GameObject>();
            List<string> detectionTags = request.detectionTags;

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

