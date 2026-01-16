using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkInteract : Interacable
{
    //[SerializeField] DialogueContainer dialogueContainer;

    NPCDefinition npcDefinition;
    NPCCharacter npcCharacter;

    private void Awake()
    {
        npcCharacter = GetComponent<NPCCharacter>();
        npcDefinition = npcCharacter.character;
    }
    public override void Interact(Player Player)
    {
        
        DialogueContainer dialogueContainer = npcDefinition.dialogueContainer[Random.Range(0,npcDefinition.dialogueContainer.Count)];
        npcCharacter.IncreaseRelationship(0.1f);
        GameManager.Instance.dialogueSystem.Initialize(dialogueContainer);
        npcCharacter.talkedToday = true;
    }
}
