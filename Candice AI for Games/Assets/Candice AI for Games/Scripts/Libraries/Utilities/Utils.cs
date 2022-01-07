using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CandiceAIforGames.AI.Utils
{
    public class Utils
    {

        public static void LogClassInitialisation(object _class)
        {
            string[] className = (_class.ToString()).Split('.');
            Debug.Log(className[className.Length - 1] + ": Initialised.");
        }

        public static void LogDamageReceived(string name,float damage, float currentHP)
        {
            Debug.Log(name + ": Hit with " + damage + " damage. New Health = " + currentHP + "    Hit Points.");
        }

        public static void LogDamageDealt(string name,float damage)
        {
            Debug.Log(name + ": Dealing " + damage + " damage.");
        }
    }
}