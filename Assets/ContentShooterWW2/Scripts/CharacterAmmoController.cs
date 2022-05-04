using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterAmmoController : MonoBehaviour
{
    [Header("Патроны")]
    public int RifleBullets = 30;
    public int MaxRifleBullets = 50;
    [Space(20)]
    public int SMGBullets = 142;
    public int MaxSMGBullets = 213;
    [Space(20)]
    public int SMGGermanBullets = 64;
    public int MaxSMGGermanBullets = 64;
    [Space(20)]
    public int RifleGermanBullets = 5;
    public int MaxRifleGermanBullets = 20;
    [Space(20)]
    public int PistolGermanBullets = 24;
    public int MaxPistolGermanBullets = 24;
    [Space(20)]
    public int PanzerfaustBullets = 2;
    public int MaxPanzefaustBullets = 2;
    [Space(20)]
    [Header("Разное")]
    public GameObject ActionBtn;
    public Text ActionBtnText;
    private CharacterWeaponsController CharacterWeapons;
    private CollectItem Item;

    private void Awake()
    {
        CharacterWeapons = GetComponent<CharacterWeaponsController>();
    }

    private void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2.5f))
        {
            if (hit.collider.GetComponent<CollectItem>())
            {
                Item = hit.collider.GetComponent<CollectItem>();
                if (Item.mainItemType == CollectItem.MainItemType.Weapon)
                    ShowActionBtn("Нажмите, чтобы подобрать", true);

                if (Item.mainItemType == CollectItem.MainItemType.BombPlace)
                    ShowActionBtn("Нажмите, чтобы заложить бомбу", true);
            }
        }
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            if (!hit.collider.GetComponent<CollectItem>())
            {
                Item = null;
                ShowActionBtn("", false);
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F))
            Action();
#endif
    }

    public void ShowActionBtn(string Label, bool State)
    {
        ActionBtn.SetActive(State);
        ActionBtnText.text = Label;
    }

    public void Action()
    {
        if (Item != null)
        {
            CharacterWeapons.GunSfx.PlayOneShot(Item.CollectSound);
            Item.GetItem();
        }
    }
}