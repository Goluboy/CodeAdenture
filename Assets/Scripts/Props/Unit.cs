using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour, IResettable
{
    public readonly Dictionary<int, bool> Dict = new();
    public Vector3 OriginalPosition { get; private set; }
    private bool _isMoving;
    private int index = 0;
    private float _speed;
    private float _timeToMove = 0.7f;
    private Vector3 _offset;
    private Box _carryingBox = null;
    private BlockType[] _solidBlocks = new BlockType[] { BlockType.wall, BlockType.unit, BlockType.stack, BlockType.box, BlockType.door};
    private Animator _animator;

    public bool IsEndExecute {  get; set; }


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        OriginalPosition = transform.position;
        Console.Units.Add(this);
    }

    public void ExecuteReturn(ReturnCommand returnCommand)
    {
        index = IndexOfCommand(returnCommand.Waypoint.GetComponent<Waypoint>());
        var keys = Dict.Keys.ToArray();
        foreach (var index in keys)
        {
            Dict[index] = false;
        }
    }
    public void ExecuteUse(UseCommand useCommand)
    {
        RaycastHit2D rayCast = Physics2D.RaycastAll(transform.position, useCommand.OffsetDirection, 1)
            .Where(x => x.collider.TryGetComponent(out Lever l))
            .FirstOrDefault();
        if (rayCast == default)
        {
            return;
        }
        if (rayCast.collider.TryGetComponent(out Lever lever))
            lever = rayCast == default ? default : rayCast.collider.GetComponent<Lever>();
        lever.Switch();
    }

    public void ExecuteMove(MoveCommand moveCommand)
    {
        _offset = moveCommand.OffsetDirection;
        _animator.SetFloat("MoveX", _offset.x);
        _animator.SetFloat("MoveY", _offset.y);
        _speed = Mathf.Max(1, (Mathf.Abs(_offset.x) + Mathf.Abs(_offset.y)) / Mathf.Sqrt(2)) / _timeToMove;
        var rayCasts = Physics2D.RaycastAll(transform.position, _offset, _offset.magnitude);
        foreach (var rayCast in rayCasts)
        {
            if (rayCast.collider.TryGetComponent(out Block block) 
                && (block.BlockType == BlockType.wall 
                || _solidBlocks.Contains(block.BlockType) && gameObject != rayCast.collider.gameObject))
            {
                return;
            }
        }
        _isMoving = true;
    }

    public void ExecuteIf(IfCommand ifCommand)
    {
        var rayCasts = Physics2D.RaycastAll(transform.position, ifCommand.OffsetDirection, 1);

        if (ifCommand.BlockType == BlockType.Greater || ifCommand.BlockType == BlockType.Less)
        {
            var otherBox = rayCasts
                    .Select(x => x.collider)
                    .Where(x => x.TryGetComponent(out Unit _))
                    .Select(x => x.GetComponent<Unit>()._carryingBox)
                    .FirstOrDefault();
            if (otherBox == null) 
            {
                otherBox = rayCasts
                    .Select(x => x.collider)
                    .Where(x => x.TryGetComponent(out Box _))
                    .Select(x => x.GetComponent<Box>())
                    .FirstOrDefault();
            }

            if (_carryingBox == null || otherBox == null)
                Dict[IndexOfCommand(ifCommand)] = false;
            else
            {
                if (ifCommand.BlockType == BlockType.Greater)
                    Dict[IndexOfCommand(ifCommand)] = ifCommand.IsTrue == (_carryingBox.Value < otherBox.Value);
                else if (ifCommand.BlockType == BlockType.Less)
                    Dict[IndexOfCommand(ifCommand)] = ifCommand.IsTrue == (_carryingBox.Value > otherBox.Value);
            }
        }
        else
        {
            foreach (var rayCast in rayCasts)
            {
                if (ifCommand.BlockType == BlockType.unit)
                {
                    if (rayCast.collider.TryGetComponent(out Unit unit) && unit != this)
                    {
                        Dict[IndexOfCommand(ifCommand)] = true;
                        break;
                    }
                    continue;
                }

                if (rayCast.collider.TryGetComponent(out Block block) 
                    && ifCommand.BlockType == block.BlockType)
                {
                    Dict[IndexOfCommand(ifCommand)] = true;
                    break;
                }
            }
            Dict[IndexOfCommand(ifCommand)] = Dict[IndexOfCommand(ifCommand)] == ifCommand.IsTrue;
        }
    }

    public void ExecutePickUp(PickUpCommand TakeCommand)
    {
        if (_carryingBox != null)
            return;
        if (Physics2D.RaycastAll(transform.position, TakeCommand.OffsetDirection, 1).Any(x => x.collider.TryGetComponent(out _carryingBox)))
        {
            PickUpBox(_carryingBox);
            _carryingBox.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void ExecuteDrop(DropCommand dropCommand)
    {
        if (_carryingBox == null)
            return;
        _offset = dropCommand.OffsetDirection;
        var rayCasts = Physics2D.RaycastAll(transform.position, _offset, _offset.magnitude);
        foreach (var rayCast in rayCasts)
        {
            if (rayCast.collider.TryGetComponent(out Stack stack))
            {
                Stack.Add(_carryingBox);
                
                _carryingBox.GetComponent<BoxCollider2D>().enabled = true;
                _carryingBox.IsCarrying = false;
                _carryingBox.gameObject.SetActive(false);
                _carryingBox = null;
                return;
            }

            if (rayCast.collider.TryGetComponent(out Block block)
                && (block.BlockType == BlockType.wall
                || block.BlockType == BlockType.unit && gameObject != rayCast.collider.gameObject))
            {
                return;
            }
        }
        _carryingBox.GetComponent<BoxCollider2D>().enabled = true;
        _carryingBox.IsCarrying = false;
        _carryingBox.transform.SetParent(null);
        _carryingBox.transform.position = transform.position + _offset + new Vector3(-.44f, .37f);
        _carryingBox = null;
    }

    public void Execute(List<CommandLine> commandLines)
    {
        foreach (var ifCommand in Console.IfCommands)
        {
            Dict.Add(IndexOfCommand(ifCommand), false);
        }

        StartCoroutine(ExecuteEnumerator(commandLines));
    }

    private void PickUpBox(Box box)
    {
        box.IsCarrying = true;
        _carryingBox = box;
        _carryingBox.transform.SetParent(transform);
        _carryingBox.transform.localPosition = new Vector3(-.45f, 1,0);
        _carryingBox.GetComponent<SpriteRenderer>().sortingOrder = 100;
    }

    private IEnumerator ExecuteEnumerator(List<CommandLine> s)
    {
        var commandLines = s.ToArray();
        var falseLvl = float.MaxValue;
        var levelEnder = FindObjectsByType(typeof(LevelEnder), FindObjectsSortMode.None).First() as LevelEnder;
        for (; index < commandLines.Length; index++)
        {
            Debug.Log($"{gameObject.name} : {index}");
            if (levelEnder.IsLevelEnded())
            {
                yield break;
            }

            if (commandLines[index].CommandLevel > falseLvl)
            {
                continue;
            }

            commandLines[index].Execute(this);

            if (commandLines[index] is IfCommand && Dict.ContainsKey(index) &&!Dict[index])
            {
                falseLvl = commandLines[index].CommandLevel;
            }

            if (commandLines[index] is IfCommand && Dict[index])
            {
                falseLvl = float.MaxValue;
            }

            foreach (var a in Dict)
            {
                Debug.Log($"{gameObject.name}, {a.Key} : {a.Value}");
            }

            if (levelEnder.IsLevelEnded())
            {
                yield break;
            }

            if (commandLines[index] is Waypoint
                || commandLines[index] is IfCommand
                || commandLines[index] is ReturnCommand)
            {
                yield return null;
            }
            else 
                yield return new WaitForSeconds(1.2f);
        }
        index = 0;
        IsEndExecute = true;
    }

    private void Update()
    {
        if (_isMoving)
        {
            _timeToMove -= Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, transform.position + _offset, _speed * Time.deltaTime);

            if (_timeToMove < 0)
            {
                _isMoving = false;
                _timeToMove = 0.7f;
                transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
                _animator.SetFloat("MoveX", 0);
                _animator.SetFloat("MoveY", 0);
            }
        }
        else
        {
            _animator.SetFloat("MoveX", 0);
            _animator.SetFloat("MoveY", 0);
        }
    }

    private int IndexOfCommand(CommandLine commandLine)
    {
        return Array.IndexOf(Console.CommandLines.ToArray(), commandLine);
    }

    public void StopExecute()
    {
        StopAllCoroutines();
        OnReset();
    }

    public void OnReset()
    {
        StopAllCoroutines();
        index = 0;
        IsEndExecute = false;
        _isMoving = false;
        _carryingBox = null;
        Dict.Clear();
        transform.position = OriginalPosition;
        _animator.SetFloat("MoveX", 0);
        _animator.SetFloat("MoveY", 0);
    }
}
