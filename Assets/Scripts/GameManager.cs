using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Play Cells (7)")]
    public GameObject playCellPrefab;
    public GameObject[] playCells = new GameObject[7];
    public float playCellXOffset = 3f;
    public float playCellYOffsetForClosedCards = 0.2f;
    public float playCellYOffsetForOpenCards = 0.5f;
    

    [Header("Collection Cells (4)")]
    public GameObject collCellPrefab;
    public float collCellXOffset = 3f;
    public GameObject[] collCells = new GameObject[4];

    public DeckClosedCards m_deckClosedCards;

    [Header("All cards")]
    public GameObject[] allCards = new GameObject[52];
    public List<GameObject> playCards;
    

    [Header("Card prefab")]
    public GameObject cardPrefab;

    [Header("Sprites")]
    //Card Suits
    public Sprite[] suitSprites = new Sprite[4];
    //Card Values
    public Sprite[] cardValueSprites = new Sprite[13];

    [Header("Values")]
    [Tooltip("Number of moves")]
	public int moves = 0;
    [Tooltip("Player score")]
	public int score = 0;



    //Index array of cards
   public int allCardsIndex = 0;

    // Use this for initialization
    void Start()
    {
        m_deckClosedCards = GameObject.Find("DeckClosedCards").GetComponent<DeckClosedCards>();
        //Instantiate all 52 cards
        InstantiateCards();
        //Shuffle cards
        Shuffle();
        //Instantiate all play cells (7)
        InstantiatePlayCells();
        //Instantiate all collection cells (4)
        InstantiateCollCells();

        allCardsIndex = 0; 
    }

    void InstantiateCards()
    {
        for (int iSuit = 0; iSuit < 4; iSuit++)
        {
            for (int iValue = 0; iValue < 13; iValue++)
            {
                Vector3 newPosition = new Vector3(m_deckClosedCards.transform.position.x, m_deckClosedCards.transform.position.y, -m_deckClosedCards.cardCount);
                GameObject cardGO = Instantiate(cardPrefab, newPosition, Quaternion.identity);
                Card card = cardGO.GetComponent<Card>();
                card.isOpen = false;

                // Apply the suit
                card.suit = iSuit;
                card.suitRenderer.sprite = suitSprites[iSuit];

                card.ApplySettings();
                

                // Apply the value
                card.value = iValue + 1;
                card.valueRenderer.sprite = cardValueSprites[iValue];
                if (card.isCardColorRed)
                {
                    card.valueRenderer.color = Color.red;
                }
                else
                {
                    card.valueRenderer.color = Color.black;
                }

                // Rename card GameObject
                card.name = "Card (" + card.value + ", " + (card.isCardColorRed ? "Rosso" : "Nero") + " (" + card.suitRenderer.sprite.name + "))";

                // Add a card to an array of deck cards
                allCards[allCardsIndex] = card.gameObject;
                allCardsIndex++;


            }
        }
    }

    void Shuffle()

    {
        int[] allCardsIndexes = new int[52];
        for (int i = 0; i < allCardsIndexes.Length; i++)
        {
            allCardsIndexes[i] = i;
        }
        for (int i = 0; i < allCardsIndexes.Length; i++)
        {
            int index = allCardsIndexes[i];
            int randomIndex = UnityEngine.Random.Range(0, allCardsIndexes.Length - 1);
            allCardsIndexes[i] = allCardsIndexes[randomIndex];
            allCardsIndexes[randomIndex] = index;

            
        }

        for (int i = allCards.Length - 1; i >= 0; i--)
        {
            GameObject cardGO = allCards[allCardsIndexes[i]];
            Card card = cardGO.GetComponent<Card>();
            m_deckClosedCards.allCardsInDeck[i] = cardGO;

            // Place the card on top of another one
            Vector3 newPosition = new Vector3(m_deckClosedCards.transform.position.x, m_deckClosedCards.transform.position.y, m_deckClosedCards.cardCount - 1);
            cardGO.transform.position = newPosition;
            card.frontSR.sortingOrder = m_deckClosedCards.cardCount;
            card.backSR.sortingOrder = m_deckClosedCards.cardCount;
            card.suitRenderer.sortingOrder = m_deckClosedCards.cardCount;
            card.valueRenderer.sortingOrder = m_deckClosedCards.cardCount;

            m_deckClosedCards.cardCount++;
        }

        allCardsIndex = 0;
    }
    

    public void InstantiatePlayCells()
    {
        // Instantiate play cells (7)
        for (int iPlayCell = 0; iPlayCell < 7; iPlayCell++)
        {
            Vector3 newPositionForPlayCell = new Vector3(playCellPrefab.transform.position.x + iPlayCell * playCellXOffset, playCellPrefab.transform.position.y, playCellPrefab.transform.position.z);
            GameObject playCellGO = Instantiate(playCellPrefab, newPositionForPlayCell, Quaternion.identity);
            playCells[iPlayCell] = playCellGO;

            // Filling play cells with cards
            for (int iPlayCellCards = 0; iPlayCellCards < iPlayCell + 1; iPlayCellCards++)
            {
                // Move the card from the deck to the play cell
                Card m_card = m_deckClosedCards.allCardsInDeck[allCardsIndex].GetComponent<Card>();

                PlayCells m_playCells = playCellGO.GetComponent<PlayCells>();

                // Place the card on top of another one
                m_card.frontSR.sortingOrder = m_playCells.cardCount;
                m_card.backSR.sortingOrder = m_playCells.cardCount;
                m_card.suitRenderer.sortingOrder = m_playCells.cardCount ;
                m_card.valueRenderer.sortingOrder = m_playCells.cardCount;
                m_playCells.cardCount++;
                m_card.inPlayCell = true;

                // Our card is no longer in deck
                m_card.inDeck = false;

                // Expand map
                Vector3 newPositionForCard;
                if (iPlayCellCards == iPlayCell)
                {
                    m_card.isOpen = true;
                    m_card.isFirst = true;
                    m_card.ApplySettings();

                    newPositionForCard = new Vector3(m_playCells.transform.position.x, m_playCells.transform.position.y - iPlayCellCards * playCellYOffsetForClosedCards, -m_playCells.cardCount);
                }
                else
                {
                    newPositionForCard = new Vector3(m_playCells.transform.position.x, m_playCells.transform.position.y - iPlayCellCards * playCellYOffsetForClosedCards, -m_playCells.cardCount);
                }

                m_card.transform.position = newPositionForCard;
                allCardsIndex++;
            }
        }
    }

    void InstantiateCollCells()
    {
        for (int iCollCell = 0; iCollCell < 4; iCollCell++)
        {
            Vector3 newPositionForCollCell = new Vector3(collCellPrefab.transform.position.x + iCollCell * collCellXOffset, collCellPrefab.transform.position.y, collCellPrefab.transform.position.z);
            GameObject cell4GO = Instantiate(collCellPrefab, newPositionForCollCell, Quaternion.identity);
            collCells[iCollCell] = cell4GO;
        }
    }

}
