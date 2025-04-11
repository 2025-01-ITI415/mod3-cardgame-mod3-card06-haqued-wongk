using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The different states a CardGolf card can be in
/// </summary>
public enum eGolfCardState
{
    drawpile,
    tableau,
    target,
    discard
}

/// <summary>
/// A subclass of Card for Golf Solitaire
/// </summary>
public class CardGolf : Card
{
    [Header("Dynamic: CardGolf")]

    public eGolfCardState state = eGolfCardState.drawpile;
    public List<CardGolf> hiddenBy = new List<CardGolf>();
    public int layoutID;
    public JsonLayoutSlot layoutSlot;
    public bool isFaceUp
    {
        get
        {
            return state == eGolfCardState.target || state == eGolfCardState.discard || layoutSlot.faceUp;
        }
    }
}
