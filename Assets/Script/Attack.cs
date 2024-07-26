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

    public float baseSpeed = 1f; // 기본 애니메이션 속도
    public AnimationCurve speedCurve; // 속도 조절을 위한 애니메이션 커브

    AnimationClip TurnClip;

    // Start is called before the first frame update
    void Start()
    {
        _thirdPersonController = GetComponent<ThirdPersonController>();
        hasAnimator = TryGetComponent(out _animator);

        TurnClip = getAnimClip("turnSwordAttack");

        if(!TurnClip)
        {
            Debug.Log("Clip Error");
        }
    }

    //Input system
    void OnAttack()
    {
        if(EquipWeapon.isMountSword)
            StartCoroutine(TurnAttack());
    }

    IEnumerator TurnAttack()
    {
        // 공중 및 공격 중엔 공격 안됨
        if(isAttack || !_thirdPersonController.Grounded) yield break;

        _animator.applyRootMotion = true;
        _animator.SetTrigger("turnAttack");

        isAttack = true;
        
        // 애니메이션 실행까지 대기
        while(!stateInfo.IsName("turnSwordAttack"))
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        // 애니메이션 실행 시 속도 조절, 프레임 체크
        while (stateInfo.IsName("turnSwordAttack"))
        {
            stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = stateInfo.normalizedTime % 1f;

            // 속도 조절 커브에 따라 속도 변경
            float speedMultiplier = speedCurve.Evaluate(normalizedTime);
            _animator.speed = baseSpeed * speedMultiplier;

            if (currAnimFrame(stateInfo, TurnClip) >= 129)
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

        yield return null;
    }

    // 애니메이션 클립을 가져와서 프레임 체크
    public int currAnimFrame(AnimatorStateInfo currStateInfo, AnimationClip clip = null)
    {
        float clipLength = clip.length;

        // 현재 애니메이션의 진행 비율 (0에서 1 사이)
        float normalizedTime = currStateInfo.normalizedTime % 1f;

        // 현재 애니메이션의 진행 위치 (초 단위)
        float currentTime = normalizedTime * clipLength;

        // 현재 애니메이션의 프레임 (초 단위로 계산 후 정수로 변환)
        float frameRate = clip.frameRate;
        int currentFrame = Mathf.RoundToInt(currentTime * frameRate);

        // 디버그 로그로 현재 프레임 확인
        //Debug.Log("Current Frame: " + currentFrame);

        return currentFrame;
    }

    public AnimationClip getAnimClip(string animName)
    {
        // 애니메이터 컨트롤러에서 현재 상태의 애니메이션 클립을 가져오는 방법
        AnimatorController controller = _animator.runtimeAnimatorController as AnimatorController;
        if (controller != null)
        {
            foreach (var layer in controller.layers)
            {
                // 레이어의 최상위 상태 머신 탐색
                AnimationClip clip = FindAnimationClipInStateMachine(layer.stateMachine, animName);
                if (clip != null)
                {
                    return clip;
                }
            }
        }

        return null;
    }

    private AnimationClip FindAnimationClipInStateMachine(AnimatorStateMachine stateMachine, string animName)
    {
        foreach (var state in stateMachine.states)
        {
            if (state.state.nameHash == Animator.StringToHash(animName))
            {
                // 상태의 애니메이션 모션을 애니메이션 클립으로 변환
                AnimationClip clip = state.state.motion as AnimationClip;
                return clip;
            }
        }

        // 서브 스테이트 머신을 재귀적으로 탐색
        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            AnimationClip clip = FindAnimationClipInStateMachine(subStateMachine.stateMachine, animName);
            if (clip != null)
            {
                return clip;
            }
        }

        return null;
    }
}
