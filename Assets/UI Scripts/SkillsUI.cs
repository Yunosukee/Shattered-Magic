using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SkillsUI : MonoBehaviour
{
    public static SkillsUI Instance { get; private set; }
    [SerializeField] private TMP_FontAsset mainFont;
    private Dictionary<uint, SkillDisplay> skillDisplays = new Dictionary<uint, SkillDisplay>();

    [System.Serializable]
    private class SkillDisplay
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI levelText;
        public Slider xpSlider;
        public TextMeshProUGUI xpText;
    }

    void Awake()
    {
        Instance = this;
        CreateSkillUI();
    }

    void CreateSkillUI()
    {
        // Container for all skill categories
        GameObject skillContainer = new GameObject("SkillContainer");
        skillContainer.transform.SetParent(transform, false);

        RectTransform containerRect = skillContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = new Vector2(20, 20);
        containerRect.offsetMax = new Vector2(-20, -20);

        // Add scroll functionality
        ScrollRect scroll = skillContainer.AddComponent<ScrollRect>();

        // Content holder
        GameObject content = new GameObject("Content");
        content.transform.SetParent(skillContainer.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = Vector2.one;
        contentRect.sizeDelta = new Vector2(0, 800); // Height for all skills

        scroll.content = contentRect;
        scroll.horizontal = false;
        scroll.vertical = true;

        // Create skill categories in the content
        float yPosition = -50f;

        // Magic Skills
        yPosition = CreateSkillCategory("Magic Skills", content, yPosition);
        CreateSkillDisplay(SkillIDs.STABILIZATION, "Stabilization", content, ref yPosition);
        CreateSkillDisplay(SkillIDs.EXTRACTION, "Extraction", content, ref yPosition);
        CreateSkillDisplay(SkillIDs.RESONANCE, "Resonance", content, ref yPosition);
        CreateSkillDisplay(SkillIDs.DETECTION, "Detection", content, ref yPosition);

        // Tech Skills
        yPosition = CreateSkillCategory("Tech Skills", content, yPosition);
        CreateSkillDisplay(SkillIDs.ENGINEERING, "Engineering", content, ref yPosition);
        CreateSkillDisplay(SkillIDs.SYNTHESIS, "Synthesis", content, ref yPosition);
        CreateSkillDisplay(SkillIDs.ANALYSIS, "Analysis", content, ref yPosition);
        CreateSkillDisplay(SkillIDs.AUTOMATION, "Automation", content, ref yPosition);

        // Crafting Skills
        yPosition = CreateSkillCategory("Crafting Skills", content, yPosition);
        CreateSkillDisplay(SkillIDs.CONSTRUCTION, "Construction", content, ref yPosition);
        CreateSkillDisplay(SkillIDs.ENCHANTING, "Enchanting", content, ref yPosition);
        CreateSkillDisplay(SkillIDs.ALCHEMY, "Alchemy", content, ref yPosition);
        CreateSkillDisplay(SkillIDs.RUNECRAFT, "Runecraft", content, ref yPosition);
    }

    private float CreateSkillCategory(string categoryName, GameObject parent, float yPosition)
    {
        GameObject categoryObj = new GameObject(categoryName);
        categoryObj.transform.SetParent(parent.transform, false);

        RectTransform rectTransform = categoryObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.sizeDelta = new Vector2(0, 40);
        rectTransform.anchoredPosition = new Vector2(0, yPosition);

        TextMeshProUGUI headerText = CreateText(categoryObj, categoryName, Vector2.zero);
        headerText.fontSize = 24;
        headerText.fontStyle = FontStyles.Bold;

        return yPosition - 60; // Return position for next element
    }

    void CreateSkillDisplay(uint skillId, string skillName, GameObject parent, ref float yPosition)
    {
        GameObject skillPanel = new GameObject($"Skill_{skillName}");
        skillPanel.transform.SetParent(parent.transform, false);

        RectTransform rectTransform = skillPanel.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.1f, 1);
        rectTransform.anchorMax = new Vector2(0.9f, 1);
        rectTransform.sizeDelta = new Vector2(0, 40);
        rectTransform.anchoredPosition = new Vector2(0, yPosition);

        SkillDisplay display = new SkillDisplay();
        float xOffset = -rectTransform.rect.width * 0.3f;

        display.nameText = CreateText(skillPanel, skillName, new Vector2(xOffset, 0));
        display.nameText.fontSize = 14;
        display.nameText.alignment = TextAlignmentOptions.Left;

        display.levelText = CreateText(skillPanel, "Lvl 1", new Vector2(0, 0));
        display.levelText.fontSize = 14;

        display.xpSlider = CreateSlider(skillPanel, new Vector2(xOffset + 200, 0));
        RectTransform sliderRect = display.xpSlider.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(100, 10);

        display.xpText = CreateText(skillPanel, "0/100 XP", new Vector2(xOffset + 300, 0));
        display.xpText.fontSize = 12;

        skillDisplays.Add(skillId, display);

        yPosition -= 45;
    }

    private TextMeshProUGUI CreateText(GameObject parent, string content, Vector2 position)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent.transform);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.font = mainFont;
        text.fontSize = 14;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(100, 30);

        return text;
    }

    private Slider CreateSlider(GameObject parent, Vector2 position)
    {
        GameObject sliderObj = new GameObject("XPSlider");
        sliderObj.transform.SetParent(parent.transform, false);

        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchoredPosition = position;
        sliderRect.sizeDelta = new Vector2(100, 10);

        Slider slider = sliderObj.AddComponent<Slider>();

        // Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderObj.transform, false);
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;

        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.0f, 0.7f, 0.0f, 1f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        // Setup slider components
        slider.fillRect = fillRect;
        slider.targetGraphic = backgroundImage;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0;
        slider.interactable = false;

        return slider;
    }

    public void UpdateSkillDisplay(uint skillId)
    {
        if (skillDisplays.ContainsKey(skillId))
        {
            int level = SkillSystem.Instance.GetSkillLevel(skillId);
            float currentXP = SkillSystem.Instance.GetSkillExperience(skillId);
            float maxXP = level * 100f; // Base XP requirement

            SkillDisplay display = skillDisplays[skillId];
            display.levelText.text = $"Lvl {level}";
            display.xpSlider.value = currentXP / maxXP;
            display.xpText.text = $"{(int)currentXP}/{(int)maxXP} XP";
        }
    }
}