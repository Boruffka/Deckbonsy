using UnityEngine;


[System.Serializable]
public class DialogueData
{
    public string npcLine;
    public string[] playerChoices;
    public int[] endings; // Id zako�cze� 
    public Sprite npcImage;  
    public Sprite backgroundImage;
}
