using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTemplate : MonoBehaviour
{
    [SerializeField] private GameObject _WhilePrefab;
    [SerializeField] private GameObject _EndWhilePrefab;
    [SerializeField] private GameManager gm;

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (this.tag == "While")
            {
                GameObject tile = Instantiate(_WhilePrefab, this.transform.position, Quaternion.identity);
                Tile TileScript = tile.GetComponent<Tile>();
                TileScript.gm = gm;
            }
            else if (this.tag == "End while")
            {
                GameObject tile = Instantiate(_WhilePrefab, this.transform.position, Quaternion.identity);
                Tile TileScript = tile.GetComponent<Tile>();
                TileScript.gm = gm;
            }

        }
    }
}
