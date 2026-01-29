using BaseCharacter.Effects;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
/// <summary>
/// Use to create an Attribute with ease
/// </summary>
public class AttributesTemplete
{
    [SerializeField] private string Name;
    [Tooltip("Set the attribute")]
    [SerializeField] private Attributes Attributes;
    [Tooltip("Set how strong the attribute is")]
    [SerializeField] private float Strength;
    [Tooltip("Set how long the attribute lasts")]
    [SerializeField] private float Time;
    [Tooltip("Set the option for each attribute.")]
    [SerializeField] private float Option;
    [Tooltip("Set a icon for the attribute")]
    [SerializeField] private Texture Texture;
    [Tooltip("Use to combine effects into 1")]
    [SerializeField] private string[] OtherEffects;

    public AttributesTemplete(string name, Attributes attributes, float strength, float time, float option)
    {
        Name = name;
        Attributes = attributes;
        Strength = strength;
        Time = time;
        Option = option;
        OtherEffects = null;
    }
    public AttributesTemplete(string name, Attributes attributes, float strength, float time, float option, params string[] otherEffects)
    {
        Name = name;
        Attributes = attributes;
        Strength = strength;
        Time = time;
        Option = option;
        OtherEffects = otherEffects;
    }
    public AttributesTemplete(AttributesTemplete other)
    {
        Name=other.Name;
        Attributes = other.Attributes;
        Strength = other.Strength;
        Time = other.Time;
        Option = other.Option;
        OtherEffects = other.OtherEffects;
        Texture = other.Texture;
    }

    public string GetName() { return Name; }
    public Attributes GetAttributes() { return Attributes; }
    public float GetStrength(float adj = 0) { return Strength + adj; }
    public float GetTime(float adj = 0) { return Time + adj; }
    public float GetOption(float adj = 0) { return Option + adj; }
    public Texture GetTexture()
    {
        if (Texture != null)
        { return Texture; }
        return null;
    }
    public Effect GetAttriStruct()
    {
        if (OtherEffects == null) 
        { return new Effect(Name, Attributes, Strength, Time, Option); }
        return new Effect(Name,Attributes,Strength,Time,Option, OtherEffects);
    }
    public void SetStrength(float value) { Strength = value; }
    public void SetTime(float value) { Time = value; }
    public void SetOption(float value) { Option = value; }
}
