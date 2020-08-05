using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViridaxGameStudios.AI;
namespace ViridaxGameStudios
{
    public class MoveAction : FSMAction
    {
        private Transform transform;
        private Animator animator;
        private string finishEvent;
        //private float size;
        Vector3[] path;
        Vector3 currentWaypoint;
        int targetIndex;
        bool canMove = false;
        public MoveAction(FSMState owner, Character aiController) :base(owner, aiController)
        {

        }

        public void Init(Transform transform, Animator animator, string finishEvent = null)
        {
            this.transform = transform;
            this.animator = animator;
            this.finishEvent = finishEvent;
            //size = transform.localScale.x;
        }
        public override void OnEnter()
        {
            aiController.isMoving = true;
            animator.SetBool("isRunning", true);
            
            if(aiController.enablePathfinding)
            {
                switch (aiController.pathfindSource)
                {
                    case PathfindSource.UnityNavmesh:
                        Debug.Log("Starting NavMesh");
                        aiController.StartMoveNavMesh(aiController.mainTarget);
                        break;
                    case PathfindSource.Candice:
                        //aiController.StartFinding();

                        break;
                }
            }
            

        }

        public override void OnExit()
        {
            //base.OnExit();
            aiController.isMoving = false;
            animator.SetBool("isRunning", false);

            if (aiController.enablePathfinding)
            {
                switch (aiController.pathfindSource)
                {
                    case PathfindSource.UnityNavmesh:
                        aiController.StopMoveNavMesh();
                        break;
                    case PathfindSource.Candice:
                        //aiController.StopFinding();

                        break;
                }
            }
            

        }

        public override void OnUpdate()
        {
            
            

            if (aiController.moveTarget != null)
            {
                //Move(AIController.target.transform);
                switch (aiController.moveType)
                {
                    case MovementType.STATIC:
                        FollowTarget(aiController.moveTarget.transform);
                        break;
                    case MovementType.DYNAMIC:
                        break;
                }
            }
            
        }
        
        
        
        private void FollowTarget(Transform Target)
        {
            LookAt(Target.gameObject);
            //float distance = Vector3.Distance(transform.position, target.transform.position);
            transform.position += transform.forward * aiController.statMoveSpeed.value * Time.deltaTime;
            //Debug.DrawLine(transform.position, Target.position, Color.green);
        }

        protected void LookAt(GameObject Target)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12 * Time.deltaTime);
        }
        protected void LookAt(Vector3 Target)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Target - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12 * Time.deltaTime);
        }
    }


}
