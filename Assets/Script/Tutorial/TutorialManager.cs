using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    Dictionary<string, Quest> questList;
    List<Quest> quests;
    

    private void SetQuest()
    {
        questList = new Dictionary<string, Quest>();

        questList.Add("공격 튜토리얼", new Quest("일반 공격", "일반 공격으로 3개의 박스를 파괴하라", " 파괴된 박스 {0} / {1}",0,5));
        //questList.Add();
    }
}
