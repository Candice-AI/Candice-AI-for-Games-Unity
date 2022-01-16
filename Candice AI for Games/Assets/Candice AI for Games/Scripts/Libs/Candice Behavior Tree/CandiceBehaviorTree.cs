﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CandiceAIforGames.AI
{
    [CreateAssetMenu(fileName = "New Candice Behavior Tree", menuName = "Behavior Tree")]
    public class CandiceBehaviorTree : ScriptableObject
    {
        public CandiceBehaviorNode rootNode;
        public CandiceBehaviorTreeS behaviorTree;
        public List<CandiceBehaviorNodeS> nodes;
        List<MethodInfo> lstFunctions;

        public void Initialise()
        {
            List<Type> behaviorTypes = new List<Type>();
            lstFunctions = new List<MethodInfo>();
            MethodInfo[] arrMethodInfos;
            nodes = behaviorTree.GetNodes();
            behaviorTypes.Add(typeof(CandiceDefaultBehaviors));
            foreach (Type type in behaviorTypes)
            {
                arrMethodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                lstFunctions.AddRange(arrMethodInfos);
            }
        }


        public CandiceBehaviorNode CreateBehaviorTree(CandiceAIController agent)
        {
            Initialise();
            rootNode = null;
            CandiceBehaviorNodeS _rootNode = null;
            nodes = behaviorTree.GetNodes();

            _rootNode = nodes[0];
            Debug.LogError("Agent: " + agent.AgentID);

            switch (_rootNode.type)
            {
                case CandiceAIManager.NODE_TYPE_SELECTOR:
                    rootNode = new CandiceBehaviorSelector();
                    rootNode.id = _rootNode.id;
                    rootNode.Initialise(agent.transform, agent);
                    (rootNode as CandiceBehaviorSelector).SetNodes(GetChildren(behaviorTree, _rootNode));
                    break;
                case CandiceAIManager.NODE_TYPE_SEQUENCE:
                    rootNode = new CandiceBehaviorSequence();
                    rootNode.id = _rootNode.id;
                    rootNode.Initialise(agent.transform, agent);
                    (rootNode as CandiceBehaviorSequence).SetNodes(GetChildren(behaviorTree, _rootNode));
                    break;
            }


            return rootNode;
        }
        List<CandiceBehaviorNode> GetChildren(CandiceBehaviorTreeS bt, CandiceBehaviorNodeS node)
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
                Debug.LogError("ID: " + id);
                if (GetNodeWithID(id, bt.GetNodes()) != null)
                {
                    _node = GetNodeWithID(id, nodes);

                    switch (_node.type)
                    {
                        case CandiceAIManager.NODE_TYPE_SELECTOR:
                            newNode = new CandiceBehaviorSelector();
                            (newNode as CandiceBehaviorSelector).SetNodes(GetChildren(bt, _node));
                            break;
                        case CandiceAIManager.NODE_TYPE_SEQUENCE:
                            newNode = new CandiceBehaviorSequence();
                            (newNode as CandiceBehaviorSequence).SetNodes(GetChildren(bt, _node));
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
