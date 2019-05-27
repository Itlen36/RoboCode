using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameManager _gm;
    [SerializeField] private RobotProcessor _RobotProcessor;
    [SerializeField] private GameObject _RobotPref;
    [SerializeField] private GameObject _LevelButtons;
    [SerializeField] private Sprite _CloseLevelButton;
    [SerializeField] private Sprite _1StarLevelButton;
    [SerializeField] private Sprite _2StarLevelButton;
    [SerializeField] private Sprite _3StarLevelButton;
    [SerializeField] private Sprite _0StarLevelButton;
    [SerializeField] private GameObject _LevelMenu;
    [SerializeField] private GameObject _Level1;
    [SerializeField] private GameObject _Level2;
    [SerializeField] private GameObject _Level3;
    [SerializeField] private GameObject _Level4;
    private int[] _LevelProgress;
    private GameObject[] _Levels;
    private GameObject _CurrentLevel;
    private int _NubnumberCurrentLevel;


    public void ReloadLevelMenu()
    {
        for (int i = 0; i < _LevelButtons.transform.childCount; i++)
        {
            switch(_LevelProgress[i])
            {
                case -1:
                    _LevelButtons.transform.GetChild(i).GetComponent<Image>().sprite = _CloseLevelButton;
                    _LevelButtons.transform.GetChild(i).GetComponent<Button>().enabled = false;
                    break;
                case 0:
                    _LevelButtons.transform.GetChild(i).GetComponent<Image>().sprite = _0StarLevelButton;
                    _LevelButtons.transform.GetChild(i).GetComponent<Button>().enabled = true;
                    break;
                case 1:
                    _LevelButtons.transform.GetChild(i).GetComponent<Image>().sprite = _1StarLevelButton;
                    _LevelButtons.transform.GetChild(i).GetComponent<Button>().enabled = true;
                    break;
                case 2:
                    _LevelButtons.transform.GetChild(i).GetComponent<Image>().sprite = _2StarLevelButton;
                    _LevelButtons.transform.GetChild(i).GetComponent<Button>().enabled = true;
                    break;
                case 3:
                    _LevelButtons.transform.GetChild(i).GetComponent<Image>().sprite = _3StarLevelButton;
                    _LevelButtons.transform.GetChild(i).GetComponent<Button>().enabled = true;
                    break;
            }
        }
    }

    public void LoadLevel(int number)
    {
        Debug.Log("Level" + number);
        _NubnumberCurrentLevel = number;
        _CurrentLevel =  Instantiate(_Levels[number - 1], Vector3.zero, Quaternion.identity);
        Vector3 StarPosition = _CurrentLevel.transform.GetChild(0).transform.position;
        GameObject Robot = Instantiate(_RobotPref, StarPosition, Quaternion.identity);
        Robot.transform.SetParent(_CurrentLevel.transform, false);
        Robot.GetComponent<Robot>().gm = _gm;
        Robot.GetComponent<Robot>().Processor = _RobotProcessor;
        _gm.Level = _CurrentLevel;
        _LevelMenu.SetActive(false);
        _CurrentLevel.SetActive(true);
    }
    public void LevelComplite(int stars)
    {
        if (_LevelProgress[_NubnumberCurrentLevel - 1] < stars)
            _LevelProgress[_NubnumberCurrentLevel - 1] = stars;
        _LevelProgress[_NubnumberCurrentLevel] = 0;
        Destroy(_CurrentLevel);
        _gm.TilesDestroyer();
        ReloadLevelMenu();
        _LevelMenu.SetActive(true);

    }
    void Start()
    {
        _Levels = new GameObject[] { _Level1, _Level2, _Level3, _Level4 };
        _LevelProgress = new int[_LevelButtons.transform.childCount];
        for (int i = 1; i < _LevelProgress.Length; i++)
            _LevelProgress[i] = -1;
        _LevelProgress[0] = 0;
        ReloadLevelMenu();
    }
}
