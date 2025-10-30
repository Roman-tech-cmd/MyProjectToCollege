using UnityEngine;

public class TileBox : MonoBehaviour
{
    [SerializeField] private GameObject[] tiles;
    [SerializeField] private int idBox;
    public int IdBox => idBox;

    public bool IsSolved;

    public bool CheckBoxToSolve()
    {
        int count = 0;
        foreach (GameObject tile in tiles)
        {
            if (tile.GetComponent<TileQuestion>().IsSolved)
            {
                count++;
            }
        }
        if (count == tiles.Length)
        {
            IsSolved = true;
            return true;
        }
        else
        {
            IsSolved = false;
            return false;
        }
    }
    public void SetTileUnselected()
    {
        foreach (var tile in tiles)
        {
            tile.transform.localScale=new Vector3(1,1,1);
        }
    }

}
