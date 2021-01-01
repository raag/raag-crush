using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public Text scoreText;

    void Start()
    {
        scoreText.text = GUIManager.sharedInstance.Score.ToString();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("MainScene");
    }
}
