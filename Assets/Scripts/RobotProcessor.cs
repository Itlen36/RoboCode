﻿using System.Collections;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotProcessor : MonoBehaviour
{
    [SerializeField] private GameObject _Begin;
    [SerializeField] private GameObject _ErrorPanel;
    [SerializeField] private Text _ErrorText;

    private int _LenghtProgram;
    public bool compil = false;
    private struct Command
    {
        public string condition;
        public int TrueTransition, FalseTransition, RegiserMotion;
        public bool RegisterPlay;
    }
    private Command[] _Program;

    private struct ProcessorRegisters
    {
        public bool Play, Collision, OnLift;
        public int Motion, CommandCounter;
    }
    private ProcessorRegisters _ProcessorRegisters;

    void Start()
    {
        _ErrorPanel.SetActive(false);
    }

    void Update()
    {
        if (compil)
        {
            BuildCode();
            compil = false;
        }
    }

    private void ProcessorStep()
    {
        if (_ProcessorRegisters.Play)
        {
            int index = _ProcessorRegisters.CommandCounter;
            if (ConditionCheck(_Program[index].condition))
            {
                if (_Program[index].RegiserMotion != -8)
                    _ProcessorRegisters.Motion = _Program[index].RegiserMotion;
                _ProcessorRegisters.Play = _Program[index].RegisterPlay;
                _ProcessorRegisters.CommandCounter = _Program[index].TrueTransition;
            }
            else
                _ProcessorRegisters.CommandCounter = _Program[index].FalseTransition;
        }
    }

    private bool ConditionCheck(string condition)
    {
        bool currentValue
        return true;
    }

    private bool BuildCode()
    {
        string error = "";
        if ((_LenghtProgram = getLenghtProgram(ref error)) == 0)
        {
            _ErrorText.text = error;
            _ErrorPanel.SetActive(true);
            return false;
        }
        _Program = new Command[_LenghtProgram];
        if (!Compilation(ref error))
        {
            _ErrorText.text = error;
            _ErrorPanel.SetActive(true);
            return false;
        }
        Debug.Log("compiled");
        return true;
    }

    private int getLenghtProgram(ref string error)
    {
        Tile tileScript = _Begin.GetComponent<Tile>();
        int index = 0;
        while (tileScript.gameObject.tag != "End begin")
        {
            index++;
            if (tileScript.BottomAffiliation)
                tileScript = tileScript.BottomAffiliation.GetComponent<Tile>();
            else
            {
                error = "Ошибка! Отсутсвует конец программы!";
                return 0;
            }
        }
        return index + 1;
    }


    private bool Compilation(ref string error)
    {
        Stack<int> StartIf, StartWhile;
        StartIf = new Stack<int>();
        StartWhile = new Stack<int>();
        Tile tileScript = _Begin.GetComponent<Tile>();
        for (int index = 0; index <_LenghtProgram-1; index++)
        {
            string cond = "";
            if (!getCondition(tileScript, ref cond, ref error))
                return false;
            _Program[index].condition = cond;
            _Program[index].RegisterPlay = true;
            _Program[index].RegiserMotion = -8;
            if (tileScript.gameObject.tag=="While")
            {
                StartWhile.Push(index);
                _Program[index].TrueTransition = index + 1;
            }
            else if (tileScript.gameObject.tag =="End while")
            {
                if (StartWhile.Count == 0)
                {
                    error = "Ошибка! Встречен конец цикла, но начало отсутствует";
                    return false;
                }
                else
                {
                    _Program[index].TrueTransition = StartWhile.Peek();
                    _Program[StartWhile.Pop()].FalseTransition = index + 1;
                }
            }
            else if (tileScript.gameObject.tag == "If")
            {
                StartIf.Push(index);
                _Program[index].TrueTransition = index + 1;
            }
            else if (tileScript.gameObject.tag == "End if")
            {
                if (StartIf.Count == 0)
                {
                    error = "Ошибка! Встречен конец условия, но начало отсутствует";
                    return false;
                }
                else
                    _Program[StartIf.Pop()].FalseTransition = index + 1;
                _Program[index].TrueTransition = index + 1;
            }
            else if (tileScript.gameObject.tag == "Move right")
            {
                _Program[index].TrueTransition = index + 1;
                _Program[index].RegiserMotion = 1;
            }
            else if (tileScript.gameObject.tag == "Move left")
            {
                _Program[index].TrueTransition = index + 1;
                _Program[index].RegiserMotion = -1;
            }
            else if (tileScript.gameObject.tag == "Jump right")
            {
                _Program[index].TrueTransition = index + 1;
                _Program[index].RegiserMotion = 2;
                _Program[index].RegisterPlay = false;
            }
            else if (tileScript.gameObject.tag == "Jump left")
            {
                _Program[index].TrueTransition = index + 1;
                _Program[index].RegiserMotion = -2;
                _Program[index].RegisterPlay = false;
            }
            else if (tileScript.gameObject.tag == "Rise Up")
            {
                _Program[index].TrueTransition = index + 1;
                _Program[index].RegiserMotion = 3;
                _Program[index].RegisterPlay = false;
            }
            else if (tileScript.gameObject.tag == "Come Down")
            {
                _Program[index].TrueTransition = index + 1;
                _Program[index].RegiserMotion = -3;
                _Program[index].RegisterPlay = false;
            }

            tileScript = tileScript.BottomAffiliation.GetComponent<Tile>();
        }
        if (StartIf.Count != 0)
        {
            error = "Ошибка!Отсутствует конец условия";
            return false;
        }
        if (StartWhile.Count != 0)
        {
            error = "Ошибка! Отсутствует конец цикла";
            return false;
        }
        return true;
    }

    private bool getCondition(Tile tile, ref string condition, ref string error)
    {
        ArrayList boolean = new ArrayList { "True", "Collision","On lift"};
        ArrayList operators = new ArrayList { "Or", "And"};
        while (tile)
        {
            Debug.Log(tile.gameObject.tag);
            if (tile.gameObject.tag == "And")
            {
                if (!tile.RightAffiliation)
                {
                    error = "Ошибка! Отсутствует операнд";
                    return false;
                }
                if (!boolean.Contains(tile.Parent.gameObject.tag) || (!boolean.Contains(tile.RightAffiliation.gameObject.tag) && tile.RightAffiliation.gameObject.tag != "Not"))
                {
                    error = "Ошибка! Операнд имет неверный тип";
                    return false;
                }
                condition += 'A';
            }
            else if (tile.gameObject.tag == "Or")
            {
                if (!tile.RightAffiliation)
                {
                    error = "Ошибка! Отсутствует операнд";
                    return false;
                }
                if (!boolean.Contains(tile.Parent.gameObject.tag) || (!boolean.Contains(tile.RightAffiliation.gameObject.tag) && tile.RightAffiliation.gameObject.tag != "Not"))
                {
                    error = "Ошибка! Операнд имет неверный тип";
                    return false;
                }
                condition += 'O';
            }
            else if (tile.gameObject.tag == "Not")
            {
                if (!tile.RightAffiliation)
                {
                    error = "Ошибка! Отсутствует операнд";
                    return false;
                }
                if (!boolean.Contains(tile.RightAffiliation.gameObject.tag))
                {
                    error = "Ошибка! Операнд имет неверный тип";
                    return false;
                }
                condition += 'N';
            }
            else if (tile.gameObject.tag == "True")
            {
                if (!tile.RightAffiliation)
                {
                    if (!operators.Contains(tile.RightAffiliation))
                    {
                        error = "Ошибка! Отсутствует оператор";
                        return false;
                    }
                }
                condition += 'T';
            }
            else if (tile.gameObject.tag == "Collision")
            {
                if (tile.RightAffiliation)
                { 
                    if (!operators.Contains(tile.RightAffiliation))
                    {
                        error = "Ошибка! Отсутствует оператор";
                        return false;
                    }
                }
                condition += 'C';
            }
            else if (tile.gameObject.tag == "On lift")
            {
                if (!tile.RightAffiliation)
                {
                    if (!operators.Contains(tile.RightAffiliation))
                    {
                        error = "Ошибка! Отсутствует оператор";
                        return false;
                    }
                }
                condition += 'L';
            }
            if (tile.RightAffiliation)
                tile = tile.RightAffiliation.GetComponent<Tile>();
            else
                tile = null;
        }
        return true;
    }
}