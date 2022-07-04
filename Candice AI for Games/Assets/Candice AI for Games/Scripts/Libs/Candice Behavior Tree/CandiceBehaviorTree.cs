using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CandiceAIforGames.AI
{
    [Serializable]
    public class CandiceBehaviorTree
    {
        [SerializeField]
        public string _name;
        public CandiceBehaviorNode rootNode;
        [SerializeField]
        public List<CandiceBehaviorNodeS> nodes;
        
        [SerializeField]
        List<MethodInfo> lstFunctions;

        public void Initialise()
        {
            List<Type> behaviorTypes = new List<Type>();
            lstFunctions = new List<MethodInfo>();
            MethodInfo[] arrMethodInfos;
            behaviorTypes.Add(typeof(CandiceDefaultBehaviors));
            foreach (Type type in behaviorTypes)
            {
                arrMethodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                lstFunctions.AddRange(arrMethodInfos);
            }
        }

        public void Evaluate()
        {
            if(rootNode != null)
            {
                rootNode.Evaluate();
            }
        }
        public CandiceBehaviorNode CreateBehaviorTree(CandiceAIController agent)
        {
            //Initialise();
            rootNode = null;
            CandiceBehaviorNodeS _rootNode = null;
            //nodes = behaviorTree.GetNodes();
            if(nodes == null || nodes.Count == 0)
            {
                return null;
            }

            _rootNode = nodes[0];

            switch (_rootNode.type)
            {
                case CandiceAIManager.NODE_TYPE_SELECTOR:
                    rootNode = new CandiceBehaviorSelector();
                    rootNode.id = _rootNode.id;
                    rootNode.Initialise(agent.transform, agent);
                    (rootNode as CandiceBehaviorSelector).SetNodes(GetChildren(nodes, _rootNode));
                    break;
                case CandiceAIManager.NODE_TYPE_SEQUENCE:
                    rootNode = new CandiceBehaviorSequence();
                    rootNode.id = _rootNode.id;
                    rootNode.Initialise(agent.transform, agent);
                    (rootNode as CandiceBehaviorSequence).SetNodes(GetChildren(nodes, _rootNode));
                    break;
            }


            return rootNode;
        }
        List<CandiceBehaviorNode> GetChildren(List<CandiceBehaviorNodeS> nodes, CandiceBehaviorNodeS node)
        {
            List<CandiceBehaviorNode> children = new List<CandiceBehaviorNode>();
            CandiceBehaviorNodeS _node = null;
            if (node.childrenIDs.Count < 1)
            {
                return children;
            }
            foreach (int id in node.childrenIDs)
            {
                CandiceBehaviorNode newNode = null;
                if (GetNodeWithID(id, nodes) != null)
                {
                    _node = GetNodeWithID(id, nodes);

                    switch (_node.type)
                    {
                        case CandiceAIManager.NODE_TYPE_SELECTOR:
                            newNode = new CandiceBehaviorSelector();
                            (newNode as CandiceBehaviorSelector).SetNodes(GetChildren(nodes, _node));
                            break;
                        case CandiceAIManager.NODE_TYPE_SEQUENCE:
                            newNode = new CandiceBehaviorSequence();
                            (newNode as CandiceBehaviorSequence).SetNodes(GetChildren(nodes, _node));
                            break;
                        case CandiceAIManager.NODE_TYPE_ACTION:
                            CandiceBehaviorAction action = new CandiceBehaviorAction((CandiceBehaviorAction.ActionNodeDelegate)lstFunctions[_node.function].CreateDelegate(typeof(CandiceBehaviorAction.ActionNodeDelegate)), rootNode);
                            newNode = action;
                            break;
                    }
                    children.Add(newNode);
                }
            }
            return children;
        }

        CandiceBehaviorNodeS GetNodeWithID(int id, List<CandiceBehaviorNodeS> nodes)
        {
            CandiceBehaviorNodeS node = null;
            bool isFound = false;
            int count = 0;
            while (!isFound && count < nodes.Count)
            {
                if (nodes[count].id == id)
                {
                    node = nodes[count];
                    isFound = true;
                }
                count++;
            }
            return node;
        }


    }
}
