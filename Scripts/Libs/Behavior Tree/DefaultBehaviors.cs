using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class DefaultBehaviors
    {
        #region CHECK/VERIFY ACTION TASKS
        public static BehaviorStates EnemyDetected(BehaviorNode rootNode)
        {

            AIController agent = rootNode.aiController;
            if (agent.enemyFound)
            {
                //Set the main target as the enemy
                if (agent.enemies.Count > 0)
                {
                    rootNode.aiController.mainTarget = agent.enemies[0];
                }
                return BehaviorStates.SUCCESS;
            }
            else
                return BehaviorStates.FAILURE;
        }

       

        public static BehaviorStates AllyDetected(BehaviorNode rootNode)
        {
            AIController agent = rootNode.aiController;
            if (agent.allyFound)
            {
                //Set the main target as the enemy
                if (agent.allies.Count > 0)
                {
                    rootNode.aiController.mainTarget = agent.allies[0];
                }
                return BehaviorStates.SUCCESS;
            }
            else
                return BehaviorStates.FAILURE;
        }
        public static BehaviorStates PlayerDetected(BehaviorNode rootNode)
        {
            AIController agent = rootNode.aiController;
            if (agent.playerFound)
            {
                rootNode.aiController.mainTarget = agent.player;
                return BehaviorStates.SUCCESS;
            }

            else
                return BehaviorStates.FAILURE;
        }
        public static BehaviorStates WithinAttackRange(BehaviorNode rootNode)
        {
            float distance = float.MaxValue;
            AIController agent = rootNode.aiController;
            try
            {
                distance = Vector3.Distance(agent.transform.position, agent.attackTarget.transform.position);
            }
            catch (Exception e)
            {
                Debug.LogError("No target within attack range: " + e.Message);
            }
            if (distance <= agent.statAttackRange.value)
                return BehaviorStates.SUCCESS;
            else
                return BehaviorStates.FAILURE;
        }
        public static BehaviorStates IsPatrolling(BehaviorNode rootNode)
        {
            AIController agent = rootNode.aiController;
            if (agent.isPatrolling)
                return BehaviorStates.SUCCESS;
            else
                return BehaviorStates.FAILURE;
        }

        public static BehaviorStates IsDead(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.FAILURE;
            AIController agent = rootNode.aiController;
            try
            {
                if (agent.isDead)
                    state = BehaviorStates.SUCCESS;
            }
            catch (Exception e)
            {
                state = BehaviorStates.FAILURE;
                if (CandiceConfig.enableDebug)
                    Debug.LogError("Error: " + e.Message);
            }
            return state;
        }
        public static BehaviorStates SetMoveTarget(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.FAILURE;
            AIController agent = rootNode.aiController;
            try
            {
                if (agent.mainTarget != null)
                {
                    agent.moveTarget = agent.mainTarget;
                    agent.movePoint = agent.moveTarget.transform.position;
                    state = BehaviorStates.SUCCESS;
                }
                else
                {
                    if(CandiceConfig.enableDebug)
                    {
                        Debug.LogError("SetMoveTarget(): Main Target is NULL");
                    }
                }
            }
            catch (Exception e)
            {
                state = BehaviorStates.FAILURE;
                if (CandiceConfig.enableDebug)
                    Debug.LogError("Error: " + e.Message);
            }
            return state;
        }
        public static BehaviorStates SetAttackTarget(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.FAILURE;
            AIController agent = rootNode.aiController;
            try
            {
                if (agent.mainTarget != null)
                {
                    agent.attackTarget = agent.mainTarget;
                    state = BehaviorStates.SUCCESS;
                }
                else
                {
                    if (CandiceConfig.enableDebug)
                    {
                        Debug.LogError("SetAttackTarget(): Main Target is NULL");
                    }
                }
            }
            catch (Exception e)
            {
                state = BehaviorStates.FAILURE;
                if (CandiceConfig.enableDebug)
                    Debug.LogError("Error: " + e.Message);
            }
            return state;
        }
        #endregion

        #region ACTION TASKS
        public static BehaviorStates Idle(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.SUCCESS;
            if(rootNode.aiController.hasAnimations)
                rootNode.aiController.Animator.SetTrigger(rootNode.aiController.idleAnimParameter);
            return state;
        }
        public static BehaviorStates InitVariables(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.SUCCESS;
            rootNode.aiController.isMoving = false;
            return state;
        }
        public static BehaviorStates LookAt(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.SUCCESS;
            AIController agent = rootNode.aiController;
            try
            {
                if (agent.moveType == MovementType.STATIC && agent.is3D && agent.pathfindSource != PathfindSource.Candice && agent.pathfindSource != PathfindSource.UnityNavmesh) 
                {
                    agent.transform.LookAt(agent.mainTarget.transform);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Fail");
                state = BehaviorStates.FAILURE;
            }
            return state;
        }
        public static BehaviorStates MoveTo(BehaviorNode rootNode)
        {
            AIController agent = rootNode.aiController;
            agent.isMoving = true;
            if(rootNode.aiController.hasAnimations)
                agent.Animator.SetTrigger(agent.moveAnimParameter);
            //rootNode.aiController.Animator.SetFloat("characterSpeed", rootNode.aiController.movementSpeed);
            BehaviorStates state = BehaviorStates.SUCCESS;
            try
            {
                if(agent.pathfindSource == PathfindSource.None)
                {
                    agent.MoveTo(agent.moveTarget.transform.position, agent.statMoveSpeed.value, agent.is3D);
                }
                else if(agent.pathfindSource == PathfindSource.Candice)
                {
                    agent.StartFinding();
                }
                else if (agent.pathfindSource == PathfindSource.UnityNavmesh)
                {
                    agent.StartMoveNavMesh(agent.moveTarget);
                }

            }
            catch (Exception e)
            {
                state = BehaviorStates.FAILURE;
                if (CandiceConfig.enableDebug)
                    Debug.LogError("Error: " + e.Message);
            }

            return state;
        }



        public static BehaviorStates Die(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.FAILURE;
            AIController agent = rootNode.aiController;
            try
            {
                agent.stopBehaviorTree = true;
                if (agent.enableRagdollOnDeath)
                    agent.EnableRagdoll();
                else
                {
                    if (rootNode.aiController.hasAnimations)
                        agent.Animator.SetTrigger("die");
                }
                    
                state = BehaviorStates.SUCCESS;
            }
            catch (Exception e)
            {
                state = BehaviorStates.FAILURE;
                if (CandiceConfig.enableDebug)
                    Debug.LogError("Error: " + e.Message);
            }
            return state;
        }


        public static BehaviorStates Attack(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.SUCCESS;
            AIController agent = rootNode.aiController;
            try
            {
                if (agent.hasAttackAnimation)
                {
                    agent.Animator.SetTrigger(agent.attackAnimParameter);
                }
                else
                {
                    if (agent.attackType == Character.ATTACK_TYPE_MELEE)
                        agent.Attack();
                    else
                        agent.AttackRange();
                }
            }
            catch (Exception e)
            {

                state = BehaviorStates.FAILURE;
            }


            //rootNode.aiController.Animator.SetBool();
            return state;
        }

        public static BehaviorStates Patrol(BehaviorNode rootNode)
        {
            BehaviorStates state = BehaviorStates.SUCCESS;
            try
            {
                rootNode.aiController.Patrol();
            }
            catch (Exception e)
            {
                state = BehaviorStates.FAILURE;
                if (CandiceConfig.enableDebug)
                    Debug.LogError("Error: " + e.Message);
            }
            return state;
            #endregion



        }
    }

}
