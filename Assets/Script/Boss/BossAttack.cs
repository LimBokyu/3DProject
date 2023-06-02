using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    [SerializeField]
    private BossAttackReflections reflection;

    private Dictionary<int, BossPatern> paterns = new Dictionary<int, BossPatern>();
    private int paternCount;

    private void Awake()
    {
        SetPaterns();
    }

    private void Start()
    {
        PlayPatern();
    }

    public void BossAttackBehaviour()
    {

    }

    private void RandomAttack()
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            default:
                break;
        }
    } 

    IEnumerator PlayPatern()
    {
        while (true)
        {
            BossPatern patern = null;
            int random = Random.Range(0, paternCount);
            paterns.TryGetValue(random, out patern);
            yield return new WaitForSeconds(patern.CoolTime);
        }
    }

    private void SetPaterns()
    {
        paterns.Clear();
        paterns.Add(0,new BossPatern(1.5f,30));
        paternCount = paterns.Count - 1;
    }
}
