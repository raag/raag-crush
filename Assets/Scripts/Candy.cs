using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{

    private static Color selectedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    private static Candy previousSelected = null;
    private SpriteRenderer spriteRenderer;
    private bool isSelected = false;
    public int id;


    private Vector2[] adjacentDirections = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void SelectCandy()
    {
        isSelected = true;
        spriteRenderer.color = selectedColor;
        previousSelected = gameObject.GetComponent<Candy>();    
    }


    private void DeSelectCandy()
    {
        isSelected = false;
        spriteRenderer.color = Color.white;
        previousSelected = null;
    }


    private void OnMouseDown() 
    {
        if (spriteRenderer == null 
            || BoardManager.sharedInstance.isShifting) 
        {
           return;
        } 

        if (isSelected)
        {
            DeSelectCandy();
        } 
        else
        {
            if (previousSelected != null)
            {
                if (this.CanSwipe()) 
                {
                    SwapSprite(previousSelected);
                    previousSelected.FindAllMatches();
                    this.FindAllMatches();
                    GUIManager.sharedInstance.MovesCounter --; 
                }
                previousSelected.DeSelectCandy();
            }
            else 
            {
                SelectCandy();
            }
        }
    }


    public void SwapSprite(Candy newCandy)
    {
        SpriteRenderer newCandySpriteRenderer = newCandy.GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == newCandySpriteRenderer.sprite)
        {
            return;
        }
        Sprite spriteBackup = spriteRenderer.sprite;
        spriteRenderer.sprite = newCandySpriteRenderer.sprite;
        newCandySpriteRenderer.sprite = spriteBackup;

        int idBackup = id;
        id = newCandy.id;
        newCandy.id = idBackup;
    }


    private GameObject GetNeighbor(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direction);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }


    private  List<GameObject> GetAllNeighbors()
    {
        List<GameObject> neighbors = new List<GameObject>();
        foreach (Vector2 direction in adjacentDirections) 
        {
            GameObject neighbor = this.GetNeighbor(direction);
            neighbors.Add(neighbor);
        }

        return neighbors;
    }


    private bool CanSwipe()
    {
        List<GameObject> neighbors = this.GetAllNeighbors();
        return neighbors.Contains(previousSelected.gameObject);

    }


    private List<GameObject> FindMatch(Vector2 direction)
    {
        List<GameObject> matchingCandies = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(
            this.transform.position,
            direction
        );

        while (hit.collider != null && isSameCandy(hit.collider))
        {
            matchingCandies.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, direction);
        }
        
        return matchingCandies;
    }

    private bool isSameCandy(Collider2D collider)
    {
        SpriteRenderer colliderRenderer = collider.GetComponent<SpriteRenderer>();
        return colliderRenderer.sprite == spriteRenderer.sprite;
    }

    private bool ClearMatch(Vector2[] directions)
    {
        List<GameObject> matchingCandies = new List<GameObject>();

        foreach (Vector2 direction in directions)
        {
            List<GameObject> matchesForDirection = this.FindMatch(direction);
            matchingCandies.AddRange(matchesForDirection);
        }
        if(matchingCandies.Count >= BoardManager.MinNeighborsToMatch)
        {
            foreach (GameObject candy in matchingCandies)
            {
               candy.GetComponent<SpriteRenderer>().sprite = null;
            }

            return true;
        }

        return false;
    }

    public void FindAllMatches()
    {
        if (spriteRenderer.sprite == null)
        {
            return;
        }

        bool horizontalMatches = ClearMatch(new Vector2[2]{
            Vector2.left, Vector2.right
        });
        
        bool verticalMatches = ClearMatch(new Vector2[2]{
            Vector2.up, Vector2.down
        });

        if (horizontalMatches || verticalMatches)
        {
            spriteRenderer.sprite = null;
            StopCoroutine(BoardManager.sharedInstance.FindNullCandies());
            StartCoroutine(BoardManager.sharedInstance.FindNullCandies());
        }

    }
}
