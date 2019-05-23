using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _BeginTile;
    [SerializeField] private GameObject _WhilePrefab;
    [SerializeField] private GameObject _EndWhilePrefab;
    [SerializeField] private GameObject _CodeEditor;
    public List<GameObject> Tiles;
    private bool _EditorView;

    void Start()
    {
        Tiles = new List<GameObject>();
        Tiles.Add(_BeginTile);
        _EditorView = true;
    }

    
    void Update()
    {
        if (_EditorView)
        {
            _CodeEditor.SetActive(true);
            if (Input.GetMouseButtonDown(0))
            {
                int layer_mask = LayerMask.GetMask("Default");
                RaycastHit2D hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider)
                {
                    Debug.Log("Ray");
                    if (hit.collider.gameObject.tag == "While")
                    {
                        GameObject tile = Instantiate(_WhilePrefab, this.transform.position, Quaternion.identity);
                        tile.transform.SetParent(_CodeEditor.transform,false);
                        Tile TileScript = tile.GetComponent<Tile>();
                        TileScript.gm = this;
                    }
                }
            }
        } else
        {
            _CodeEditor.SetActive(false);
        }
    }
}
