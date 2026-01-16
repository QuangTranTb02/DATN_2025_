using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum Gender
{
    Male,
    Female,
    [InspectorName("Any [For romance only]")]
    Any
}
[Serializable]
public class PortraitsCollection
{
    public Texture2D normal;
    public Texture2D surprised;
    public Texture2D confused;
    public Texture2D angry;
}
[CreateAssetMenu(menuName = "Data/ NPC character")]
public class NPCDefinition : ScriptableObject
{
    public string Name = "Nameless";
    public Gender Gender = Gender.Male;
    public GameManager characterPrefab;
    PortraitsCollection portraitsCollection;

    [Header("Interaction")]
    public bool canBeRomaced;
    public Gender romanceableGender;
    public List<Item> itemLike;
    public List<Item> itemDislike;

    [Header("Dialogues")]
    public List<DialogueContainer> dialogueContainer;

    [Header("Schedule WIP")]
    public string schedule;
}
