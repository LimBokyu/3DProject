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

        questList.Add("���� Ʃ�丮��", new Quest("�Ϲ� ����", "�Ϲ� �������� 3���� �ڽ��� �ı��϶�", " �ı��� �ڽ� {0} / {1}",0,5));
        //questList.Add();
    }
}
