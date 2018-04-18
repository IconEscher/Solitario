using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckClosedCards : MonoBehaviour
{
    // All cards in the deck
    //public List<GameObject> allCards;
    public GameObject[] allCardsInDeck = new GameObject[52];

    // Sprite for the end of the deck
    public Sprite endSprite;

    // Number of cards in the cell
    public int cardCount = 0;

    // Number of attempts
    public DeckOpenedCards m_DeckOpenedCards;

    void OnMouseDown()
    {
        // To recompile a deck
		Transform deckOpenedTransform = m_DeckOpenedCards.transform;
        cardCount = 0;

			RaycastHit2D hit;
			hit = Physics2D.Raycast (deckOpenedTransform.position, Vector3.back);

			if (hit) {
				GameObject cardGO = hit.collider.gameObject;
				Card card = cardGO.GetComponent<Card> ();

				// Place the card on top of another one
				Vector3 newPosition = new Vector3 (transform.position.x, transform.position.y, cardCount - 1);
				cardGO.transform.position = newPosition;
				card.frontSR.sortingOrder = cardCount;
                card.backSR.sortingOrder = cardCount;
                card.suitRenderer.sortingOrder = cardCount + 1;
				card.valueRenderer.sortingOrder = cardCount + 1;
				card.isOpen = false;
				card.ApplySettings ();
				card.inDeck = true;
				card.isFirst = false;
				card.inDeckHelper = false;

				cardCount++;
				m_DeckOpenedCards.cardCount--;
			}
		}
    

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
