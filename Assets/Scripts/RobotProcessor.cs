using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class RobotProcessor : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    [SerializeField] private GameObject _Begin;
    [SerializeField] private GameObject _ErrorPanel;
    [SerializeField] private Text _ErrorText;
    [SerializeField] private Button _ButtonPlay;

    private int _LenghtProgram;


    private struct Command
    {
        public string condition;
        public int TrueTransition, FalseTransition, RegiserMotion;
        public bool RegisterPlay;
    }
    private Command[] _Program;

    public struct RegistersStruct
    {
        public bool Play, Collision, OnLift;
        public int Motion, CommandCounter;
    }
    public RegistersStruct Registers;

    void Start()
    {
        _ErrorPanel.SetActive(false);
        Registers.CommandCounter = 0;
        _ButtonPlay.onClick.AddListener(CompilAndRun);
    }

    void CompilAndRun()
    {
        if (BuildCode())
        {
            Registers.CommandCounter = 0;
            Registers.Play = true;
            gm.EditorView = false;
        }
    }

    private void Update()
    {
        ProcessorStep();
    }

    private void ProcessorStep()
    {
        if (Registers.Play && Registers.CommandCounter!=_LenghtProgram-1)
        {
            int index = Registers.CommandCounter;
            //Debug.Log("StartStep");
            //Debug.Log("Registers: play = " + Registers.Play.ToString() + " collision = " + Registers.Collision.ToString() + " onLift = " + Registers.OnLift.ToString() + " Motion = " + Registers.Motion.ToString() + " CounterComand = " + Registers.CommandCounter.ToString());
            //Debug.Log("Comand: condition = " + _Program[index].condition + " trueTrasition = " + _Program[index].TrueTransition.ToString() + " falseTrasition = " + _Program[index].FalseTransition.ToString() + " registerMotion = " + _Program[index].RegiserMotion.ToString() + " registerPlay = " + _Program[index].RegisterPlay.ToString());
            if (ConditionCheck(_Program[index].condition))
            {
                if (_Program[index].RegiserMotion != -8)
                    Registers.Motion = _Program[index].RegiserMotion;
                Registers.Play = _Program[index].RegisterPlay;
                Registers.CommandCounter = _Program[index].TrueTransition;
            }
            else
                Registers.CommandCounter = _Program[index].FalseTransition;
        } else if (Registers.CommandCounter == _LenghtProgram - 1)
        {
            Registers.Motion = 0;
            Registers.Play = false;
        }
    }

    private bool ConditionCheck(string condition)
    {
        if (condition == "")
            return true;
        else
        {
            bool currentValue, value, not, and, or;
            or = true;
            and = false;
            not = false;
            value = false;
            currentValue = false;

            for (int i = 0; i < condition.Length; i++)
            {
                if (condition[i] == 'T')
                {
                    value = true;
                    if (not)
                        value = !value;
                    currentValue = (currentValue & and & value) | (or & value);
                    or = false;
                    and = false;
                    not = false;
                    value = false;
                }
                else if (condition[i] == 'C')
                {
                    value = Registers.Collision;
                    if (not)
                        value = !value;
                    currentValue = (currentValue & and & value) | (or & value);
                    or = false;
                    and = false;
                    not = false;
                    value = false;
                }
                else if (condition[i] == 'L')
                {
                    value = Registers.OnLift;
                    if (not)
                        value = !value;
                    currentValue = (currentValue & and & value) | (or & value);
                    or = false;
                    and = false;
                    not = false;
                    value = false;
                }
                else if (condition[i] == 'A')
                    and = true;
                else if (condition[i] == 'O')
                    or = true;
                else if (condition[i] == 'N')
                    not = true;
            }
            return currentValue;
        }
    }

    private bool BuildCode()
    {
        string error = "";
        _ErrorPanel.SetActive(false);
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
            else if (tileScript.gameObject.tag == "MoveRight")
            {
                _Program[index].TrueTransition = index + 1;
                _Program[index].RegiserMotion = 1;
            }
            else if (tileScript.gameObject.tag == "MoveLeft")
            {
                _Program[index].TrueTransition = index + 1;
                _Program[index].RegiserMotion = -1;
            }
            else if (tileScript.gameObject.tag == "Jump to right")
            {
                _Program[index].TrueTransition = index + 1;
                _Program[index].RegiserMotion = 2;
                _Program[index].RegisterPlay = false;
            }
            else if (tileScript.gameObject.tag == "Jump to left")
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
            else if (tileScript.gameObject.tag == "Begin")
            {
                _Program[0].TrueTransition = 1;
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
        ArrayList boolean = new ArrayList { "True", "Collision","On lift","IsABox"};
        ArrayList operators = new ArrayList { "Or", "And"};
        while (tile)
        {
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
                if (tile.RightAffiliation)
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
                if (tile.RightAffiliation)
                {
                    if (!operators.Contains(tile.RightAffiliation))
                    {
                        error = "Ошибка! Отсутствует оператор";
                        return false;
                    }
                }
                condition += 'L';
            }
            else if (tile.gameObject.tag == "IsABox")
            {
                if (tile.RightAffiliation)
                {
                    if (!operators.Contains(tile.RightAffiliation))
                    {
                        error = "Ошибка! Отсутствует оператор";
                        return false;
                    }
                }
                condition += 'T'; //Заменить когда будут коробки
            }
            if (tile.RightAffiliation)
                tile = tile.RightAffiliation.GetComponent<Tile>();
            else
                tile = null;
        }
        return true;
    }
}
