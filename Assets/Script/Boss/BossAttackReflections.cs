using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackReflections : MonoBehaviour
{
    [SerializeField]
    private Transform cutableTransform;
    private List<Transform> cutables;

    private float disableTimer = 0f;
    private Coroutine endBossPatern = null;

    private void Awake()
    {
        GetComponentsInChildren<Transform>(cutables);
    }

    public void EnableCutPoint(int value, float timer)
    {
        cutables[value].gameObject.SetActive(true);
        disableTimer = timer;
        endBossPatern = StartCoroutine(DisAbleCutPointEndPatern(value));
    }

    IEnumerator DisAbleCutPointEndPatern(int value)
    {
        yield return new WaitForSeconds(disableTimer);
        DisAbleCutPoint(value);
    }

    public void DisAbleCutPoint(int value)
    {
        cutables[value].gameObject.SetActive(false);
    }
}
