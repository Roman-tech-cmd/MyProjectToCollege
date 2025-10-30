using UnityEngine;
using UnityEngine.UI;

public class ButtonTakerQuestion : MonoBehaviour
{
    public int idButton;
    public TileManager tileManager;

    private void OnEnable()
    {
        tileManager = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();
        // Подписываемся на событие нажатия
        GetComponent<Button>().onClick.AddListener(OnAnswerSelected);
    }

    private void OnDisable()
    {
        // Отписываемся от события
        GetComponent<Button>().onClick.RemoveListener(OnAnswerSelected);
    }

    private void OnAnswerSelected()
    {
        if (tileManager != null && tileManager.SelectedTiles != null)
        {
            tileManager.CheckAnswer(idButton);
        }
    }

}
