using System.Collections.Generic;
using static Enums;
using static AllLibary;
namespace BaseCharacter.Effects
{
    public interface IApplyEffects
    {
        public void ApplyAttribute(Effect attribute)
        {
            ApplyAttribute(attribute.Attributes, attribute.Strength, attribute.Time, attribute.Option);
            foreach (string search in attribute.GetOtherEffects())
            {
                ApplyAttribute(ItemLibary.SearchLibaryForAttribute(search));
            }
        }
        public void ApplyAttribute(AttributesTemplete attribute)
        {
            ApplyAttribute(attribute.GetAttributes(), attribute.GetStrength(), attribute.GetTime(), attribute.GetOption());
        }
        public void ApplyAttribute(List<Effect> attribute)
        {
            if (attribute.Count <= 0) return;
            for (int i = 0; i < attribute.Count; i++)
            {
                ApplyAttribute(attribute[i].Attributes, attribute[i].Strength, attribute[i].Time, attribute[i].Option);
            }
        }
        public void ApplyAttribute(Effect[] attribute)
        {
            if (attribute.Length <= 0) return;
            for (int i = 0; i < attribute.Length; i++)
            {
                ApplyAttribute(attribute[i].Attributes, attribute[i].Strength, attribute[i].Time, attribute[i].Option);
            }
        }
        public void ApplyAttribute(List<AttributesTemplete> attribute)
        {
            if (attribute.Count <= 0) return;
            for (int i = 0; i < attribute.Count; i++)
            {
                ApplyAttribute(attribute[i].GetAttributes(), attribute[i].GetStrength(), attribute[i].GetTime(), attribute[i].GetOption());
            }
        }
        /// <summary>
        /// Apply attributes
        /// </summary>
        /// <param name="attributes">the attribute</param>
        /// <param name="strength">How strong the attribute is</param>
        /// <param name="time">How long it lasts</param>
        /// <param name="options">Usually contains tick rate, but sometimes contains addiotnal option such as Lift value in Floataitons</param>
        public void ApplyAttribute(List<Attributes> attributes, List<float> strength, List<float> time, List<float> options)
        {
            if (attributes.Count <= 0) return;
            for (int i = 0; i < attributes.Count; i++)
            {
                ApplyAttribute(attributes[i], strength[i], time[i], options[i]);
            }
        }
        public void ApplyAttribute(Attributes attributes, float strength, float time, float options);
    }
    public interface IEffects
    {
        
    }
}