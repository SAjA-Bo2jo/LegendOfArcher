/*
 * UIManager ��ũ��Ʈ�� �ټ� UI �г��� ���� ����, Back ��ư ���� �� ���� �ֱ� ���� â �ݱ� �� ������ UI ��Ȳ���Դϴ�.
 * �ܼ� �ɼ�â ���� �޴� ������ �ʿ��ϸ� OptionUIManager�� ����ص� ����մϴ�.
 * ������Ʈ�� ���� �ʿ� �� Ȱ���ϼ���.
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
            Debug.Log("���� UI�� �����ϴ�.");
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
