using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro�� ����ϱ� ���� �ʿ�

public class AbilitySlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI itemNameText; // Item Name �ؽ�Ʈ
    [SerializeField] private Image itemImage; // �׸� �ڽ��� �̹��� (�����Ƽ ������)
    [SerializeField] private TextMeshProUGUI itemInfoText; // Item Info �ؽ�Ʈ
    [SerializeField] private Button selectButton; // �����Ƽ ���� ��ư

    private GameObject currentAbilityPrefab; // �� ������ ���� ǥ���ϴ� �����Ƽ ������

    void Awake() // Start ��� Awake���� ������ �����Ͽ� ������ ����
    {
        // ��ư Ŭ�� �̺�Ʈ�� �Լ� ����
        selectButton.onClick.AddListener(OnSelectButtonClicked);
    }

    /// <summary>
    /// ���Կ� �����Ƽ ������ �����մϴ�.
    /// </summary>
    /// <param name="abilityPrefab">ǥ���� �����Ƽ�� ������.</param>
    /// <param name="name">�����Ƽ �̸�.</param>
    /// <param name="description">�����Ƽ ����.</param>
    /// <param name="icon">�����Ƽ ������ (���� ����).</param>
    public void SetAbility(GameObject abilityPrefab, string name, string description, Sprite icon = null)
    {
        currentAbilityPrefab = abilityPrefab;
        itemNameText.text = name;
        itemInfoText.text = description;

        if (icon != null)
        {
            itemImage.sprite = icon;
            itemImage.enabled = true; // �̹����� ���̵��� Ȱ��ȭ
        }
        else
        {
            itemImage.enabled = false; // �������� ������ �̹��� ��Ȱ��ȭ
            itemImage.sprite = null; // ��������Ʈ �ʱ�ȭ
        }
        selectButton.interactable = true; // ��ư Ȱ��ȭ
        gameObject.SetActive(true); // ���� GameObject Ȱ��ȭ
    }

    /// <summary>
    /// ������ �ʱ�ȭ�մϴ�.
    /// </summary>
    public void ClearSlot()
    {
        currentAbilityPrefab = null;
        itemNameText.text = "";
        itemInfoText.text = "";
        itemImage.sprite = null;
        itemImage.enabled = false;
        selectButton.interactable = false; // ��ư ��Ȱ��ȭ
        // ��� ������ ��Ȱ��ȭ�ϴ� ���, �������� 3�� �̸��� ���� ��Ȱ��ȭ�ϵ��� ���� ���� �ʿ�.
        // ���⼭�� �׳� ���븸 ����ϴ�. �ʿ�� GameObject.SetActive(false) ���.
        gameObject.SetActive(false); // ���� GameObject ��Ȱ��ȭ (����������)
    }

    /// <summary>
    /// ���� ��ư�� Ŭ���Ǿ��� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯�Դϴ�.
    /// </summary>
    private void OnSelectButtonClicked()
    {
        if (currentAbilityPrefab != null)
        {
            // AbilitySelectionManager�� ã�� ���õ� �����Ƽ�� �����մϴ�.
            // FindObjectOfType�� ����� ���� �� �� �����Ƿ�, �Ŵ����� ���� �Ҵ��ϴ� ���� �����ϴ�.
            FindObjectOfType<AbilitySelectionManager>()?.OnAbilitySelected(currentAbilityPrefab);
        }
    }
}