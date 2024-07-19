using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Attack : MonoBehaviour
{
    StarterAssets.ThirdPersonController _thirdPersonController;
    Animator _animator;
    bool hasAnimator;
    public bool isAttack = false;
    
    AnimatorStateInfo stateInfo;

    public float baseSpeed = 1f; // �⺻ �ִϸ��̼� �ӵ�
    public AnimationCurve speedCurve; // �ӵ� ������ ���� �ִϸ��̼� Ŀ��

    AnimationClip TurnClip;

    // Start is called before the first frame update
    void Start()
    {
        _thirdPersonController = GetComponent<ThirdPersonController>();
        hasAnimator = TryGetComponent(out _animator);

        TurnClip = getAnimClip("turnSwordAttack");
    }

    void OnAttack()
    {
        if(EquipWeapon.isMountSword)
            StartCoroutine(TurnAttack());
    }

    IEnumerator TurnAttack()
    {
        if(isAttack || !_thirdPersonController.Grounded) yield break;

        _animator.applyRootMotion = true;
        _animator.SetTrigger("turnAttack");

        isAttack = true;

        while(!stateInfo.IsName("turnSwordAttack"))
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        while (stateInfo.IsName("turnSwordAttack"))
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime % 1f;

            // �ӵ� ���� Ŀ�꿡 ���� �ӵ� ����
            float speedMultiplier = speedCurve.Evaluate(normalizedTime);
            _animator.speed = baseSpeed * speedMultiplier;

            if (currAnimFrame(stateInfo) == 129)
            {
                _animator.SetBool("ExitAttack", true);
            }


            yield return null;
        }

        _animator.SetBool("ExitAttack", false);
        
        _animator.speed = baseSpeed;
        _animator.applyRootMotion = false;

        //for(int i = 0; i < 80; i++) yield return new WaitForEndOfFrame();

        isAttack = false;
    }

    int currAnimFrame(AnimatorStateInfo currStateInfo)
    {
        float clipLength = TurnClip.length;

        // ���� �ִϸ��̼��� ���� ���� (0���� 1 ����)
        float normalizedTime = currStateInfo.normalizedTime % 1f;

        // ���� �ִϸ��̼��� ���� ��ġ (�� ����)
        float currentTime = normalizedTime * clipLength;

        // ���� �ִϸ��̼��� ������ (�� ������ ��� �� ������ ��ȯ)
        float frameRate = TurnClip.frameRate;
        int currentFrame = Mathf.RoundToInt(currentTime * frameRate);

        // ����� �α׷� ���� ������ Ȯ��
        //Debug.Log("Current Frame: " + currentFrame);

        return currentFrame;
    }

    AnimationClip getAnimClip(string animName)
    {
        // �ִϸ����� ��Ʈ�ѷ����� ���� ������ �ִϸ��̼� Ŭ���� �������� ���
        AnimatorController controller = _animator.runtimeAnimatorController as AnimatorController;
        if (controller != null)
        {
            foreach (var layer in controller.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    if (state.state.nameHash == Animator.StringToHash(animName))
                    {
                        // ������ �ִϸ��̼� ����� �ִϸ��̼� Ŭ������ ��ȯ
                        AnimationClip clip = state.state.motion as AnimationClip;
                        
                        return clip;
                    }
                }
            }
        }

        return null;
    }
}
