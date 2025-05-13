using UnityEngine;


[System.Serializable]
public class DialogueData
{
    public string npcLine;
    public string[] playerChoices;
    public string[] playerExpandedResponses;
    public string[] npcResponses; 
    public int[] endings;
    public Sprite npcImage;
    public Sprite backgroundImage;
}
