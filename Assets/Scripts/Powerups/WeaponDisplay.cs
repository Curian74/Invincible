using Assets.Scripts.Powerups;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDisplay : MonoBehaviour
{
    [SerializeField] private List<Sprite> weaponSprites;

    private SpriteRenderer spriteRenderer;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = weaponSprites[0];
    }

    public void SetWeapon(int index)
    {
        spriteRenderer.sprite = weaponSprites[index];
    }
}
