using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour, IResettable
{
    [SerializeField] private LevelEnder _levelEnder;
    [SerializeField] private Button _executeButton;
    public static IfCommand[] IfCommands => CommandLines.Where(x => x is IfCommand).Select(x => x as IfCommand).ToArray();
    public float CommandLineHeight { get; } = .575f;
    public float CommandLinePadding { get; } = .4f;
    public static List<Unit> Units { get; private set; } = new List<Unit>();
    public static List<CommandLine> CommandLines { get; set; } = new();
    public static Console Instance { get; private set; }

    private bool _isReady = true;
    private Box[] _boxes;
    private Lever[] _levers;
    private Door[] _doors;
    private ConditionOfEndOfLevel[] _conditions;
    private Stack[] _stacks;

    public void CloseLevel()
    {
        CommandLines = new();
        Units = new();
        ResetScene();
    }

    private void Awake()
    {
        _boxes = FindObjectsByType(typeof(Box), FindObjectsSortMode.None)
                                                        .Select(x => x as Box).ToArray();
        _levers = FindObjectsByType(typeof(Lever), FindObjectsSortMode.None)
            .Select(x => x.GetComponents<Lever>().FirstOrDefault()).ToArray();

        _doors = FindObjectsByType(typeof(Door), FindObjectsSortMode.None)
            .Select(x => x.GetComponents<Door>().FirstOrDefault()).ToArray();

        _conditions = FindObjectsByType(typeof(ConditionOfEndOfLevel), FindObjectsSortMode.None)
                                                        .Select(x => x as ConditionOfEndOfLevel).ToArray();

        _stacks = FindObjectsByType(typeof(Stack), FindObjectsSortMode.None)
                                                        .Select(x => x as Stack).ToArray();
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        ResetScene();
    }

    public void Execute()
    {
        Units = Units.Where(x => x!=null).ToList();
        if (!_isReady)
            return;
        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        _executeButton.GetComponent<Button>().interactable = false;
        foreach (Unit unit in Units)
        {
            unit.Execute(CommandLines);
        }
    }

    public void StopExecute()
    {
        Array.ForEach(Units.ToArray(), x => x.StopExecute());
        _executeButton.GetComponent<Button>().interactable = true;
        ResetScene();
    }

    public void EndExecute()
    {
        if (_levelEnder.IsLevelEnded())
            return; 

        ResetScene();

        _executeButton.GetComponent<Button>().interactable = true;
    }

    public void ResetScene()
    {
        _isReady = false;
        _executeButton.GetComponent<Button>().interactable = true;
        _levelEnder.Panel.SetActive(false);
        DragNDrop.BlockDragging = false;

        Array.ForEach(Units.ToArray(), x => x.OnReset());

        Array.ForEach(_levers, x => x.OnReset());

        Array.ForEach(_doors, x => x.OnReset());

        Array.ForEach(_conditions, x => x.OnReset());

        Array.ForEach(_stacks, x => x.OnReset());

        Array.ForEach(_boxes, x => x.OnReset());

        Debug.Log("RELOADED----RELOADED----RELOADED----RELOADED----RELOADED----RELOADED");
        _isReady = true;
    }

    public void Add(CommandLine commandLine)
    {
        if (!CommandLines.Contains(commandLine))
            CommandLines.Add(commandLine);

        OrderCommandLines();
    }

    public void Remove(CommandLine commandLine)
    {
        CommandLines.Remove(commandLine);
        Destroy(commandLine.gameObject);

        OrderCommandLines();
    }

    public void OrderCommandLines()
    {
        CommandLines.Sort((CommandLine x, CommandLine y) => -x.transform.position.y.CompareTo(y.transform.position.y));
        for (int i = 0; i < CommandLines.Count; i++)
        {
            CommandLines[i].transform.position = new Vector3(transform.position.x + CommandLines[i].CommandLevel * CommandLinePadding, 
                transform.position.y - i * CommandLineHeight);
            if (i == CommandLines.Count - 1 && CommandLines[i] is Waypoint)
            {
                CommandLines[i].transform.position = new Vector3(transform.position.x + CommandLines[i].CommandLevel * CommandLinePadding,
                transform.position.y - (i - 1) * CommandLineHeight);
            }
        }
    }

    private void Update()
    {
        if (Units.All(x => x.IsEndExecute))
            EndExecute();
    }
}
