using UnityEngine;
using UnityEngine.SceneManagement;

public class Restarter : MonoBehaviour
{
    private static Restarter _instance;

    public static Restarter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<Restarter>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(Restarter).Name + " (Singleton)";
                    _instance = obj.AddComponent<Restarter>();
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = GetComponent<Restarter>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SceneManager.LoadScene(0);
        }
    }
}
