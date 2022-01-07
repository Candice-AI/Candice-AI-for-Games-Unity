using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    public class CandiceBehaviorTreeMono : MonoBehaviour
    {
        CandiceAIController aiController;
        private CandiceBehaviorSequence rootNode;
        private CandiceBehaviorAction ScanForObjectsNode;
        private CandiceBehaviorAction canSeeEnemyNode;
        private CandiceBehaviorAction lookAtNode;

        private CandiceBehaviorSelector attackOrChaseSelector;
        private CandiceBehaviorSequence attackSequence;
        private CandiceBehaviorAction withinAttackRange;
        private CandiceBehaviorAction attackNode;

        private CandiceBehaviorAction moveNode;



        private CandiceBehaviorSequence followSequence;




















        private CandiceBehaviorSequence canSeeEnemySequence;
        private CandiceBehaviorSequence patrolSequence;
        private CandiceBehaviorSequence isDeadSequence;

        private CandiceBehaviorAction initNode;
        
        
        
        
        
        private CandiceBehaviorAction idleNode;
        private CandiceBehaviorAction isDeadNode;
        private CandiceBehaviorAction dieNode;
        

        private CandiceBehaviorAction setAttack;
        private CandiceBehaviorAction setMove;

        
        
        private CandiceBehaviorAction rotateToNode;
        private CandiceBehaviorAction isPatrollingNode;
        private CandiceBehaviorAction patrolNode;

        CandiceBehaviorSelector enemyDetctedSelector;
        CandiceBehaviorAction fleeNode;
        CandiceDefaultBehaviors paladinBehaviours;



        public void Initialise(CandiceAIController aiController)
        {
            this.aiController = aiController;
        }
        // Start is called before the first frame update
        void Start()
        {
            aiController = GetComponent<CandiceAIController>();
            rootNode = new CandiceBehaviorSequence();
            rootNode.Initialise(transform, aiController);

            ScanForObjectsNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.ScanForObjects, rootNode);
            canSeeEnemyNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.EnemyDetected, rootNode);
            lookAtNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.LookAt, rootNode);
            attackNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.Attack, rootNode);
            moveNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.MoveForward, rootNode);
            withinAttackRange = new CandiceBehaviorAction(CandiceDefaultBehaviors.WithinAttackRange, rootNode);


            attackSequence = new CandiceBehaviorSequence();
            attackSequence.SetNodes(new List<CandiceBehaviorNode> { withinAttackRange, lookAtNode, attackNode });
            followSequence = new CandiceBehaviorSequence();
            followSequence.SetNodes(new List<CandiceBehaviorNode> { lookAtNode, moveNode });
            attackOrChaseSelector = new CandiceBehaviorSelector();
            attackOrChaseSelector.SetNodes(new List<CandiceBehaviorNode> { attackSequence,followSequence });

            

            rootNode.SetNodes(new List<CandiceBehaviorNode>{ ScanForObjectsNode,canSeeEnemyNode,attackOrChaseSelector});
            /*lookAtNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.LookAt, rootNode);
            rotateToNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.RotateTo, rootNode);
            canSeeEnemyNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.EnemyDetected, rootNode);
            fleeNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.Wander, rootNode);


            enemyDetctedSelector = new CandiceBehaviorSelector();
            enemyDetctedSelector.SetNodes(new List<CandiceBehaviorNode> { canSeeEnemyNode });

            isPatrollingNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.IsPatrolling, rootNode);
            patrolNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.Patrol, rootNode);
            patrolSequence = new CandiceBehaviorSequence();
            patrolSequence.SetNodes(new List<CandiceBehaviorNode> { isPatrollingNode, patrolNode, rotateToNode, moveNode });

            idleNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.Idle, rootNode);

            rootNode.SetNodes(new List<CandiceBehaviorNode>{enemyDetctedSelector,fleeNode,lookAtNode,moveNode
            });*/

            /*
            initNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.InitVariables, rootNode);
            moveNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.MoveTo, rootNode);
            lookAtNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.LookAt, rootNode);
            rotateToNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.RotateTo, rootNode);
            canSeeEnemyNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.EnemyDetected, rootNode);
            withinAttackRange = new CandiceBehaviorAction(CandiceDefaultBehaviors.WithinAttackRange, rootNode);
            attackNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.Attack, rootNode);
            attackSequence = new CandiceBehaviorSequence();
            attackSequence.SetNodes(new List<CandiceBehaviorNode> { withinAttackRange, rotateToNode, attackNode });
            isDeadNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.IsDead, rootNode);
            dieNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.Die, rootNode);
            setAttack = new CandiceBehaviorAction(CandiceDefaultBehaviors.SetAttackTarget, rootNode);
            setMove = new CandiceBehaviorAction(CandiceDefaultBehaviors.SetMoveTarget, rootNode);

            attackOrChaseSelector = new CandiceBehaviorSelector();
            attackOrChaseSelector.SetNodes(new List<CandiceBehaviorNode> { attackSequence, new Inverter(lookAtNode), moveNode });
            canSeeEnemySequence = new CandiceBehaviorSequence();
            canSeeEnemySequence.SetNodes(new List<CandiceBehaviorNode> { canSeeEnemyNode, setMove, setAttack, attackOrChaseSelector });
            isDeadSequence = new CandiceBehaviorSequence();
            isDeadSequence.SetNodes(new List<CandiceBehaviorNode> { isDeadNode, dieNode });

            isPatrollingNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.IsPatrolling, rootNode);
            patrolNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.Patrol, rootNode);
            patrolSequence = new CandiceBehaviorSequence();
            patrolSequence.SetNodes(new List<CandiceBehaviorNode> { isPatrollingNode, patrolNode, rotateToNode, moveNode });

            idleNode = new CandiceBehaviorAction(CandiceDefaultBehaviors.Idle, rootNode);

            rootNode.SetNodes(new List<CandiceBehaviorNode>{
            new CandiceBehaviorInverter(initNode), isDeadSequence, canSeeEnemySequence, patrolSequence, idleNode
            });
            */
        }

        // Update is called once per frame
        void Update()
        {
            Evaluate();
        }

        public void Evaluate()
        {
            rootNode.Evaluate();
        }


    }

}
