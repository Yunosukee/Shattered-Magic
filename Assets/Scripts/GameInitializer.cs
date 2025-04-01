using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class GameInitializer : MonoBehaviour
{
    public static GameInitializer Instance { get; private set; }
    private GameObject mainCanvas;
    private GameObject skillsPanel;
    private GameObject inventoryPanel;
    private GameObject resourcePanel;
    private GameObject craftingPanel;
    private GameObject questPanel;
    //private QuestManager questManager;
    //private ResearchManager researchManager;
    //private PlayerSkills playerSkills;
    //private ResourceManager resourceManager;



    void Start()
    {
        Instance = this;
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        Debug.Log("[GameInitializer] Starting initialization sequence");

        // Setup EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        // Setup Canvas
        SetupMainCanvas();
        yield return new WaitForEndOfFrame();

        // Initialize databases
        InitializeDatabases();
        yield return new WaitForEndOfFrame();

        // Create game systems
        GameObject systems = new GameObject("GameSystems");
        systems.AddComponent<SkillSystem>();
        systems.AddComponent<InventorySystem>();
        systems.AddComponent<ResourceGatherer>();
        systems.AddComponent<MagicStabilitySkill>();
        systems.AddComponent<CraftingSystem>();
        systems.AddComponent<ResourceManager>();
        systems.AddComponent<PlayerSkills>();
        systems.AddComponent<QuestManager>();
        systems.AddComponent<ResearchManager>();

        // Setup UI
        CreatePanels();
        CreateNavigationButtons();
        SwitchPanel("Skills");

        Debug.Log("[GameInitializer] Initialization sequence complete");
    }

    private void InitializeDatabases()
    {
        Debug.Log("[GameInitializer] Starting database initialization");

        var itemDB = ScriptableObject.CreateInstance<ItemDatabase>();
        ItemInitializer.InitializeAllItems(itemDB);
        ItemDatabase.Instance = itemDB;

        var nodeDB = ScriptableObject.CreateInstance<ResourceNodeDatabase>();
        NodeInitializer.InitializeAllNodes(nodeDB);
        ResourceNodeDatabase.Instance = nodeDB;

        var recipeDB = ScriptableObject.CreateInstance<RecipeDatabase>();
        RecipeInitializer.InitializeAllRecipes(recipeDB, itemDB);
        RecipeDatabase.Instance = recipeDB;

        Debug.Log("[GameInitializer] Database initialization complete");
    }

    void SetupMainCanvas()
    {
        mainCanvas = new GameObject("MainCanvas");
        Canvas canvas = mainCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1;

        CanvasScaler scaler = mainCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        mainCanvas.AddComponent<GraphicRaycaster>();
        Debug.Log("[GameInitializer] Main canvas setup complete");
    }

    void CreatePanels()
    {
        skillsPanel = CreatePanel("SkillsPanel");
        skillsPanel.AddComponent<SkillsUI>();

        inventoryPanel = CreatePanel("InventoryPanel");
        inventoryPanel.AddComponent<InventoryUI>();

        resourcePanel = CreatePanel("ResourcePanel");
        resourcePanel.AddComponent<ResourceNodeUI>();

        craftingPanel = CreatePanel("CraftingPanel");
        craftingPanel.AddComponent<CraftingUI>();

        questPanel = CreatePanel("QuestPanel");
        questPanel.AddComponent<QuestUI>();

        GameObject toastObj = new GameObject("ToastNotification");
        toastObj.transform.SetParent(mainCanvas.transform, false);
        toastObj.AddComponent<ToastNotification>();

        Debug.Log("[GameInitializer] UI panels created");
    }

    GameObject CreatePanel(string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(mainCanvas.transform, false);

        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 0.9f);
        rectTransform.offsetMin = new Vector2(20, 20);
        rectTransform.offsetMax = new Vector2(-20, -20);

        Image background = panel.AddComponent<Image>();
        background.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        return panel;
    }

    void CreateNavigationButtons()
    {
        GameObject buttonContainer = new GameObject("NavigationButtons");
        buttonContainer.transform.SetParent(mainCanvas.transform, false);

        RectTransform containerRect = buttonContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 0.9f);
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        Image navBackground = buttonContainer.AddComponent<Image>();
        navBackground.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        float buttonWidth = 150f;
        float buttonSpacing = 20f;
        float totalWidth = (buttonWidth * 4) + (buttonSpacing * 3);
        float startX = -totalWidth / 2;

        CreateNavButton("Skills", new Vector2(startX, 0), buttonContainer);
        CreateNavButton("Inventory", new Vector2(startX + buttonWidth + buttonSpacing, 0), buttonContainer);
        CreateNavButton("Resources", new Vector2(startX + (buttonWidth + buttonSpacing) * 2, 0), buttonContainer);
        CreateNavButton("Crafting", new Vector2(startX + (buttonWidth + buttonSpacing) * 3, 0), buttonContainer);
        CreateNavButton("Quests", new Vector2(startX + (buttonWidth + buttonSpacing) * 4, 0), buttonContainer);

        Debug.Log("[GameInitializer] Navigation buttons created");
    }

    void CreateNavButton(string panelName, Vector2 position, GameObject parent)
    {
        GameObject buttonObj = new GameObject($"{panelName}Button");
        buttonObj.transform.SetParent(parent.transform, false);

        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(150, 40);
        rectTransform.anchoredPosition = position;

        Image buttonImage = buttonObj.AddComponent<Image>();
        Button button = buttonObj.AddComponent<Button>();

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f);
        button.colors = colors;

        buttonImage.color = colors.normalColor;
        button.targetGraphic = buttonImage;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = panelName;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        buttonText.fontSize = 24;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnButtonClick(panelName));
    }

    private void OnButtonClick(string panelName)
    {
        Debug.Log($"[GameInitializer] Button {panelName} clicked!");
        SwitchPanel(panelName);
    }

    public void UnlockBuilding(string buildingId)
    {
        Debug.Log($"[GameInitializer] Unlocked building: {buildingId}");
        // Implement your building unlock logic here

        // Show notification
        ToastNotification.Instance.ShowToast($"Unlocked new building: {buildingId}", ToastType.Success);
    }

    public void SwitchPanel(string panelName)
    {
        skillsPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        resourcePanel.SetActive(false);
        craftingPanel.SetActive(false);
        questPanel.SetActive(false);

        switch (panelName)
        {
            case "Skills":
                skillsPanel.SetActive(true);
                break;
            case "Inventory":
                inventoryPanel.SetActive(true);
                break;
            case "Resources":
                resourcePanel.SetActive(true);
                break;
            case "Crafting":
                craftingPanel.SetActive(true);
                break;
            case "Quests":
                questPanel.SetActive(true);
                break;
        }
        Debug.Log($"[GameInitializer] Switched to panel: {panelName}");
    }
}