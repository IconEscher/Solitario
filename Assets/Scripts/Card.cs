using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    GameManager gameManager;

    [Header("Renderers")]
    //public SpriteRenderer backgroundSR;
    public SpriteRenderer frontSR;
    public SpriteRenderer backSR;
    public SpriteRenderer suitRenderer;
    public SpriteRenderer valueRenderer;

    public Sprite openedCardTexture;
    public Sprite closedCardTexture;

    [Header("Bools")] [Tooltip("Is the card Red or Black?")]
    public bool isCardColorRed = false;
    [Tooltip("Check if we are moving a card")]
    public bool mouseDragging = false;
    [Tooltip("Check if the card is the first in order, if true = flip")]
    public bool isFirst = false;

    [Tooltip("Check if is in deck - not played yet")]
    public bool inDeck = true;

    [Tooltip("Check is is in one of the 7 play cells")]
    public bool inPlayCell = false;

    [Tooltip("Check if is in one of the 4 collection cells")]
    public bool inCollCell = false;

    [Tooltip("Is the card opened or closed?")]
    public bool isOpen = false;

    [Tooltip("Check if the card is opened but not played")]
    public bool inDeckHelper = false;

    [Header("Values")]
    public int sortingOrderAdd = 5;
    public int sortingOrderAddZ = 5;
    [Tooltip("0-1 = Red, 2-3 = Black")]
    public int suit = 0;
    [Tooltip("Value of the card - A = 1, J = 11, Q = 12, K = 13")]
    public int value = 0;
    public Vector3 oldPositionInCell;
    public int backgroundSRSortingOrder;
    public int suitSRSortingOrder;
    public int valueSRSortingOrder;

    // Old card location
    Vector3 oldCardPosition;
    Transform oldParent;

    public void ApplySettings()
    {
        
        if (isOpen)
        {
            StartCoroutine(Flip(this.transform, new Vector3(0f, 180f, 0f), 0.5f));
        }

        // Set bool Color
        if (suit < 2)
        {
            isCardColorRed = true;
        }
        else
        {
            isCardColorRed = false;
        }
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        //Drawing the map
        if (!mouseDragging && transform.parent != null && transform.parent.GetComponent<Card>() != null)
        {
            Card parentCard = transform.parent.GetComponent<Card>();
            //SpriteRenderer parentCardSR = parentCard.GetComponentInChildren<SpriteRenderer>();
            frontSR.sortingOrder += 1;
            backSR.sortingOrder += 1;
            suitRenderer.sortingOrder = frontSR.sortingOrder + 1;
            valueRenderer.sortingOrder = frontSR.sortingOrder + 1;
            parentCard.isFirst = false;
        }
    }
    #region OnMouseDrag
    void OnMouseDrag()
    {
        mouseDragging = true;

        if (isOpen 
            && (inDeckHelper || inPlayCell || inCollCell))
        {
            Vector3 newPosition = Camera.main.ScreenPointToRay(Input.mousePosition).origin + oldCardPosition;
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z + 10);
           // suitRenderer.sortingOrder += 1;
           // valueRenderer.sortingOrder += 1;
        }
    }
    #endregion

    #region OnMouseDown
    void OnMouseDown()
    {
        // Save card position
        oldCardPosition = transform.position - Camera.main.ScreenPointToRay(Input.mousePosition).origin;

        #region Closed_Card
        if (!isOpen)
        {
            // If the card is in the deck closed
            if (inDeck)
            {
                // Settings cards
                isOpen = true;
                ApplySettings();

                // Move the card from the deckClosed into the deckOpened
                DeckOpenedCards DOC = GameObject.Find("DeckOpenedCards").GetComponent<DeckOpenedCards>();
                Vector3 newPosition = new Vector3(DOC.transform.position.x, DOC.transform.position.y, -DOC.cardCount);
                transform.position = newPosition;
                frontSR.sortingOrder = DOC.cardCount;
                backSR.sortingOrder = DOC.cardCount;
                suitRenderer.sortingOrder = DOC.cardCount + 1;
                valueRenderer.sortingOrder = DOC.cardCount + 1;
                DOC.cardCount++;

                isFirst = true;
            }
            // if the card in Playcell and is the first
            if (inPlayCell && isFirst)
            {
                // Open card
                isOpen = true;
                ApplySettings();
            }
        }
        #endregion

        #region Opened_Card
                // Save the position of the card in the cell
                if (isOpen
                    && (inDeckHelper || inPlayCell || inCollCell))
                {
                    oldPositionInCell = transform.position;
                    oldParent = transform.parent;
                    transform.SetParent(null);

                    backgroundSRSortingOrder = frontSR.sortingOrder;
                 //   backSR.sortingOrder = backSR.sortingOrder + 1;
                    suitSRSortingOrder = suitRenderer.sortingOrder;
                    valueSRSortingOrder = valueRenderer.sortingOrder;

                    // Make a map visible on top of all maps
                    frontSR.sortingOrder += sortingOrderAdd;
                    backSR.sortingOrder += sortingOrderAdd;
                    suitRenderer.sortingOrder += sortingOrderAdd;
                    valueRenderer.sortingOrder += sortingOrderAdd;
                    transform.position = new Vector3(transform.position.x, transform.position.y, -sortingOrderAddZ);
                }
        #endregion
    }
    #endregion

    #region OnMouseUp
        void OnMouseUp()
        {
            // Not dragging anymore
            mouseDragging = false;

            if (isOpen && !inDeck)
            {
                // Disable box collider
                BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
                boxCollider2D.enabled = false;

                RaycastHit2D hit;
                hit = Physics2D.Raycast(transform.position + (-oldCardPosition), Vector3.forward);

                // Check if can be moved
                Card otherCard = null;
                PlayCells m_playCells = null;
                CollectionCells m_collCells = null;

                if (hit.collider != null)
                {
                    otherCard = hit.collider.GetComponent<Card>();
                    m_playCells = hit.collider.GetComponent<PlayCells>();
                    m_collCells = hit.collider.GetComponent<CollectionCells>();
                }

                if (m_playCells != null // Is there a playCell
                    && m_playCells.isFirst // There are no other cards above the cell
                    && value == 13 // Only the king can land in an empty playCell
                    )
                {
                    // Attach the card to this cell
                    transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z + 10);
                    transform.SetParent(null);

                    SpriteRenderer playCellSR = m_playCells.GetComponent<SpriteRenderer>();
                    frontSR.sortingOrder = playCellSR.sortingOrder + 1;
                    backSR.sortingOrder = playCellSR.sortingOrder + 1;
                    suitRenderer.sortingOrder = frontSR.sortingOrder +1;
                    valueRenderer.sortingOrder = frontSR.sortingOrder +1;
                    m_playCells.isFirst = false;
                    isFirst = true;

                    inCollCell = false;
                    inDeckHelper = false;
                    inPlayCell = true;
                }
                else
                // collCell + Ace check
                if (m_collCells != null // Is there a collCell
                    && m_collCells.isFirst // There are no other cards above the cell
                    && value == 1 // Only an ace can land in an empty collCell
                    )
                {
                    // Attach the card to this cell
                    transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z - 1);
                    transform.SetParent(null);

                    SpriteRenderer collCellSR = m_collCells.GetComponent<SpriteRenderer>();
                    frontSR.sortingOrder = collCellSR.sortingOrder + 1;
                    backSR.sortingOrder = collCellSR.sortingOrder + 1;
                    suitRenderer.sortingOrder = frontSR.sortingOrder + 1;
                    valueRenderer.sortingOrder = frontSR.sortingOrder + 1;

                    m_collCells.isFirst = false;
                    isFirst = true;

                    inPlayCell = false;
                    inDeckHelper = false;
                    inCollCell = true;
                }
                else
                // On an open card in cell inCell7
                if (otherCard != null // Is there another card
                    && otherCard.inPlayCell // The other card must be in one of 7 cells
                    && otherCard.isFirst
                    && otherCard.isOpen // Is another card open?
                    && otherCard.isCardColorRed != isCardColorRed // Is the color of the other card
                    && otherCard.value == value + 1 // The value of the other card should be greater by one
                    )
                {
                    // Attach a map to this map
                    transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y - gameManager.playCellYOffsetForOpenCards, hit.collider.transform.position.z - 1);
                    transform.SetParent(hit.collider.transform);

                    frontSR.sortingOrder = otherCard.frontSR.sortingOrder + 1;
                    backSR.sortingOrder = otherCard.backSR.sortingOrder + 1;
                    suitRenderer.sortingOrder = otherCard.suitRenderer.sortingOrder + 1;
                    valueRenderer.sortingOrder = otherCard.valueRenderer.sortingOrder + 1;

                    otherCard.isFirst = false;
                    if (transform.childCount < 3)
                    {
                        isFirst = true;
                    }

                    inCollCell = false;
                    inDeckHelper = false;
                    inPlayCell = true;
                }
                else
                // To an open card in cell inCell4
                if (otherCard != null // Is there another card
                    && isFirst
                    && otherCard.inCollCell // The other card must be in one of the 4 cells
                    && otherCard.isFirst
                    && otherCard.isOpen // Is another card open?
                    && otherCard.suit == suit // The colors must match
                    && otherCard.value == value - 1 // The value of the other card should be less by one
                    )
                {
                    // Attach a map to this map
                    transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y, hit.collider.transform.position.z - 1);
                    transform.SetParent(null);

                    frontSR.sortingOrder = otherCard.frontSR.sortingOrder + 1;
                    backSR.sortingOrder = otherCard.backSR.sortingOrder + 1;
                    suitRenderer.sortingOrder = frontSR.sortingOrder + 1;
                    valueRenderer.sortingOrder = frontSR.sortingOrder + 1;

                    otherCard.isFirst = false;
                    isFirst = true;

                    inPlayCell = false;
                    inDeckHelper = false;
                    inCollCell = true;

                }
                // Return the card to its original location
                else if (!inDeck)
                {
                    transform.position = oldPositionInCell;
                    transform.SetParent(oldParent);

                    frontSR.sortingOrder = backgroundSRSortingOrder;
                    backSR.sortingOrder = backgroundSRSortingOrder;
                    suitRenderer.sortingOrder = suitSRSortingOrder;
                    valueRenderer.sortingOrder = valueSRSortingOrder;

                    // Reset the hit beam
                    hit = new RaycastHit2D();
                }

                // Automatically open the map under the old place of this map
                if (hit.collider != null && (m_playCells != null || m_collCells != null || otherCard != null))
                {
                    RaycastHit2D hitDownCard;
                    hitDownCard = Physics2D.Raycast(oldPositionInCell, Vector3.forward);
                    Card oldDownCard = null;
                    PlayCells cell7Down = null;
                    CollectionCells cell4Down = null;

                    if (hitDownCard.collider != null)
                    {
                        oldDownCard = hitDownCard.collider.GetComponent<Card>();
                        cell7Down = hitDownCard.collider.GetComponent<PlayCells>();
                        cell4Down = hitDownCard.collider.GetComponent<CollectionCells>();
                    }

                    if (oldDownCard != null)
                    {
                        oldDownCard.isFirst = true;
                        oldDownCard.isOpen = true;
                        oldDownCard.ApplySettings();
                    }
                    else
                    if (cell7Down != null)
                    {
                        cell7Down.isFirst = true;
                    }
                    else
                    if (cell4Down != null)
                    {
                        cell4Down.isFirst = true;
                    }
                }

                // Enable box collider
                boxCollider2D.enabled = true;
            }

            if (inDeck)
            {
                // No longer in the deck
                inDeck = false;
                inDeckHelper = true;
            }


        }
    #endregion


    public IEnumerator Flip(Transform thisTransform, Vector3 degrees, float time)
    {
            Quaternion startRotation = thisTransform.rotation;
            Quaternion endRotation = thisTransform.rotation * Quaternion.Euler(degrees);
            float rate = 1.0f / time;
            float t = 0.0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime * rate;
                thisTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

        }
        yield return null;
    }
}
