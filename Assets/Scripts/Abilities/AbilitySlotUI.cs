using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro를 사용하기 위해 필요

public class AbilitySlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI itemNameText; // Item Name 텍스트
    [SerializeField] private Image itemImage; // 네모난 박스의 이미지 (어빌리티 아이콘)
    [SerializeField] private TextMeshProUGUI itemInfoText; // Item Info 텍스트
    [SerializeField] private Button selectButton; // 어빌리티 선택 버튼

    private GameObject currentAbilityPrefab; // 이 슬롯이 현재 표시하는 어빌리티 프리팹

    void Awake() // Start 대신 Awake에서 리스너 연결하여 안정성 높임
    {
        // 버튼 클릭 이벤트에 함수 연결
        selectButton.onClick.AddListener(OnSelectButtonClicked);
    }

    /// <summary>
    /// 슬롯에 어빌리티 정보를 설정합니다.
    /// </summary>
    /// <param name="abilityPrefab">표시할 어빌리티의 프리팹.</param>
    /// <param name="name">어빌리티 이름.</param>
    /// <param name="description">어빌리티 설명.</param>
    /// <param name="icon">어빌리티 아이콘 (선택 사항).</param>
    public void SetAbility(GameObject abilityPrefab, string name, string description, Sprite icon = null)
    {
        currentAbilityPrefab = abilityPrefab;
        itemNameText.text = name;
        itemInfoText.text = description;

        if (icon != null)
        {
            itemImage.sprite = icon;
            itemImage.enabled = true; // 이미지가 보이도록 활성화
        }
        else
        {
            itemImage.enabled = false; // 아이콘이 없으면 이미지 비활성화
            itemImage.sprite = null; // 스프라이트 초기화
        }
        selectButton.interactable = true; // 버튼 활성화
        gameObject.SetActive(true); // 슬롯 GameObject 활성화
    }

    /// <summary>
    /// 슬롯을 초기화합니다.
    /// </summary>
    public void ClearSlot()
    {
        currentAbilityPrefab = null;
        itemNameText.text = "";
        itemInfoText.text = "";
        itemImage.sprite = null;
        itemImage.enabled = false;
        selectButton.interactable = false; // 버튼 비활성화
        // 모든 슬롯을 비활성화하는 대신, 선택지가 3개 미만일 때만 비활성화하도록 로직 조정 필요.
        // 여기서는 그냥 내용만 지웁니다. 필요시 GameObject.SetActive(false) 사용.
        gameObject.SetActive(false); // 슬롯 GameObject 비활성화 (선택적으로)
    }

    /// <summary>
    /// 선택 버튼이 클릭되었을 때 호출되는 이벤트 핸들러입니다.
    /// </summary>
    private void OnSelectButtonClicked()
    {
        if (currentAbilityPrefab != null)
        {
            // AbilitySelectionManager를 찾아 선택된 어빌리티를 전달합니다.
            // FindObjectOfType은 비용이 많이 들 수 있으므로, 매니저를 직접 할당하는 것이 좋습니다.
            FindObjectOfType<AbilitySelectionManager>()?.OnAbilitySelected(currentAbilityPrefab);
        }
    }
}