using BaseCharacter;
using BaseCharacter.FiveSenses;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    [SerializeField] private EntityTemplete entity;
    private List<TrackingData> enemySighted = new List<TrackingData>();
    private List<TrackingData> thingSighted = new List<TrackingData>();
    private List<TrackingData> friendSighted = new List<TrackingData>();
    private float TimeToCheckAgain;
    // Start is called before the first frame update
    void Start()
    {
        if (entity == null)
        {
            entity.GetComponentInParent<EntityTemplete>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > TimeToCheckAgain)
        {
            Ray ray = new Ray(transform.position, entity.transform.rotation * transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(ray, entity.Player.Vision);
            for (int i = 0; i < hits.Length; i++)
            {
                try
                {
                    if (hits[i].collider.TryGetComponent(out EntityTemplete templete) && !templete.Equals(entity))
                    {
                        WhoToAttack.CanAttackOtherEntity(entity, templete, ref enemySighted);
                        thingSighted.Add(new TrackingData(templete));
                    }
                    if (hits[i].collider.TryGetComponent(out Walking walking))
                    {
                        WhoToAttack.CanAttackOtherPlayer(entity, walking, ref enemySighted);
                        thingSighted.Add(new TrackingData(walking));
                    }
                    if (hits[i].collider.TryGetComponent(out IHasCharacter character) && !character.Equals(entity))
                    {
                        WhoToAttack.CanAttackOtherPlayer(entity, character, ref enemySighted);
                        thingSighted.Add(new TrackingData(character.GetName(),character.GetCharName(),character.GetGameObject()));
                    }
                }
                catch (NullReferenceException ex) 
                {
                    Debug.LogWarning("Entity does not excists. " + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    Debug.LogAssertion("Entity does not excists. " + ex.StackTrace);
                }
            }
            TimeToCheckAgain = Time.time + entity.GetEyeUpdateRate;
        }
    }
    public List<TrackingData> GetSightedEntites()
    {
        return enemySighted;
    }
}
//Sleeper votes:

//Galatz: 6
//Typical: 1

// Bug in bathroom
// Ender, Hotpot,

//Camo: ender, gelatz, ben
//

//Vote:
//Galazt: 3
//Liv: 3


//Ben: Invest
//Typical: Defense --> Thinks 
//Nerdy: Invest
//
//Hotpot: Support
//Liv: 

//PPL: Asranger, Ben, Hotpot, Liv, Typical, Zandrum, Nerdy


//Defense: Veteran (Zandrum),
//Support: Nerdy
//Invest: Ben, 