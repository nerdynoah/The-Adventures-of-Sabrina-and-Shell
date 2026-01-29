using BaseCharacter.FiveSenses.HearingSounds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ear : MonoBehaviour, IHear
{
    [SerializeField] private SphereCollider rangeOfHearing;
    [SerializeField] private EntityTemplete templete;
    public List<Sound> CollectedSounds { get; private set; } =  new List<Sound>();
    public void RespondToSound(Sound sound)
    {
        CollectedSounds.Add(sound);
    }

    void Start()
    {
        if (rangeOfHearing == null)
        {
            rangeOfHearing = GetComponent<SphereCollider>();
        }
        rangeOfHearing.radius = templete.Player.Hear;
    }

}
