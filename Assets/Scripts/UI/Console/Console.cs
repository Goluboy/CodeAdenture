using System;
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

    private Box[] boxes;

    public void CloseLevel()
    {
        CommandLines = new();
        Units = new();
        ResetScene();
    }

    private void Awake()
    {
        boxes = FindObjectsByType(typeof(Box), FindObjectsSortMode.None)
                                                        .Select(x => x as Box).ToArray();
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        ResetScene();
    }

    public void Execute()
    {
        _executeButton.GetComponent<Button>().interactable = false;
        foreach (Unit unit in Units)
        {
            unit.Execute(CommandLines);
        }

        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
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
        _executeButton.GetComponent<Button>().interactable = true;
        _levelEnder.Panel.SetActive(false);
        DragNDrop.BlockDragging = false;

        Array.ForEach(Units.ToArray(), x => x.Dict.Clear());
        Array.ForEach(Units.ToArray(), x => x.transform.position = x.OriginalPosition);
        Array.ForEach(Units.ToArray(), x => x.IsEndExecute = false);

        var levers = FindObjectsByType(typeof(Lever), FindObjectsSortMode.None).Select(x => x.GetComponents<Lever>().FirstOrDefault()).ToArray();
        Array.ForEach(levers, x => x.OnReset());

        var doors = FindObjectsByType(typeof(Door), FindObjectsSortMode.None).Select(x => x.GetComponents<Door>().FirstOrDefault()).ToArray();
        Array.ForEach(doors, x => x.OnReset());

        var conditions = FindObjectsByType(typeof(ConditionOfEndOfLevel), FindObjectsSortMode.None)
                                                        .Select(x => x as ConditionOfEndOfLevel).ToArray();
        Array.ForEach(conditions, x => x.OnReset());

        var stacks = FindObjectsByType(typeof(Stack), FindObjectsSortMode.None)
                                                        .Select(x => x as Stack).ToArray();
        Array.ForEach(stacks, x => x.OnReset());

        Array.ForEach(boxes, x => x.OnReset());
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
