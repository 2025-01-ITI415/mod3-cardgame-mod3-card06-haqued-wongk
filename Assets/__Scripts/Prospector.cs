using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Deck))]
[RequireComponent(typeof(JsonParseLayout))]
public class Prospector : MonoBehaviour
{
    private static Prospector S;

    [Header("Dynamic")]
    public List<CardGolf> drawPile;
    public List<CardGolf> discardPile;
    public List<CardGolf> mine;
    public CardGolf target;

    private Transform layoutAnchor;

    private Deck deck;
    private JsonLayout jsonLayout;

    private Dictionary<int, CardGolf> mineIdToCardDict;

    void Start()
    {
        if (S != null) Debug.LogError("Attempted to set S more than once!");
        S = this;

        jsonLayout = GetComponent<JsonParseLayout>().layout;
        deck = GetComponent<Deck>();
        deck.InitDeck();
        Deck.Shuffle(ref deck.cards);

        drawPile = ConvertCardsToCardGolfs(deck.cards);

        LayoutMine();

        MoveToTarget(Draw());
        UpdateDrawPile();
    }

    List<CardGolf> ConvertCardsToCardGolfs(List<Card> listCard)
    {
        List<CardGolf> listCP = new List<CardGolf>();
        CardGolf cp;
        foreach (Card card in listCard)
        {
            cp = card as CardGolf;
            listCP.Add(cp);
        }
        return (listCP);
    }

    CardGolf Draw()
    {
        if (drawPile.Count == 0)
        {
            CheckForGameOver();
            return null;
        }
        CardGolf cp = drawPile[0];
        drawPile.RemoveAt(0);
        return cp;
    }

    void LayoutMine()
    {
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
        }

        CardGolf cp;
        mineIdToCardDict = new Dictionary<int, CardGolf>();

        foreach (JsonLayoutSlot slot in jsonLayout.slots)
        {
            cp = Draw();
            cp.faceUp = slot.faceUp;
            cp.transform.SetParent(layoutAnchor);

            int z = int.Parse(slot.layer[slot.layer.Length - 1].ToString());

            cp.SetLocalPos(new Vector3(
                jsonLayout.multiplier.x * slot.x,
                jsonLayout.multiplier.y * slot.y,
                -z));

            cp.layoutID = slot.id;
            cp.layoutSlot = slot;
            cp.state = eCardState.mine;
            cp.SetSpriteSortingLayer(slot.layer);

            mine.Add(cp);
            mineIdToCardDict.Add(slot.id, cp);
        }
    }

    void MoveToDiscard(CardGolf cp)
    {
        cp.state = eCardState.discard;
        discardPile.Add(cp);
        cp.transform.SetParent(layoutAnchor);

        cp.SetLocalPos(new Vector3(
            jsonLayout.multiplier.x * jsonLayout.discardPile.x,
            jsonLayout.multiplier.y * jsonLayout.discardPile.y,
            0));

        cp.faceUp = true;
        cp.SetSpriteSortingLayer(jsonLayout.discardPile.layer);
        cp.SetSortingOrder(-200 + (discardPile.Count * 3));
    }

    void MoveToTarget(CardGolf cp)
    {
        if (target != null) MoveToDiscard(target);
        MoveToDiscard(cp);
        target = cp;
        cp.state = eCardState.target;
        cp.SetSpriteSortingLayer("Target");
        cp.SetSortingOrder(0);
    }

    void UpdateDrawPile()
    {
        CardGolf cp;
        for (int i = 0; i < drawPile.Count; i++)
        {
            cp = drawPile[i];
            cp.transform.SetParent(layoutAnchor);
            Vector3 cpPos = new Vector3();
            cpPos.x = jsonLayout.multiplier.x * jsonLayout.drawPile.x;
            cpPos.x += jsonLayout.drawPile.xStagger * i;
            cpPos.y = jsonLayout.multiplier.y * jsonLayout.drawPile.y;
            cpPos.z = 0.1f * i;
            cp.SetLocalPos(cpPos);

            cp.faceUp = false;
            cp.state = eCardState.drawpile;
            cp.SetSpriteSortingLayer(jsonLayout.drawPile.layer);
            cp.SetSortingOrder(-10 * i);
        }
    }

    public void SetMineFaceUps()
    {
        CardGolf coverCP;
        foreach (CardGolf cp in mine)
        {
            bool faceUp = true;
            foreach (int coverID in cp.layoutSlot.hiddenBy)
            {
                coverCP = mineIdToCardDict[coverID];
                if (coverCP == null || coverCP.state == eCardState.mine)
                {
                    faceUp = false;
                }
            }
            cp.faceUp = faceUp;
        }
    }

    void CheckForGameOver()
    {
        if (mine.Count == 0)
        {
            GameOver(true);
            return;
        }

        if (drawPile.Count > 0) return;

        foreach (CardGolf cp in mine)
        {
            if (cp.faceUp && Mathf.Abs(cp.rank - target.rank) == 1) return;
        }

        GameOver(false);
    }

    void GameOver(bool won)
    {
        if (won)
        {
            ScoreManager.TALLY(eScoreEvent.gameWin);
        }
        else
        {
            ScoreManager.TALLY(eScoreEvent.gameLoss);
        }

        CardSpritesSO.RESET();
        SceneManager.LoadScene("__Prospector_Scene_0");
    }

    static public void CARD_CLICKED(CardGolf cp)
    {
        switch (cp.state)
        {
            case eCardState.target:
                break;
            case eCardState.drawpile:
                S.MoveToTarget(S.Draw());
                S.UpdateDrawPile();
                ScoreManager.TALLY(eScoreEvent.draw);
                break;
            case eCardState.mine:
                bool validMatch = true;
                if (!cp.faceUp) validMatch = false;
                if (Mathf.Abs(cp.rank - S.target.rank) != 1) validMatch = false;

                if (validMatch)
                {
                    S.mine.Remove(cp);
                    S.MoveToTarget(cp);
                    S.SetMineFaceUps();
                    ScoreManager.TALLY(eScoreEvent.mine);
                }
                break;
        }
        S.CheckForGameOver();
    }
}
