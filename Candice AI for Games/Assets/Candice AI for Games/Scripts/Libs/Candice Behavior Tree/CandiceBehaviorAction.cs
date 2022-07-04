using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    public class CandiceBehaviorAction : CandiceBehaviorNode
    {
        /// <summary>
        /// Method signature for the action
        /// </summary>
        public delegate CandiceBehaviorStates ActionNodeDelegate(CandiceBehaviorNode rootNode);
        CandiceBehaviorNode rootNode;

        /// <summary>
        /// The delegate that is called to evaluate this node
        /// </summary>
        private ActionNodeDelegate m_action;

        /// <summary>
        /// Because this node contains no logic itself, 
        /// the logic must be passed in the form of  
        /// a delegate. As the signature states, the action 
        /// needs to return a CandiceBehaviorStates enum
        /// </summary>
        public CandiceBehaviorAction(ActionNodeDelegate action, CandiceBehaviorNode rootNode)
        {
            this.rootNode = rootNode;
            m_action = action;
        }

        /// <summary>
        /// Evaluates the node using the passed in delegate and  
        /// reports the resulting state as appropriate
        /// </summary>
        public override CandiceBehaviorStates Evaluate()
        {
            switch (m_action(rootNode))
            {
                case CandiceBehaviorStates.SUCCESS:
                    m_nodeState = CandiceBehaviorStates.SUCCESS;
                    return m_nodeState;
                case CandiceBehaviorStates.FAILURE:
                    m_nodeState = CandiceBehaviorStates.FAILURE;
                    return m_nodeState;
                case CandiceBehaviorStates.RUNNING:
                    m_nodeState = CandiceBehaviorStates.RUNNING;
                    return m_nodeState;
                default:
                    m_nodeState = CandiceBehaviorStates.FAILURE;
                    return m_nodeState;
            }
        }
    }
}

