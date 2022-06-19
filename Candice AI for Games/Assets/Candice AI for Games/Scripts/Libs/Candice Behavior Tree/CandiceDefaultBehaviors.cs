using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    /// <summary>
    /// Class that houses all methods/functions the Candice Behavior Tree will use in the Action nodes.
    /// </summary>
    /// <remarks>
    /// By default, all of these methods have a reference to the Candice AI Controller that houses the logic, and simply calls those methods in the controller.
    /// </remarks>
    /// 
    public class CandiceDefaultBehaviors
    {

        public static CandiceBehaviorStates ScanForObjects(CandiceBehaviorNode rootNode)
        {
            try
            {
                CandiceAIController agent = rootNode.aiController;
                agent.ScanForObjects();
                return CandiceBehaviorStates.SUCCESS;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return CandiceBehaviorStates.FAILURE;
            }

        }
        
        public static CandiceBehaviorStates ObjectDetected(CandiceBehaviorNode rootNode)
        {

            CandiceAIController agent = rootNode.aiController;
            if (agent.ObjectDetected)
            {
                return CandiceBehaviorStates.SUCCESS;
            }
            else
                return CandiceBehaviorStates.FAILURE;
        }
        public static CandiceBehaviorStates PlayerDetected(CandiceBehaviorNode rootNode)
        {

            CandiceAIController agent = rootNode.aiController;
            if (agent.PlayerDetected)
            {
                return CandiceBehaviorStates.SUCCESS;
            }
            else
                return CandiceBehaviorStates.FAILURE;
        }
        public static CandiceBehaviorStates EnemyDetected(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;
            if (agent.EnemyDetected)
            {
                return CandiceBehaviorStates.SUCCESS;
            }
            else
                return CandiceBehaviorStates.FAILURE;
        }
        public static CandiceBehaviorStates AllyDetected(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;
            if (agent.AllyDetected)
            {
                return CandiceBehaviorStates.SUCCESS;
            }
            else
                return CandiceBehaviorStates.FAILURE;
        }

        public static CandiceBehaviorStates LookAt(CandiceBehaviorNode rootNode)
        {
            try
            {
                CandiceAIController agent = rootNode.aiController;
                agent.movementModule.LookAt(agent.transform, agent);
                return CandiceBehaviorStates.SUCCESS;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return CandiceBehaviorStates.FAILURE;
            }

        }
        public static CandiceBehaviorStates RotateTo(CandiceBehaviorNode rootNode)
        {
            CandiceBehaviorStates state = CandiceBehaviorStates.SUCCESS;
            try
            {
                float desiredAngle = 180;
                int direction = 1;
                float angle = Vector3.Angle((rootNode.aiController.MainTarget.transform.position - rootNode.aiController.transform.position), rootNode.aiController.transform.right);
                if (angle > 90)
                    angle = 360 - angle;
                if (angle > desiredAngle)
                    direction = -1;

                float rotation = (direction * rootNode.aiController.RotationSpeed) * Time.deltaTime;
                rootNode.aiController.transform.Rotate(0, rotation, 0);
            }
            catch (Exception e)
            {
                state = CandiceBehaviorStates.FAILURE;
                Debug.LogError("CandiceDefaultBehaviors.RotateTo: " + e.Message);

            }

            return state;
        }
        public static CandiceBehaviorStates AvoidObstacles(CandiceBehaviorNode rootNode)
        {
            try
            {
                CandiceAIController agent = rootNode.aiController;
                agent.AvoidObstacles();
                return CandiceBehaviorStates.SUCCESS;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return CandiceBehaviorStates.FAILURE;
            }

        }
        public static CandiceBehaviorStates CandicePathfind(CandiceBehaviorNode rootNode)
        {
            try
            {
                CandiceAIController agent = rootNode.aiController;
                if(agent.CandicePathfind())
                    return CandiceBehaviorStates.SUCCESS;
                else
                    return CandiceBehaviorStates.FAILURE;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return CandiceBehaviorStates.FAILURE;
            }

        }
        

        public static CandiceBehaviorStates SetMovePoint(CandiceBehaviorNode rootNode)
        {
            CandiceBehaviorStates state = CandiceBehaviorStates.FAILURE;
            CandiceAIController agent = rootNode.aiController;
            try
            {
                if (agent.MainTarget != null)
                {
                    agent.MovePoint = agent.MainTarget.transform.position;
                    state = CandiceBehaviorStates.SUCCESS;
                }
                else
                {
                    Debug.LogError("CandiceDefaultBehaviors.SetMoveTarget: Main Target is NULL");
                }
            }
            catch (Exception e)
            {
                state = CandiceBehaviorStates.FAILURE;
                Debug.LogError("CandiceDefaultBehaviors.SetMovePoint: " + e.Message);

            }
            return state;
        }
        public static CandiceBehaviorStates MoveForward(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;
            agent.IsMoving = true;
            CandiceBehaviorStates state = CandiceBehaviorStates.SUCCESS;
            try
            {
                agent.movementModule.MoveForward(agent.transform, agent);
                
            }
            catch (Exception e)
            {
                state = CandiceBehaviorStates.FAILURE;
                Debug.LogError("DefaultBehaviors.MoveForward: " + e.Message);
            }

            return state;
        }
        public static CandiceBehaviorStates MoveForwardWithSlopeAlignment(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;
            agent.IsMoving = true;
            CandiceBehaviorStates state = CandiceBehaviorStates.SUCCESS;
            try
            {
                agent.movementModule.MoveForwardWithSlopeAlignment(agent.transform, agent);

            }
            catch (Exception e)
            {
                state = CandiceBehaviorStates.FAILURE;
                Debug.LogError("DefaultBehaviors.MoveForwardWithSlopeAlignment: " + e.Message);
            }

            return state;
        }
        public static CandiceBehaviorStates Flee(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;
            agent.Flee();
            return CandiceBehaviorStates.SUCCESS;
        }

        public static CandiceBehaviorStates Wander(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;
            agent.Wander();
            return CandiceBehaviorStates.SUCCESS;
        }
        public static CandiceBehaviorStates WaypointPatrol(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;
            agent.IsMoving = true;
            CandiceBehaviorStates state = CandiceBehaviorStates.SUCCESS;
            try
            {
                agent.WaypointPatrol();

            }
            catch (Exception e)
            {
                state = CandiceBehaviorStates.FAILURE;
                Debug.LogError("DefaultBehaviors.WaypointPatrol: " + e.Message);
            }

            return state;
        }
        
        public static CandiceBehaviorStates WithinAttackRange(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;

            if (agent.WithinAttackRange())
                return CandiceBehaviorStates.SUCCESS;
            else
                return CandiceBehaviorStates.FAILURE;
        }
        public static CandiceBehaviorStates SetAttackTarget(CandiceBehaviorNode rootNode)
        {
            CandiceBehaviorStates state = CandiceBehaviorStates.FAILURE;
            CandiceAIController agent = rootNode.aiController;
            try
            {
                if (agent.MainTarget != null)
                {
                    agent.AttackTarget = agent.MainTarget;
                    state = CandiceBehaviorStates.SUCCESS;
                }
                else
                {
                    Debug.LogError("CandiceDefaultBehaviors.SetAttackTarget: Main Target is NULL");
                }
            }
            catch (Exception e)
            {
                state = CandiceBehaviorStates.FAILURE;
                Debug.LogError("CandiceDefaultBehaviors.SetAttackTarget: " + e.Message);
            }
            return state;
        }
        public static CandiceBehaviorStates AttackMelee(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;
            agent.AttackMelee();
            return CandiceBehaviorStates.SUCCESS;
        }
        public static CandiceBehaviorStates AttackRange(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;
            agent.AttackRanged();
            return CandiceBehaviorStates.SUCCESS;
        }

        public static CandiceBehaviorStates ScanForObjects2D(CandiceBehaviorNode rootNode)
        {
            try
            {
                CandiceAIController agent = rootNode.aiController;
                agent.ScanForObjects2D();
                return CandiceBehaviorStates.SUCCESS;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return CandiceBehaviorStates.FAILURE;
            }

        }



    }

}
