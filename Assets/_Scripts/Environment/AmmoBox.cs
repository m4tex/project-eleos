using System;
using _Scripts.Interfaces;
using _Scripts.Utilities;
using _Scripts.Weapons;
using UnityEngine;
using UnityEngine.Events;

public class AmmoBox : MonoBehaviour, IInteractable
{
    public UnityEvent Interaction { get; set; } = new();
    public string IntPrompt { get; set; }
    public ConditionFunction Condition { get; set; }

    public enum AmmoBoxType
    {
        Small,
        Big
    };

    public AmmoBoxType type;
    private bool _available = true;
    private Material _mat;
    private Color _initialColor;
    private Color _fadedColor;
    private void Start()
    {
        _mat = GetComponent<MeshRenderer>().material;
        _initialColor = _mat.color;
        _fadedColor = _initialColor.Saturation(.3f);

        IntPrompt = "Refill";
        Condition = () => _available;
        Interaction.AddListener(() =>
        {
            switch (type)
            {
                case AmmoBoxType.Small:
                    LoadoutManager.GetCurrentWeapon().SmallRefill(4);
                    break;
                case AmmoBoxType.Big:
                    LoadoutManager.RefillAllWeapons();
                    break;
                default:
                    throw new Exception("Undefined ammo box type encountered");
            }

            _available = false;
            _mat.MaterialFade(_fadedColor, 0.25f, this);
        });
    }

    public void Refresh()
    {
        _available = true;
        _mat.MaterialFade(_initialColor, 0.25f, this);
    }
}
