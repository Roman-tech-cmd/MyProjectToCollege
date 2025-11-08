using UnityEngine;
using UnityEngine.UI;

public class ButtonTakerQuestion : MonoBehaviour
{
    public int idButton;
    public TileManager tileManager;

    private void OnEnable()
    {
        tileManager = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();
        GetComponent<Button>().onClick.AddListener(OnAnswerSelected);
    }

    private void OnDisable()
    {
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
