using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelManager _LevelManager;
    [SerializeField] private GameObject _Begin;
    [SerializeField] private GameObject _EndBeginPrefab;
    [SerializeField] private GameObject _WhilePrefab;
    [SerializeField] private GameObject _EndWhilePrefab;
    [SerializeField] private GameObject _IfPrefab;
    [SerializeField] private GameObject _EndIfPrefab;
    [SerializeField] private GameObject _OrPrefab;
    [SerializeField] private GameObject _NotPrefab;
    [SerializeField] private GameObject _AndPrefab;
    [SerializeField] private GameObject _CollisionPrefab;
    [SerializeField] private GameObject _OnLiftPrefab;
    [SerializeField] private GameObject _MoveRightPrefab;
    [SerializeField] private GameObject _MoveLeftPrefab;
    [SerializeField] private GameObject _JumpToRightPrefab;
    [SerializeField] private GameObject _JumpToLeftPrefab;
    [SerializeField] private GameObject _RiseUpPrefab;
    [SerializeField] private GameObject _ComeDownPrefab;
    [SerializeField] private GameObject _TruePrefab;
    [SerializeField] private GameObject _IsABoxPrefab;
    [SerializeField] private GameObject _CodeEditor;
    [SerializeField] private GameObject _PanelMenu;
    [SerializeField] private Text _ButtonChageViewText;
    [SerializeField] private GameObject _ButtonClearCode;
    public List<GameObject> Tiles;
    public GameObject Level;
    public bool EditorView;
    public int StarsCount;
    public bool Finished;

    void Start()
    {
        Tiles.Add(_Begin);
        EditorView = false;
    }

    void Update()
    {
        if (Finished)
        {
            _LevelManager.LevelComplite(StarsCount);
            StarsCount = 0;
            Finished = false;
        }
        else if (EditorView)
        {
            _CodeEditor.SetActive(true);
            Level.SetActive(false);
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider)
                {
                    GameObject tile = null;
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (hit.collider.gameObject.tag == "While")
                        tile = Instantiate(_WhilePrefab, position , Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "End while")
                        tile = Instantiate(_EndWhilePrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "End begin")
                        tile = Instantiate(_EndBeginPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "If")
                        tile = Instantiate(_IfPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "End if")
                        tile = Instantiate(_EndIfPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "Not")
                        tile = Instantiate(_NotPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "Or")
                        tile = Instantiate(_OrPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "And")
                        tile = Instantiate(_AndPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "Collision")
                        tile = Instantiate(_CollisionPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "On lift")
                        tile = Instantiate(_OnLiftPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "MoveRight")
                        tile = Instantiate(_MoveRightPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "MoveLeft")
                        tile = Instantiate(_MoveLeftPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "Jump to right")
                        tile = Instantiate(_JumpToRightPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "Jump to left")
                        tile = Instantiate(_JumpToLeftPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "Rise Up")
                        tile = Instantiate(_RiseUpPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "Come Down")
                        tile = Instantiate(_ComeDownPrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "True")
                        tile = Instantiate(_TruePrefab, position, Quaternion.identity);
                    else if (hit.collider.gameObject.tag == "IsABox")
                        tile = Instantiate(_IsABoxPrefab, position, Quaternion.identity);


                    if (tile != null)
                    {
                        tile.transform.SetParent(_CodeEditor.transform, false);
                        Tile TileScript = tile.GetComponent<Tile>();
                        TileScript.gm = this;
                    }
                }
            }
        } else
        {
            _CodeEditor.SetActive(false);
            if (Level)
                Level.SetActive(true);
        }
    }

    public void ChangeView()
    {
        EditorView = !EditorView;
        if (EditorView)
        {
            _ButtonChageViewText.text = "Level view";
            _ButtonClearCode.SetActive(true);
        }
        else
        {
            _ButtonChageViewText.text = "Code editor";
            _ButtonClearCode.SetActive(false);
        }

    }
    public void TilesDestroyer()
    {
        Tile BeginTile = _Begin.GetComponent<Tile>();
        if (BeginTile.BottomAffiliation)
            DestroyTile(BeginTile.BottomAffiliation);
        BeginTile.BottomAffiliationEnabled = true;
        Tiles.Clear();
        Tiles.Add(_Begin);
    }
    private void DestroyTile(GameObject Obj)
    {
        Tile tileScrip = Obj.GetComponent<Tile>();
        if(tileScrip.RightAffiliation)
            DestroyTile(tileScrip.RightAffiliation);
        if (tileScrip.BottomAffiliation)
            DestroyTile(tileScrip.BottomAffiliation);
        Destroy(Obj);
    }

}
