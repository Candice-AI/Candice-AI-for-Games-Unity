using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CandiceAIforGames.AI
{
    public class CandiceProjectile : MonoBehaviour
    {
        public Rigidbody rb;
        public GameObject target;
        public float attackDamage = 10f;
        public float moveSpeed = 200f;
        public bool destroyOnCollision = true;
        public bool destroyAfterDelay = false;
        public float destroyDelay = 5f;
        public float collisionDelay = 2f;
        public bool isFired = false;
        public bool stopOnCollision = true;
        private float timeElapsed = 0;
        public bool followTarget = false;
        public bool useForce = false;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            timeElapsed += Time.deltaTime;
            if (destroyAfterDelay && (timeElapsed%60) > destroyDelay)
            {
                Destroy(gameObject);
            }

            if (isFired)
            {
                if(followTarget)
                {
                    transform.LookAt(new Vector3(target.transform.position.x, gameObject.transform.position.y, target.transform.position.z));
                }
                Move();
            }
        }
        public void Fire(GameObject attackTarget)
        {
            target = attackTarget;
            transform.LookAt(new Vector3(target.transform.position.x, gameObject.transform.position.y-1, target.transform.position.z));
            isFired = true;
        }

        private void Move()
        {
            if(useForce)
                rb.velocity = transform.forward * moveSpeed * Time.deltaTime;
            else
                transform.position += transform.forward * 10 * Time.deltaTime;
        }


        void OnTriggerEnter(Collider collider)
        {
            DealDamage(collider.gameObject);
            //Check if destroyOnCollision is enabled and check if collided object is the target. 
            if (destroyOnCollision && collider.gameObject == target.gameObject)
            {
                Debug.Log("Collided with: " + collider.gameObject.name);
                StartCoroutine(DestroyAfterCollisionDelay());


                if (stopOnCollision)
                {
                    isFired = false;
                    gameObject.transform.SetParent(collider.gameObject.transform);
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }

            }
        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject != gameObject && isFired)
            {
                //Debug.Log("Collided with: " + collision.gameObject.name);
                //Debug.Log("Fire True: " + fireTrue);
                DealDamage(collision.gameObject);
                if (stopOnCollision)
                {
                    isFired = false;
                    gameObject.transform.SetParent(collision.gameObject.transform);
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                if (destroyOnCollision)
                {
                    StartCoroutine(DestroyAfterCollisionDelay());

                }
            }
        }
        void DealDamage(GameObject go)
        {
            try
            {
                go.SendMessage("CandiceReceiveDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
            }
            catch //(Exception e)
            {
                //Debug.LogError(e.Message);
            }

        }
        IEnumerator DestroyAfterCollisionDelay()
        {
            yield return new WaitForSeconds(collisionDelay);
            Destroy(gameObject);

        }
    }
}