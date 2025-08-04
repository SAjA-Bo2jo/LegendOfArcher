using UnityEngine;

public class OptionUIManager : MonoBehaviour
{
    [Header("Option Panels")]
    [SerializeField] private GameObject optionPanel;       // �ɼ�â ��ü �г�
    [SerializeField] private GameObject collectionPanel;   // �ݷ��� â
    [SerializeField] private GameObject challengePanel;    // ç���� â
    [SerializeField] private GameObject makerPanel;        // ����Ŀ â

    private void Start()
    {
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (challengePanel != null) challengePanel.SetActive(false);
        if (makerPanel != null) makerPanel.SetActive(false);
        // �ɼ�â�� TitleSceneUIManager�� �Ѱ� ���Ƿ� ���⼭ ��Ȱ��ȭ���� ����
    }

    // �ɼ�â ��ü �ݱ� (�ɼ�â �� ���� Back ��ư)
    public void OnOptionBackButton()
    {
        if (optionPanel != null)
            optionPanel.SetActive(false);
    }

    public void OnCollectionBackButton()
    {
        if (collectionPanel != null)
            collectionPanel.SetActive(false);
    }

    public void OnChallengeBackButton()
    {
        if (challengePanel != null)
            challengePanel.SetActive(false);
    }

    public void OnMakerBackButton()
    {
        if (makerPanel != null)
            makerPanel.SetActive(false);
    }

    public void OnCollectionButton()
    {
        if (collectionPanel != null)
            collectionPanel.SetActive(true);
    }

    public void OnChallengeButton()
    {
        if (challengePanel != null)
            challengePanel.SetActive(true);
    }

    public void OnMakerButton()
    {
        if (makerPanel != null)
            makerPanel.SetActive(true);
    }

    public void OnNotionButton()
    {
        string url = "https://www.notion.so/teamsparta/2-2382dc3ef514814c8d5fe650bc5aa0d9"; // ���ϴ� URL�� �����ϼ���
        Application.OpenURL(url);
    }
}
