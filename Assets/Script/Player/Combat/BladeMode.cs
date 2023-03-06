using Cinemachine;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BladeMode : MonoBehaviour
{
    public TimeManager timemanager;
    public Transform CutPlane;
    private float cutTimer = 0.2f;
    private float attackTimer = 0;
    private bool cutable = true;
    private Coroutine cutcool = null;

    private PlayerCamera playerCam;
    private PostProcessingController postprocessingcontroller;
    private PlayerController playercontroller;
    private ScreenFlash flash;

    public UnityEvent BladeStart;
    public UnityEvent BladeEnd;
    private bool OnBladeMode;
    private bool BladeAttack;

    private void Awake()
    {
        playerCam = GetComponent<PlayerCamera>();
        playercontroller= GetComponent<PlayerController>();
        postprocessingcontroller = GetComponent<PostProcessingController>();
    }

    private void Start()
    {
        flash = playercontroller.flash;
    }
    public void BladeModeState()
    {
        if (!BladeAttack)
            CutPlane.Rotate(0f, 0f, Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime * 100);

        if (OnBladeMode)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                BladeStart?.Invoke();
                playerCam.CameraShake();
                BladeAttack = true;
                cutable = false;
                if (cutcool == null)
                    cutcool = StartCoroutine(CutCoolTime());
            }

            if (BladeAttack)
            {
                attackTimer += Time.unscaledDeltaTime;
                if (attackTimer >= cutTimer)
                {
                    BladeEnd?.Invoke();
                    attackTimer = 0;
                    BladeAttack = !BladeAttack;
                }
            }
        }
    }

    private IEnumerator CutCoolTime()
    {
        yield return new WaitForSecondsRealtime(cutTimer);
        cutable = true;
        cutcool = null;
        float ran = Random.Range(10f, 30f);
        int randompos = Random.Range(0, 2);
        float positive = randompos == 0 ? 1f : -1f;
        CutPlane.Rotate(0f, 0f, ran * positive);
    }
    public void ModeChanger(bool BladeMode)
    {
        OnBladeMode = !BladeMode;
        CutPlane.gameObject.SetActive(OnBladeMode);
        CutPlane.localEulerAngles = Vector3.zero;

        playerCam.BladeModeCameraSetting(OnBladeMode);
        postprocessingcontroller.BladeModeSetPostProcessing(OnBladeMode);
        playercontroller.anim.SetBool("BladeMode", OnBladeMode);
        attackTimer = 0;
        timemanager.SlowMotion(OnBladeMode);

        string debug = OnBladeMode ? "BladeModeOn" : "BladeModeOff";
        Debug.Log(debug);
        if (!OnBladeMode)
        {
            float y = transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            transform.Rotate(new Vector3(0, y, 0));
            // ㄴ BladeMode 시에 바라보고 있던 방향으로의 캐릭터를 회전 
        }
        else
            flash.BladeFlash();
    }
}
