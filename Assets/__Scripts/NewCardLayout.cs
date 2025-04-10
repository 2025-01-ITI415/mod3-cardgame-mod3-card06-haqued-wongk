using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The different states a CardGolf card can be in
/// </summary>
public enum eGolfCardState { drawpile, tableau, target, discard }

/// <summary>
/// A subclass of Card for Golf Solitaire
/// </summary>
public class CardGolf : Card
{
    [Header("Dynamic: CardGolf")]
    public eGolfCardState state = eGolfCardState.drawpile;

    // This list stores other cards that hide this one (useful for face-up logic)
    public List<CardGolf> hiddenBy = new List<CardGolf>();

    // Used to match layout ID from JSON
    public int layoutID;

    // Stores layout info from the JSON
    public JsonLayoutSlot layoutSlot;
}
