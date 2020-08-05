using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class AttackAction : FSMAction
    {
        Animator animator;
        string finishEvent;
        public AttackAction(FSMState owner, Character aiController, string finishEvent = null) : base(owner, aiController)
        {
            animator = aiController.Animator;
            this.finishEvent = finishEvent;
        }
        public override void OnEnter()
        {
            animator.SetBool(aiController.attackAnimParameter, true);
        }

        public override void OnExit()
        {
            animator.SetBool(aiController.attackAnimParameter, false);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        
    }
}

