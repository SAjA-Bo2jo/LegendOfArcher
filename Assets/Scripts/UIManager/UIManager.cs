/*
 * UIManager 스크립트는 다수 UI 패널의 스택 관리, Back 버튼 누를 때 가장 최근 열린 창 닫기 등 복잡한 UI 상황용입니다.
 * 단순 옵션창 서브 메뉴 관리만 필요하면 OptionUIManager만 사용해도 충분합니다.
 * 프로젝트에 따라 필요 시 활용하세요.
 */

using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject collectionPanel;
    [SerializeField] private GameObject challengePanel;
    [SerializeField] private GameObject makerPanel;

    private Stack<GameObject> activePanels = new Stack<GameObject>();

    private void Start()
    {
        if (optionPanel != null) optionPanel.SetActive(false);
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (challengePanel != null) challengePanel.SetActive(false);
        if (makerPanel != null) makerPanel.SetActive(false);
    }

    public void OpenPanel(GameObject panel)
    {
        if (panel == null) return;

        if (activePanels.Count > 0 && activePanels.Peek() == panel) return;

        panel.SetActive(true);
        activePanels.Push(panel);
    }

    public void CloseLastPanel()
    {
        if (activePanels.Count == 0) return;

        GameObject lastPanel = activePanels.Pop();
        lastPanel.SetActive(false);
    }

    public void OnBackButton()
    {
        if (activePanels.Count > 0)
            CloseLastPanel();
        else
            Debug.Log("닫을 UI가 없습니다.");
    }

    public void OnOptionButton()
    {
        if (optionPanel != null) optionPanel.SetActive(true);

        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (challengePanel != null) challengePanel.SetActive(false);
        if (makerPanel != null) makerPanel.SetActive(false);

        activePanels.Clear();
        activePanels.Push(optionPanel);
    }

    public void OnCollectionButton() => OpenPanel(collectionPanel);

    public void OnChallengeButton() => OpenPanel(challengePanel);

    public void OnMakerButton() => OpenPanel(makerPanel);
}
