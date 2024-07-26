using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Hang : MonoBehaviour
{
    Animator _animator;
    bool hasAnimator;

    CharacterController _controller;
    AnimatorStateInfo stateInfo;
    Attack attack;

    AnimationClip HangClip;

    public Transform targetBar;
    public Transform hangPosition;

    public bool isHangStart = false;
    EquipWeapon equip;
    // Start is called before the first frame update
    void Start()
    {
        hasAnimator = TryGetComponent(out _animator);

        _controller = GetComponent<CharacterController>();

        stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        attack = GetComponent<Attack>();
        equip = GetComponent<EquipWeapon>();

        HangClip = attack.getAnimClip("HangStart");

        hangPosition = equip.FindDeepChild(transform, "HangPos");

        if (!HangClip)
        {
            Debug.LogError("No Clip !");
        }

    }

    private void Update()
    {
        //stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        //if (stateInfo.IsName("HangStart"))
        //{
        //   transform.position = Vector3.Lerp(transform.position, targetBar.position, Time.deltaTime);
        //}

        //transform.position = Vector3.Lerp(transform.position, targetBar.position, Time.deltaTime);


    }

    void OnHang()
    {
        StartCoroutine(HangStart());
    }

    IEnumerator HangStart()
    {
        if(isHangStart) yield break;

        float prev = 0f;
        float duration = 2f;

        stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        Vector3 start = transform.position;
        Vector3 end = targetBar.position;
        Vector3 peak = (start + end) / 2 + Vector3.up * 5;

        _animator.SetBool("isHang", true);
        isHangStart = true;

        // 컨트롤러를 막 호출해서 터진거같음 기존 이동관련 컴포넌트에서 조건문으로 제어해준다
        while (!stateInfo.IsName("HangStart"))
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        while (stateInfo.IsName("HangStart"))
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            prev += Time.deltaTime;

            float t = prev / 0.5f;
            // 양쪽 보간
            Vector3 a = Vector3.Lerp(start, peak, t);
            Vector3 b = Vector3.Lerp(peak, end, t);

            // 최종 보간
            Vector3 hangPos = Vector3.Lerp(a, b, t);

            Vector3 moveDirection = hangPos - hangPosition.position;

            _controller.Move(moveDirection);

            if (attack.currAnimFrame(stateInfo, HangClip) >= 135)
            {
                _animator.SetBool("isHang", false);
            }

            yield return null;
        }
        _animator.SetBool("isHang", false);

        isHangStart = false;

        yield return null;
    }
}
