using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject _Sprite;
    public bool RightAffiliationEnabled, LeftAffiliationEnabled, BottomAffiliationEnabled, TopAffiliationEnabled, MovementEnabled, Established;
    public GameObject RightAffiliation, BottomAffiliation, Parent;
    public GameManager gm;
    public Vector3 RightPort, BottomPort;
    public float width, height;
    private float _TabOffset, _PortOffset, _Offset, _BondingDistance;
    private string _ConnectionSideToParent;


    private void Awake()
    {
        _Offset = 0.05f;
        _BondingDistance = 0.5f;
        this._ConnectionSideToParent = "Null";
        this.width = _Sprite.GetComponent<SpriteRenderer>().size.x;
        this.height = _Sprite.GetComponent<SpriteRenderer>().size.y;
        if (this.tag == "Begin")
        {
            this._TabOffset = 0f;
            this._PortOffset = 0.4f;
            this.RightAffiliationEnabled = false;
            this.BottomAffiliationEnabled = true;
            this.MovementEnabled = false;
            this.Established = true;
            this.BottomPort = new Vector3(this.transform.position.x - (float)0.5f * this.width + _PortOffset, this.transform.position.y - (float)0.5f * this.height - _Offset, this.transform.position.z);
        }
        else if (this.tag == "While")
        {
            this._TabOffset = 0f;
            this._PortOffset = 0.4f;
            this.RightAffiliationEnabled = true;
            this.BottomAffiliationEnabled = true;
            this.LeftAffiliationEnabled = false;
            this.TopAffiliationEnabled = true;
            this.MovementEnabled = true;
            this.Established = false;
        }
        else if (this.tag == "End while")
        {
            this._TabOffset = -0.4f;
            this.RightAffiliationEnabled = false;
            this.BottomAffiliationEnabled = true;
            this.LeftAffiliationEnabled = false;
            this.TopAffiliationEnabled = true;
            this.MovementEnabled = true;
            this.Established = false;
        }
    }


    void Update()
    {
        if (!Established)
        {
            if (this.MovementEnabled)
            {
                if (this.transform.position != Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)))
                {
                    this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
                    foreach (GameObject OtherTile in gm.Tiles)
                    {
                        Tile OtherTileScript = OtherTile.GetComponent<Tile>();
                        if (OtherTileScript.BottomAffiliationEnabled && this.TopAffiliationEnabled && Vector3.Distance(new Vector3(this.transform.position.x - 0.5f * this.width, this.transform.position.y + 0.5f * this.height, this.transform.position.z), OtherTileScript.BottomPort) <= _BondingDistance)
                        {
                            this.transform.position = new Vector3(OtherTileScript.BottomPort.x + 0.5f * this.width + this._TabOffset, OtherTileScript.BottomPort.y - 0.5f * this.height, OtherTileScript.BottomPort.z);
                            this.MovementEnabled = false;
                            this.Parent = OtherTile;
                            Color CurrentColor = this._Sprite.GetComponent<SpriteRenderer>().color;
                            this._Sprite.GetComponent<SpriteRenderer>().color = new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, 1f);
                            this._ConnectionSideToParent = "Top";
                        } else if (OtherTileScript.RightAffiliationEnabled && this.LeftAffiliationEnabled && Vector3.Distance(new Vector3(this.transform.position.x - 0.5f * this.width, this.transform.position.y, this.transform.position.z), OtherTileScript.RightPort) <= _BondingDistance)
                        {
                            this.transform.position = new Vector3(OtherTileScript.RightPort.x + 0.5f * this.width, OtherTileScript.RightPort.y, OtherTileScript.RightPort.z);
                            this.MovementEnabled = false;
                            this.Parent = OtherTile;
                            Color CurrentColor = this._Sprite.GetComponent<SpriteRenderer>().color;
                            this._Sprite.GetComponent<SpriteRenderer>().color = new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, 1f);
                            this._ConnectionSideToParent = "Left";
                        }
                    }
                }
            }
            else if (Vector3.Distance(this.transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10))) >= 1.8f * _BondingDistance)
            {
                Color CurrentColor = this._Sprite.GetComponent<SpriteRenderer>().color;
                this._Sprite.GetComponent<SpriteRenderer>().color = new Color(CurrentColor.r, CurrentColor.g, CurrentColor.b, 0.5f);
                this.Parent = null;
                this.MovementEnabled = true;
                this._ConnectionSideToParent = "Null";
            }
            if (Input.GetMouseButtonUp(0))
            {
                Established = true;
                if (!Parent)
                    Destroy(this.gameObject);
                else
                {
                    if (_ConnectionSideToParent == "Top")
                        Parent.GetComponent<Tile>().BottomAffiliationEnabled = false;
                    else if (_ConnectionSideToParent == "Left")
                        Parent.GetComponent<Tile>().RightAffiliationEnabled = false;
                    this.BottomPort = new Vector3(this.transform.position.x - 0.5f * this.width + _PortOffset, this.transform.position.y - 0.5f * this.height - _Offset, this.transform.position.z);
                    this.RightPort = new Vector3(this.transform.position.x + 0.5f * this.width + _Offset, this.transform.position.y, this.transform.position.z);
                    gm.Tiles.Add(this.gameObject);
                }
            }
        }
    }
}
