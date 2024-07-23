using System.Collections;
using System.Collections.Generic;
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

    bool isHangStart = false;
    // Start is called before the first frame update
    void Start()
    {
        hasAnimator = TryGetComponent(out _animator);

        _controller = GetComponent<CharacterController>();

        stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        attack = GetComponent<Attack>();

        HangClip = attack.getAnimClip("HangStart");

        if(!HangClip)
        {
            Debug.LogError("No Clip !");
        }

    }

    private void Update()
    {
        //stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        //if (stateInfo.IsName("HangStart"))
        //{
        //    print("123");
        //}
    }

    void OnHang()
    {
        StartCoroutine(HangStart());
    }

    IEnumerator HangStart()
    {
        if(isHangStart) yield break;

        _animator.SetBool("isHang", true);
        isHangStart = true;

        // 다음 애니메이션으로 넘어가고 isname 비교하니까 터짐
        while (!stateInfo.IsName("HangStart"))
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        while (stateInfo.IsName("HangStart"))
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            //transform.position = Vector3.Lerp(transform.position, targetBar.position, Time.deltaTime);

            _controller.Move(transform.position);

            if (attack.currAnimFrame(stateInfo, HangClip) == 135)
            {
                _animator.SetBool("isHang", false);
            }

            yield return null;
        }

        isHangStart = false;

        yield return null;
    }
}
