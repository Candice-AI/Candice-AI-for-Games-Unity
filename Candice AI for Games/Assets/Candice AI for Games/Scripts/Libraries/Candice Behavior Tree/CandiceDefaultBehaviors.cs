using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace CandiceAIforGames.AI
{
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
                return CandiceBehaviorStates.FAILURE;
            }

        }
        public static CandiceBehaviorStates MoveForward(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;
            agent.IsMoving = true;
            
            /*if (rootNode.aiController.hasAnimations)
            {
                if (rootNode.aiController.animationType == AnimationType.CodeBased)
                {
                    if (!agent.currentAnimation.Equals(agent.moveAnimationName))
                    {
                        agent.currentAnimation = agent.moveAnimationName;
                        agent.Animator.Play(agent.moveAnimationName);
                    }
                }
                else
                    agent.Animator.SetBool(agent.moveTransitionParameter, true);
            }*/

            //rootNode.aiController.Animator.SetFloat("characterSpeed", rootNode.aiController.movementSpeed);
            CandiceBehaviorStates state = CandiceBehaviorStates.SUCCESS;
            try
            {
                agent.movementModule.MoveForward(agent.transform, agent);
                
            }
            catch (Exception e)
            {
                state = CandiceBehaviorStates.FAILURE;
                Debug.LogError("DefaultBehaviors.MoveTo: " + e.Message);
            }

            return state;
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
            catch(Exception ex)
            {
                return CandiceBehaviorStates.FAILURE;
            }
                
        }

        public static CandiceBehaviorStates WithinAttackRange(CandiceBehaviorNode rootNode)
        {
            float distance = float.MaxValue;
            CandiceAIController agent = rootNode.aiController;
            try
            {
                distance = Vector3.Distance(agent.transform.position, agent.AttackTarget.transform.position);
            }
            catch (Exception e)
            {
                Debug.LogError("DefaultBehaviors.WithinAttackRange: No target within attack range: " + e.Message);
            }
            if (distance <= agent.AttackRange)
                return CandiceBehaviorStates.SUCCESS;
            else
                return CandiceBehaviorStates.FAILURE;
        }
        public static CandiceBehaviorStates Attack(CandiceBehaviorNode rootNode)
        {
            CandiceAIController agent = rootNode.aiController;
            agent.Attack();
            return CandiceBehaviorStates.SUCCESS;
        }

    }

}
