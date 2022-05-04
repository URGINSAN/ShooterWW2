using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    public enum MainItemType
    {
        Weapon = 0,
        BombPlace = 1
    }
    public MainItemType mainItemType;
    public enum ItemType
    {
        Rifle = 0,
        SMG = 1,
        Rifle_German = 2,
        SMG_German = 3,
        Pistol_German = 4
    }
    public ItemType itemType;
    public int Count;
    public bool Randomize;
    [Space(20)]
    public AudioClip CollectSound;
    [Header("BombPlace")]
    public GameObject BombPlaceHide;
    public GameObject BombPlaceShow;

    public void GetItem()
    {
        if (mainItemType == MainItemType.Weapon)
        {
            int C;
            if (!Randomize)
                C = Count;
            else
                C = Random.RandomRange(1, Count);

            switch (itemType)
            {
                case ItemType.Rifle:
                    CheckPlayerSlots(0);
                    if (FindObjectOfType<CharacterAmmoController>().RifleBullets < FindObjectOfType<CharacterAmmoController>().MaxRifleBullets)
                    {
                        //CheckPlayerSlots(0);
                        FindObjectOfType<CharacterAmmoController>().RifleBullets += C;
                        if (FindObjectOfType<CharacterAmmoController>().RifleBullets > FindObjectOfType<CharacterAmmoController>().MaxRifleBullets)
                            FindObjectOfType<CharacterAmmoController>().RifleBullets = FindObjectOfType<CharacterAmmoController>().MaxRifleBullets;
                        ApplyGetItem();
                    }

                    break;
                case ItemType.SMG:
                    CheckPlayerSlots(1);
                    if (FindObjectOfType<CharacterAmmoController>().SMGBullets < FindObjectOfType<CharacterAmmoController>().MaxSMGBullets)
                    {
                        //CheckPlayerSlots(1);
                        FindObjectOfType<CharacterAmmoController>().SMGBullets += C;
                        if (FindObjectOfType<CharacterAmmoController>().SMGBullets > FindObjectOfType<CharacterAmmoController>().MaxSMGBullets)
                            FindObjectOfType<CharacterAmmoController>().SMGBullets = FindObjectOfType<CharacterAmmoController>().MaxSMGBullets;
                        ApplyGetItem();
                    }

                    break;
                case ItemType.Rifle_German:
                    CheckPlayerSlots(2);
                    if (FindObjectOfType<CharacterAmmoController>().RifleGermanBullets < FindObjectOfType<CharacterAmmoController>().MaxRifleGermanBullets)
                    {
                        //CheckPlayerSlots(2);
                        FindObjectOfType<CharacterAmmoController>().RifleGermanBullets += C;
                        if (FindObjectOfType<CharacterAmmoController>().RifleGermanBullets > FindObjectOfType<CharacterAmmoController>().MaxRifleGermanBullets)
                            FindObjectOfType<CharacterAmmoController>().RifleGermanBullets = FindObjectOfType<CharacterAmmoController>().MaxRifleGermanBullets;
                        ApplyGetItem();
                    }

                    break;
                case ItemType.SMG_German:
                    CheckPlayerSlots(3);
                    if (FindObjectOfType<CharacterAmmoController>().SMGGermanBullets < FindObjectOfType<CharacterAmmoController>().MaxSMGGermanBullets)
                    {
                        //CheckPlayerSlots(3);
                        FindObjectOfType<CharacterAmmoController>().SMGGermanBullets += C;
                        if (FindObjectOfType<CharacterAmmoController>().SMGGermanBullets > FindObjectOfType<CharacterAmmoController>().MaxSMGGermanBullets)
                            FindObjectOfType<CharacterAmmoController>().SMGGermanBullets = FindObjectOfType<CharacterAmmoController>().MaxSMGGermanBullets;
                        ApplyGetItem();
                    }

                    break;
                case ItemType.Pistol_German:
                    CheckPlayerSlots(4);
                    if (FindObjectOfType<CharacterAmmoController>().PistolGermanBullets < FindObjectOfType<CharacterAmmoController>().MaxPistolGermanBullets)
                    {
                        //CheckPlayerSlots(4);
                        FindObjectOfType<CharacterAmmoController>().PistolGermanBullets += C;
                        if (FindObjectOfType<CharacterAmmoController>().PistolGermanBullets > FindObjectOfType<CharacterAmmoController>().MaxPistolGermanBullets)
                            FindObjectOfType<CharacterAmmoController>().PistolGermanBullets = FindObjectOfType<CharacterAmmoController>().MaxPistolGermanBullets;
                        ApplyGetItem();
                    }

                    break;
            }
        }

        if (mainItemType == MainItemType.BombPlace)
        {
            BombPlaceHide.SetActive(false);
            BombPlaceShow.SetActive(true);
            GetComponentInChildren<BombController>().StartBomb = true;
            BombPlaceShow.transform.SetParent(null);
            Destroy(gameObject);
        }
    }

    void ApplyGetItem()
    {
        FindObjectOfType<CharacterWeaponsController>().CurrentWeaponController.ApplyAmmoType();
        FindObjectOfType<CharacterWeaponsController>().CheckAmmo();
        Destroy(gameObject);
    }

    void CheckPlayerSlots(int WeaponType)
    {
        CharacterWeaponsController CharacterWeapons;
        if (FindObjectOfType<CharacterWeaponsController>() != null)
        {
            CharacterWeapons = FindObjectOfType<CharacterWeaponsController>();

            if (CharacterWeapons.WeaponSlot0 == null)
            {
                CharacterWeapons.WeaponSlot0 = CharacterWeapons.WeaponNotInSlot[WeaponType];
                CharacterWeapons.CheckWeapons();
                CharacterWeapons.WeaponSlot0.transform.SetParent(CharacterWeapons.Slot0.transform);
                return;
            }

            if (CharacterWeapons.WeaponSlot0 != null && CharacterWeapons.WeaponSlot1 == null && CharacterWeapons.WeaponSlot0 != CharacterWeapons.WeaponNotInSlot[WeaponType])
            {
                CharacterWeapons.WeaponSlot1 = CharacterWeapons.WeaponNotInSlot[WeaponType];
                CharacterWeapons.WeaponSlot1.transform.SetParent(CharacterWeapons.Slot1.transform);
                CharacterWeapons.GunMark2Img.sprite = CharacterWeapons.WeaponSlot1.GetComponent<WeaponController>().GunIcon;
                return;
            }
            
            if (CharacterWeapons.WeaponSlot0 != null && CharacterWeapons.WeaponSlot1 != null)
            {
                if (CharacterWeapons.PrimarySlot == true && CharacterWeapons.WeaponSlot0 != CharacterWeapons.WeaponNotInSlot[WeaponType] && CharacterWeapons.WeaponSlot1 != CharacterWeapons.WeaponNotInSlot[WeaponType])
                {
                    GameObject GO = Instantiate(CharacterWeapons.WeaponItemsForCollect[CharacterWeapons.CurrentWeaponType], transform.position, Quaternion.identity);
                    GO.GetComponent<CollectItem>().Count = CharacterWeapons.WeaponSlot0.GetComponent<WeaponController>().AmmoInClip;
                    CharacterWeapons.WeaponSlot0.GetComponent<WeaponController>().AmmoInClip = 0;
                    CharacterWeapons.WeaponSlot0.transform.SetParent(CharacterWeapons.NotInSlotWeapon.transform);
                    CharacterWeapons.WeaponSlot0 = CharacterWeapons.WeaponNotInSlot[WeaponType];
                    CharacterWeapons.CurrentWeaponController = CharacterWeapons.WeaponSlot0.GetComponent<WeaponController>();
                    CharacterWeapons.WeaponSlot0.transform.SetParent(CharacterWeapons.Slot0.transform);
                    
                    Destroy(gameObject);
                    return;
                }
                if (CharacterWeapons.PrimarySlot == false && CharacterWeapons.WeaponSlot0 != CharacterWeapons.WeaponNotInSlot[WeaponType] && CharacterWeapons.WeaponSlot1 != CharacterWeapons.WeaponNotInSlot[WeaponType])
                {
                    GameObject GO = Instantiate(CharacterWeapons.WeaponItemsForCollect[CharacterWeapons.CurrentWeaponType], transform.position, Quaternion.identity);
                    GO.GetComponent<CollectItem>().Count = CharacterWeapons.WeaponSlot1.GetComponent<WeaponController>().AmmoInClip;
                    CharacterWeapons.WeaponSlot1.GetComponent<WeaponController>().AmmoInClip = 0;
                    CharacterWeapons.WeaponSlot1.transform.SetParent(CharacterWeapons.NotInSlotWeapon.transform);
                    CharacterWeapons.WeaponSlot1 = CharacterWeapons.WeaponNotInSlot[WeaponType];
                    CharacterWeapons.CurrentWeaponController = CharacterWeapons.WeaponSlot1.GetComponent<WeaponController>();
                    CharacterWeapons.WeaponSlot1.transform.SetParent(CharacterWeapons.Slot1.transform);
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }
}