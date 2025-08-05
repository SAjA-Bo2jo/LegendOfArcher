// AbilitySlotUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemInfoText;
    [SerializeField] private Button selectButton;

    private GameObject currentAbilityPrefab;

    void Awake()
    {
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
        }
    }

    public void SetAbility(GameObject abilityPrefab, string name, string description, Sprite icon = null)
    {
        currentAbilityPrefab = abilityPrefab;
        itemNameText.text = name;
        itemInfoText.text = description;

        if (icon != null)
        {
            itemImage.sprite = icon;
            itemImage.enabled = true;
        }
        else
        {
            itemImage.enabled = false;
            itemImage.sprite = null;
        }

        selectButton.interactable = true;
        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        currentAbilityPrefab = null;
        itemNameText.text = "";
        itemInfoText.text = "";
        itemImage.sprite = null;
        itemImage.enabled = false;
        selectButton.interactable = false;
        gameObject.SetActive(false);
    }

    private void OnSelectButtonClicked()
    {
        if (currentAbilityPrefab != null)
        {
            AbilitySelectionManager.Instance?.OnAbilitySelected(currentAbilityPrefab);
        }
    }
}