using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DragNDrop : MonoBehaviour
{
    static public bool BlockDragging { get; set; } = true;
    private bool _isDragging;
    private Vector3 _originalPosition;
    private int _sortingLayer;
    private bool _isCommandLine;
    private CommandLine _commandLine;
    private CommandLine _clone;
    private bool _isGhostExist;

    private void Start()
    {
        _isCommandLine = TryGetComponent(out _commandLine);
        _sortingLayer = GetComponent<SpriteRenderer>().sortingOrder;
    }

    private void OnMouseDown()
    {
        if (BlockDragging)
            return;
        _isGhostExist = false;
        _isDragging = true;
        _originalPosition = transform.position;
        GetComponent<SpriteRenderer>().sortingOrder = 100;

    }

    private void OnMouseUp()
    {
        if (BlockDragging)
            return;
        _isDragging = false;
        if (TryGetComponent(out Spawnable spawnable))
        {
            Spawner spawner = spawnable.Spawner;
            if (spawner.transform.position == transform.position)
                Destroy(gameObject);

            if (_originalPosition == spawner.transform.position)
            {
                spawner.SpawnNew();
            }
        }

        if (_isCommandLine)
        {
            CommandLineBehavior();
        }
        else
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));

        GetComponent<SpriteRenderer>().sortingOrder = _sortingLayer;

        if (_clone != null)
        {
            Console.Instance.Remove(_clone);
            Destroy(_clone);
            _clone = null;
        }
    }

    private void CommandLineBehavior()
    {
        if (transform.position.x > Console.Instance.transform.position.x + 1e-5)
        {
            if (Array.IndexOf(Console.CommandLines.ToArray(), _clone) < 1)
            {
                Console.Instance.Add(_commandLine);
                _commandLine.OnSetup();
                return;
            }
        }
        else
        {
            Console.Instance.Remove(_commandLine);
            Destroy(gameObject);
            return;
        }
        Console.Instance.Add(_commandLine);
        _commandLine.IfList = _clone.IfList;
        _commandLine.OnSetup();
        transform.position = _clone.transform.position;
    }

    private void GhostCommandLine()
    {
        _isGhostExist = true;
        _clone = Instantiate(_commandLine);
        Color tmp = _clone.GetComponent<SpriteRenderer>().color;
        tmp.a = 0.5f;
        _clone.GetComponent<SpriteRenderer>().color = tmp;
        Console.Instance.Add(_clone);
    }

    float RoundToFraction(float value, float fraction)
    {
        return (float)Math.Round(Math.Round(value / fraction) * fraction, 1, MidpointRounding.AwayFromZero);
    }

    private void Update()
    {
        if (_isDragging)
        {
            if (_isCommandLine)
            {
                var curLvl = RoundToFraction(transform.position.x - Console.Instance.transform.position.x, Console.Instance.CommandLinePadding);
                if (Array.IndexOf(Console.CommandLines.ToArray(), _clone) > 0 && (Console.CommandLines[Array.IndexOf(Console.CommandLines.ToArray(), _clone) - 1] is IfCommand))
                {
                    if (transform.position.x > Console.Instance.transform.position.x + 1e-5
                    && curLvl < 3)
                    {
                        var minLvl = Array.IndexOf(Console.CommandLines.ToArray(), _clone) > 0 ?
                            Console.CommandLines[Array.IndexOf(Console.CommandLines.ToArray(), _clone) - 1].CommandLevel + 1 : 0;

                        _commandLine.CommandLevel = Mathf.Min(RoundToFraction(transform.position.x - Console.Instance.transform.position.x,
                            Console.Instance.CommandLinePadding)
                            / Console.Instance.CommandLinePadding, minLvl);
                        if (_isGhostExist)
                            _clone.CommandLevel = Mathf.Min(_commandLine.CommandLevel, minLvl);
                    }
                }
                else
                {
                    if (transform.position.x > Console.Instance.transform.position.x + 1e-5
                    && curLvl < 3)
                    {
                        var minLvl = Array.IndexOf(Console.CommandLines.ToArray(), _clone) > 0 ?
                            Console.CommandLines[Array.IndexOf(Console.CommandLines.ToArray(), _clone) - 1].CommandLevel : 0;

                        _commandLine.CommandLevel = Mathf.Min(RoundToFraction(transform.position.x - Console.Instance.transform.position.x,
                            Console.Instance.CommandLinePadding)
                            / Console.Instance.CommandLinePadding, minLvl);
                        if (_isGhostExist)
                            _clone.CommandLevel = Mathf.Min(_commandLine.CommandLevel, minLvl);
                    }
                }

                if (Console.CommandLines.Contains(_commandLine)) Console.CommandLines.Remove(_commandLine);
                Vector2 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                transform.Translate(_mousePosition);
                if (!_isGhostExist)
                    GhostCommandLine();
                _clone.transform.position = transform.position;

                Console.Instance.OrderCommandLines();
            }
            else
            {
                Vector2 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                transform.Translate(_mousePosition);
            }
        }
    }

}