using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EquipWeapon : MonoBehaviour
{
    Transform backSwordPos;
    Transform handSwordPos;

    static public bool isMountSword = false;

    void Start()
    {
        backSwordPos = FindDeepChild(transform, "BackSwordPosition");
        handSwordPos = FindDeepChild(transform, "SwordPosition");

        if (backSwordPos == null)
        {
            Debug.LogError("BackSwordPosition not found!");
        }

        if (handSwordPos == null)
        {
            Debug.LogError("SwordPosition not found!");
        }
    }

    // Input system
    void OnMount()
    {
        if (isMountSword)
        {
            Transform weapon = handSwordPos.GetChild(0);

            if (weapon != null)
            {
                weapon.SetParent(backSwordPos);
                weapon.localPosition = Vector3.zero;
                weapon.localRotation = Quaternion.identity;

                isMountSword = false;
            }
        }
        else
        {
            Transform weapon = backSwordPos.GetChild(0);

            if (weapon != null)
            {
                weapon.SetParent(handSwordPos);
                weapon.localPosition = Vector3.zero;
                weapon.localRotation = Quaternion.identity;

                isMountSword = true;
            }
        }
    }

    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
