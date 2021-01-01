using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour
{

    public static GUIManager sharedInstance;

    public Text movesText;
    public Text scoreText;
    public int Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
            scoreText.text = "Score: " + score;
        }
    }
    
    public int MovesCounter
    {
        get
        {
            return movesCounter;
        }

        set
        {
            movesCounter = value;
            movesText.text = "Moves: " + movesCounter;

            if (movesCounter < 0) 
            {
                StartCoroutine(this.GameOver());
            }
        }
    }

    private int movesCounter;
    private int score;

    void Start()
    {
        Score = 0;
        MovesCounter = 30;    
    }

    private void Awake() 
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitUntil(() => !BoardManager.sharedInstance.isShifting);

        SceneManager.LoadScene("GameOverScene");

    }
}
