using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;

[Serializable]
public class CharacterStat
{
    public string name;
    public float baseValue;
    private bool showStat;
    private bool showModifiers;
    public virtual float value
    {
        get
        {
            if (isDirty || baseValue != lastBaseValue)
            {
                lastBaseValue = baseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }

    protected bool isDirty = true;
    protected float _value;
    protected readonly List<StatModifier> statModifiers;
    protected readonly ReadOnlyCollection<StatModifier> StatModifiers;
    protected float lastBaseValue = float.MaxValue;

    public CharacterStat()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }
    public CharacterStat(string name, float baseValue) : this()
    {
        this.name = name;
        this.baseValue = baseValue;
    }

    public virtual void AddModifier(StatModifier mod)
    {
        isDirty = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifiersOrder);
    }

    protected virtual int CompareModifiersOrder(StatModifier a, StatModifier b)
    {
        if (a.order < b.order)
            return -1;
        else if (a.order > b.order)
            return 1;
        return 0;
    }
    public virtual bool RemoveModifier(StatModifier mod)
    {
        
        if(statModifiers.Remove(mod))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;
        for(int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].source == source)
            {
                isDirty = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }
    protected virtual float CalculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPercentAdd = 0;
        for(int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];
            if(mod.Type == StatModType.Flat)
            {
                finalValue += mod.value;
            }
            else if(mod.Type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.value;

                if(i+1 >= statModifiers.Count || statModifiers[i+1].Type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if(mod.Type == StatModType.PercentMult)
            {
                finalValue *= 1 + mod.value;
            }
        }

        return (float)(Math.Round(finalValue, 4));
    }
    public virtual void Draw()
    {
        GUIStyle _style = EditorStyles.foldout;
        FontStyle previousStyle = _style.fontStyle;
        _style.fontStyle = FontStyle.Bold;
        GUIContent label1 = new GUIContent(name);
        showStat = EditorGUILayout.Foldout(showStat, name, _style);
        _style.fontStyle = previousStyle;

        if (showStat)
        {
            GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            GUIContent label = new GUIContent("Base Value", "The base value before applying any modfiers.");
            EditorGUILayout.FloatField(label, baseValue);
            label = new GUIContent("Final Value", "The value after applying all modifiers.");
            EditorGUILayout.FloatField(label, CalculateFinalValue());
            //GUIContent labe
            GUILayout.Space(4);
            //style.fontSize = 16;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (!showModifiers)
                GUILayout.BeginVertical("box");
            else
                GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Stat Modifiers", style);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //showModifiers = EditorGUILayout.Foldout(showModifiers, "Modifiers");
            //if (showModifiers)
            //{

            //}
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();

            GUILayout.Space(4);
            if (GUILayout.Button("Add Modifier"))
            {
                AddModifierWindow window = EditorWindow.GetWindow<AddModifierWindow>();
                window.stat = this;
                window.minSize = new Vector2(400f, 100f);
                window.maxSize = new Vector2(400f, 100f);
                window.Show();
            }
            if (statModifiers.Count > 0)
            {
                
                showModifiers = EditorGUILayout.Foldout(showModifiers, "Modifiers");
                if (showModifiers)
                {
                    foreach (StatModifier mod in statModifiers)
                    {
                        GUILayout.Space(2);
                        GUILayout.BeginVertical("box");
                        mod.value = EditorGUILayout.FloatField("Value", mod.value);
                        label = new GUIContent("Modifier Type", "");
                        EditorGUILayout.EnumPopup(label, mod.Type);
                        EditorGUILayout.LabelField("Order", mod.order.ToString());
                        GUILayout.Space(2);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Remove Modifier"))
                        {
                            bool isRemove = EditorUtility.DisplayDialog("Remove Modifier", "Are you sure you want to remove this modifier?", "Yes", "No");
                            if(isRemove)
                            {
                                statModifiers.Remove(mod);
                                return;
                            }
                            
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }
                }
                
            }


            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }
        


        
    }

}
public class AddModifierWindow : EditorWindow
{
    public CharacterStat stat;
    float value;
    int order;
    StatModType type;
    private void OnGUI()
    {
        GUIContent label = new GUIContent("Value");
        if (stat != null)
        {
            GUILayout.Space(2);
            GUILayout.BeginVertical("box");
            value = EditorGUILayout.FloatField(label, value);
            type = (StatModType)EditorGUILayout.EnumPopup("Type", type);
            order = EditorGUILayout.IntField("Order", order);
            GUILayout.EndVertical();
            GUILayout.Space(2);
            if(GUILayout.Button("Add"))
            {
                if(type == 0)
                {
                    EditorUtility.DisplayDialog("Add Modifier", "Please make sure to select a Modifier Type.", "Okay");
                    return;
                }
                

                StatModifier mod = new StatModifier(value, type, order);
                stat.AddModifier(mod);
                Close();
            }
        }
        if(GUILayout.Button("Close"))
        {
            Close();
        }
    }
}
