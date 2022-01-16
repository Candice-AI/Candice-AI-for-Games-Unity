using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    public class CandiceModuleMovement: CandiceBaseModule
    {
        public CandiceModuleMovement(string moduleName = "CandiceModuleMovement") : base(moduleName) { }
        public void MoveForward(Transform transform, CandiceAIController aiController)
        {
            transform.position += transform.forward * aiController.MoveSpeed * Time.deltaTime;
        }

        public void LookAt(Transform transform, CandiceAIController aiController)
        {
            transform.LookAt(aiController.LookPoint);
        }
        public void LookAway(Transform transform, CandiceAIController aiController)
        {
            transform.LookAt(-aiController.MainTarget.transform.forward);
        }
    }
}

