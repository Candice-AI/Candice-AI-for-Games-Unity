using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CandiceAIforGames.AI
{
    public class CandiceBaseModule
    {
        private string moduleName;
        private bool enableDebug = true;

        public CandiceBaseModule(string moduleName)
        {
            ModuleName = moduleName;
        }

        public string ModuleName { get => moduleName; set => moduleName = value; }
        public bool EnableDebug { get => enableDebug; set => enableDebug = value; }
    }
}
