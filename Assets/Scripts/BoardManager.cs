﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager sharedInstance;
    public List<Sprite> prefabs = new List<Sprite>();
    public GameObject currentCandy;
    public int xSize, ySize;

    private GameObject[,] candies;
    public bool isShifting{ get; set; }

    private Candy selectedCandy;
    public const int MinNeighborsToMatch = 2;

    public AudioSource backgroundMusic;


    void Start()
    {
        if (sharedInstance == null) 
        {
            sharedInstance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
        Vector2 offset = currentCandy.GetComponent<BoxCollider2D>().size;
        SetupBoard(offset);

        backgroundMusic.Play();
    }


    private void SetupBoard(Vector2 offset)
    {
        candies = new GameObject[xSize, ySize];

        float startX = this.transform.position.x;
        float startY = this.transform.position.y;

        int idx = -1;

        for (int x = 0; x < xSize; x++) 
        {
            for (int y = 0; y < ySize; y++) 
            {
                GameObject newCandy = Instantiate(
                    currentCandy,
                    new Vector3(
                        startX + (offset.x * x),
                        startY + (offset.y * y),
                        0
                    ),
                    currentCandy.transform.rotation
                );
                
                do 
                {
                    idx = Random.Range(0, prefabs.Count);
                } while (NeighborIsSameCandy(x, y));
                newCandy.GetComponent<SpriteRenderer>().sprite = prefabs[idx];
                newCandy.GetComponent<Candy>().id = idx;
                newCandy.name = string.Format("Candy[{0}][{1}]", x, y);
                candies[x, y] = newCandy;
            }
        }

        bool NeighborIsSameCandy(int x, int y) 
        {
            return (x > 0 && idx == candies[x-1, y].GetComponent<Candy>().id) 
                    || (y > 0 && idx == candies[x, y - 1].GetComponent<Candy>().id);
        }
    }


    void Update()
    {

    }

    
   public IEnumerator FindNullCandies()
   {
       for (int x = 0; x < xSize; x ++) 
       {
           for (int y = 0; y < ySize; y++)
           {
               SpriteRenderer renderer = candies[x, y].GetComponent<SpriteRenderer>();
               if (renderer.sprite == null)
               {
                   yield return StartCoroutine(MakeCandiesFall(x, y));
                   break;
               }
           }
       }

       for (int x = 0; x < xSize; x++)
       {
           for (int y = 0; y < ySize; y++)
           {
               candies[x, y].GetComponent<Candy>().FindAllMatches();
           }
       }
   } 

   private IEnumerator MakeCandiesFall(int x, int yStart, float shiftDelay = 0.05f)
   {
       isShifting = true;
       List<SpriteRenderer> renderers = new List<SpriteRenderer>();
       int nullCandies = 0;
       for (int y = yStart; y < ySize; y ++)
       {
            SpriteRenderer renderer = candies[x, y].GetComponent<SpriteRenderer>();
            if (renderer.sprite == null)
            {
                nullCandies ++;
            }

            renderers.Add(renderer);
       }

       for (int i = 0; i < nullCandies; i++)
       {
           GUIManager.sharedInstance.Score += 10;
           yield return new WaitForSeconds(shiftDelay);
           for ( int j = 0; j < renderers.Count - 1; j ++)
           {
               renderers[j].sprite = renderers[j + 1].sprite;
               renderers[j + 1].sprite = this.GetNewCandy(x, ySize - 1); 
           }
       }
       isShifting = false;
   }


   private Sprite GetNewCandy(int x, int y)
   {
       List<Sprite> possibleCandies = new List<Sprite>();
       possibleCandies.AddRange(prefabs);
       if (x > 0)
       {
           possibleCandies.Remove(
               candies[x - 1, y].GetComponent<SpriteRenderer>().sprite
           );
       }
       if (x < xSize - 1)
       {
           possibleCandies.Remove(
               candies[x + 1, y].GetComponent<SpriteRenderer>().sprite
           );
       }
       if (y > 0)
       {
           possibleCandies.Remove(
               candies[x, y - 1].GetComponent<SpriteRenderer>().sprite
           );
       }

       return possibleCandies[Random.Range(0, possibleCandies.Count)];
   }
}
