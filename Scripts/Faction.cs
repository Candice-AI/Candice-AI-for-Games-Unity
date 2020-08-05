using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Faction
{
    public string name;
    public string description;
    public Sprite artwork;
    public Color color;
    public List<FactionRelationship> relationships;

    public Faction(string name, string description, Sprite artwork, Color color)
    {
        this.name = name;
        this.description = description;
        this.artwork = artwork;
        this.color = color;
       
    }
}

[System.Serializable]
public class FactionRelationship
{
    public string main;
    public string other;

    public FactionRelationship()
    {

    }



}

