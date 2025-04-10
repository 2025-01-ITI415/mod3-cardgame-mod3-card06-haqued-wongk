using UnityEngine;

public class DeckLoader : MonoBehaviour
{
    public DeckData deck;

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Deck2");
        if (jsonFile != null)
        {
            deck = JsonUtility.FromJson<DeckData>(jsonFile.text);
            Debug.Log("Deck loaded with " + deck.cards.Length + " cards.");
        }
        else
        {
            Debug.LogError("Deck2.json not found in Resources!");
        }
    }
}
