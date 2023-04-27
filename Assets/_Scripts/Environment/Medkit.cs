using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Interfaces;
using _Scripts.Player;
using _Scripts.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Medkit : MonoBehaviour, IInteractable
{
    public UnityEvent Interaction { get; set; } = new();
    public string IntPrompt { get; set; }
    public ConditionFunction Condition { get; set; }

    public float refreshTime;
    
    private bool _available = true;
    private Material _mat;
    private Color _init, _faded;
    
    private void Start()
    {
        _mat = GetComponent<MeshRenderer>().material;
        _init = _mat.color;
        _faded = _init.Saturation(.65f);

        IntPrompt = "Use Medkit";
        Condition = () => _available;
        Interaction.AddListener(() =>
        {
            StatsManager.Health = 100;
            _available = false;
            _mat.MaterialFade(_faded, 0.25f, this);
            Invoke(nameof(Refresh), refreshTime);
        });
    }

    public void Refresh()
    {
        _available = true;
        _mat.MaterialFade(_init, 0.25f, this);
    }
}