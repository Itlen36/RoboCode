using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    [SerializeField] private GameObject _LevelButtons;
    [SerializeField] private Sprite _CloseLevelButton;
    [SerializeField] private Sprite _1StarLevelButton;
    [SerializeField] private Sprite _2StarLevelButton;
    [SerializeField] private Sprite _3StarLevelButton;
    [SerializeField] private Sprite _0StarLevelButton;
    [SerializeField] private GameObject _LevelMenu;
    [SerializeField] private GameObject _Level1;
    [SerializeField] private GameObject _Level2;
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
                    break;
                case 1:
                    _LevelButtons.transform.GetChild(i).GetComponent<Image>().sprite = _1StarLevelButton;
                    break;
                case 2:
                    _LevelButtons.transform.GetChild(i).GetComponent<Image>().sprite = _2StarLevelButton;
                    break;
                case 3:
                    _LevelButtons.transform.GetChild(i).GetComponent<Image>().sprite = _3StarLevelButton;
                    break;
            }
        }
    }

    public void LoadLevel(int number)
    {
        _NubnumberCurrentLevel = number;
        _CurrentLevel =  Instantiate(_Levels[number - 1], Vector3.zero, Quaternion.identity);
        gm.Level = _CurrentLevel;
        _LevelMenu.SetActive(false);
        _CurrentLevel.SetActive(true);
    }
    public void LevelComplite(int stars)
    {
        _LevelProgress[_NubnumberCurrentLevel-1] = stars;
        Destroy(_CurrentLevel);

    }
    void Start()
    {
        _Levels = new GameObject[] { _Level1, _Level2 };
        _LevelProgress = new int[_LevelButtons.transform.childCount];
        for (int i = 1; i < _LevelProgress.Length; i++)
            _LevelProgress[i] = -1;
        _LevelProgress[0] = 0;
        ReloadLevelMenu();
    }
}
