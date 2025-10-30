using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(GameManager).Name + " (Singleton)";
                    _instance = obj.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }


    [SerializeField] private GameObject SceneMenu;
    [SerializeField] private GameObject SceneGame;
    [SerializeField] private GameObject SceneEnd;
    [SerializeField] private GameObject AnswerButtons;
    [SerializeField] private GameObject ButtonGoToFinal;

    [SerializeField] private TileManager tileManager;
    void Start()
    {
        Application.targetFrameRate = 120;
        
    }
    public void GoToGame()
    {
        SceneMenu.SetActive(false);
        SceneGame.SetActive(true);
        SceneEnd.SetActive(false);

    }

    public void GoToMainMenu()
    {
        SceneMenu.SetActive(true);
        SceneGame.SetActive(false);
        SceneEnd.SetActive(false);
    }

    public void QuitGame()
    {
        DOTween.KillAll();
        Application.Quit();
    }

    public void RestartGame()
    {
        DOTween.KillAll();
        Restarter.Instance.Restart();    
    }
    
    void Update()
    {
        if (tileManager.IsTileBox1Solved && tileManager.IsTileBox2Solved && tileManager.IsTileBox3Solved)
        {
            AnswerButtons.SetActive(false);
            ButtonGoToFinal.SetActive(true);
            tileManager.TextQuestion.text = "";
            tileManager.NumQuestion.text = "";
            tileManager.BlockMenu.SetActive(false);
            tileManager.BlockMenu2.SetActive(false);
            tileManager.HintMenu.transform.DOLocalMoveX(1230, 0.5f);
        }
    }
    
    public void GoToFinal()
    {
        SceneEnd.SetActive(true);
        SceneMenu.SetActive(false);
        SceneGame.SetActive(false);
    }
}
