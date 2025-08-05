using UnityEngine;

public class OptionUIManager : MonoBehaviour
{
    [Header("Option Panels")]
    [SerializeField] private GameObject optionPanel;       // 옵션창 전체 패널
    [SerializeField] private GameObject collectionPanel;   // 콜렉션 창
    [SerializeField] private GameObject challengePanel;    // 챌린지 창
    [SerializeField] private GameObject makerPanel;        // 메이커 창

    private void Start()
    {
        if (collectionPanel != null) collectionPanel.SetActive(false);
        if (challengePanel != null) challengePanel.SetActive(false);
        if (makerPanel != null) makerPanel.SetActive(false);
        // 옵션창은 TitleSceneUIManager가 켜고 끄므로 여기서 비활성화하지 않음
    }

    // 옵션창 전체 닫기 (옵션창 내 메인 Back 버튼)
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
        string url = "https://www.notion.so/teamsparta/2-2382dc3ef514814c8d5fe650bc5aa0d9"; // 원하는 URL로 변경하세요
        Application.OpenURL(url);
    }
}
