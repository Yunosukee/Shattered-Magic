using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance { get; private set; }
    private Image recipeImage;
    private Image progressBar;
    private Button craftButton;
    private Button cancelButton;
    private TextMeshProUGUI recipeInfoText;
    private uint currentRecipeId = 1;

    void Awake()
    {
        Instance = this;
        Debug.Log("[CraftingUI] Initialized");
    }

    void Start()
    {
        CreateCraftingUI();
        SetupButtonListeners();
        UpdateRecipeDisplay();
        Debug.Log("[CraftingUI] UI Created");
    }

    void Update()
    {
        if (CraftingSystem.Instance != null && CraftingSystem.Instance.IsCrafting(currentRecipeId))
        {
            float progress = CraftingSystem.Instance.GetProgress(currentRecipeId);
            UpdateProgressBar(progress);

            if (progress >= 1f)
            {
                ResetCraftingState();
            }
        }
    }

    private void SetupButtonListeners()
    {
        if (craftButton != null)
            craftButton.onClick.AddListener(StartCrafting);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelCrafting);

        if (progressBar != null)
        {
            RectTransform fillRect = progressBar.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(0, fillRect.sizeDelta.y);
        }

        if (cancelButton != null)
            cancelButton.gameObject.SetActive(false);
    }

    private void UpdateProgressBar(float progress)
    {
        RectTransform fillRect = progressBar.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(200 * progress, fillRect.sizeDelta.y);
    }

    public void ResetCraftingState()
    {
        craftButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(false);
        RectTransform fillRect = progressBar.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(0, fillRect.sizeDelta.y);
        UpdateRecipeDisplay();
    }

    private void CreateCraftingUI()
    {
        CreateRecipeSelection();

        // Main container
        GameObject container = new GameObject("CraftingNode");
        container.transform.SetParent(transform, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(200, 300); // Increased height

        // Node image container
        GameObject imageContainer = new GameObject("ImageContainer");
        imageContainer.transform.SetParent(container.transform, false);
        RectTransform imageContainerRect = imageContainer.AddComponent<RectTransform>();
        imageContainerRect.anchorMin = new Vector2(0, 0.6f); // Changed from 0.7f
        imageContainerRect.anchorMax = new Vector2(1, 1f);
        imageContainerRect.sizeDelta = Vector2.zero;

        // Node info container
        GameObject infoContainer = new GameObject("InfoContainer");
        infoContainer.transform.SetParent(container.transform, false);
        RectTransform infoContainerRect = infoContainer.AddComponent<RectTransform>();
        infoContainerRect.anchorMin = new Vector2(0, 0.35f); // Changed from 0.4f
        infoContainerRect.anchorMax = new Vector2(1, 0.6f); // Changed from 0.7f
        infoContainerRect.sizeDelta = Vector2.zero;

        // Progress bar container
        GameObject progressContainer = new GameObject("ProgressContainer");
        progressContainer.transform.SetParent(container.transform, false);
        RectTransform progressContainerRect = progressContainer.AddComponent<RectTransform>();
        progressContainerRect.anchorMin = new Vector2(0, 0.25f); // Changed from 0.2f
        progressContainerRect.anchorMax = new Vector2(1, 0.35f); // Changed from 0.4f
        progressContainerRect.sizeDelta = Vector2.zero;

        // Buttons container
        GameObject buttonsContainer = new GameObject("ButtonsContainer");
        buttonsContainer.transform.SetParent(container.transform, false);
        RectTransform buttonsContainerRect = buttonsContainer.AddComponent<RectTransform>();
        buttonsContainerRect.anchorMin = new Vector2(0, 0);
        buttonsContainerRect.anchorMax = new Vector2(1, 0.2f);
        buttonsContainerRect.sizeDelta = Vector2.zero;

        CreateRecipeImage(imageContainer.transform);
        CreateRecipeInfo(infoContainer.transform);
        CreateProgressBar(progressContainer.transform);
        CreateButtons(buttonsContainer.transform);
    }

    private void CreateRecipeImage(Transform parent)
    {
        GameObject imageObj = new GameObject("RecipeImage");
        imageObj.transform.SetParent(parent, false);
        recipeImage = imageObj.AddComponent<Image>();
        RectTransform imageRect = recipeImage.GetComponent<RectTransform>();
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.sizeDelta = Vector2.zero;
    }

    private void CreateRecipeInfo(Transform parent)
    {
        GameObject infoTextObj = new GameObject("RecipeInfo");
        infoTextObj.transform.SetParent(parent, false);
        recipeInfoText = infoTextObj.AddComponent<TextMeshProUGUI>();
        recipeInfoText.fontSize = 14;
        recipeInfoText.alignment = TextAlignmentOptions.Center;
        recipeInfoText.color = Color.white;

        RectTransform infoRect = infoTextObj.GetComponent<RectTransform>();
        infoRect.anchorMin = Vector2.zero;
        infoRect.anchorMax = Vector2.one;
        infoRect.sizeDelta = Vector2.zero;
    }

    private void CreateProgressBar(Transform parent)
    {
        GameObject progressObj = new GameObject("ProgressBar");
        progressObj.transform.SetParent(parent, false);
        Image progressBg = progressObj.AddComponent<Image>();
        progressBg.color = Color.gray;
        RectTransform progressRect = progressBg.GetComponent<RectTransform>();
        progressRect.anchorMin = Vector2.zero;
        progressRect.anchorMax = Vector2.one;
        progressRect.sizeDelta = Vector2.zero;

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(progressObj.transform, false);
        progressBar = fillObj.AddComponent<Image>();
        progressBar.color = Color.green;
        RectTransform fillRect = progressBar.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(0, 1);
        fillRect.pivot = new Vector2(0, 0.5f);
        fillRect.sizeDelta = new Vector2(0, 0);
    }

    private void CreateButtons(Transform parent)
    {
        CreateButton("CraftButton", "Craft", new Vector2(0.25f, 0), parent, ref craftButton);
        CreateButton("CancelButton", "Cancel", new Vector2(0.25f, 0), parent, ref cancelButton);
    }

    private void CreateButton(string name, string text, Vector2 anchor, Transform parent, ref Button buttonRef)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f);

        RectTransform buttonRect = button.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(anchor.x, 0);
        buttonRect.anchorMax = new Vector2(anchor.x + 0.5f, 1);
        buttonRect.sizeDelta = Vector2.zero;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.color = Color.white;
        buttonText.fontSize = 14;
        buttonText.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        button.targetGraphic = buttonImage;
        buttonRef = button;
    }

    private void CreateRecipeSelection()
    {
        GameObject selectionPanel = new GameObject("RecipeSelection");
        selectionPanel.transform.SetParent(transform, false);

        RectTransform selectionRect = selectionPanel.AddComponent<RectTransform>();
        selectionRect.anchorMin = new Vector2(0, 0);
        selectionRect.anchorMax = new Vector2(1, 0.3f);
        selectionRect.offsetMin = Vector2.zero;
        selectionRect.offsetMax = Vector2.zero;

        GridLayoutGroup grid = selectionPanel.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(80, 80);
        grid.spacing = new Vector2(10, 10);
        grid.padding = new RectOffset(10, 10, 10, 10);

        // Add background image to make selection panel visible
        Image background = selectionPanel.AddComponent<Image>();
        background.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        foreach (var recipe in RecipeDatabase.Instance.recipes)
        {
            CreateRecipeButton(selectionPanel.transform, recipe);
        }
    }

    private void CreateRecipeButton(Transform parent, RecipeData recipe)
    {
        GameObject buttonObj = new GameObject($"RecipeButton_{recipe.id}");
        buttonObj.transform.SetParent(parent, false);

        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.sprite = recipe.resultItem.icon;

        button.onClick.AddListener(() => SelectRecipe(recipe.id));
    }

    private void SelectRecipe(uint recipeId)
    {
        Debug.Log($"Selected Recipe ID: {recipeId}");
        currentRecipeId = recipeId;
        UpdateRecipeDisplay();
    }

    private void UpdateRecipeDisplay()
    {
        if (RecipeDatabase.Instance == null || recipeImage == null || recipeInfoText == null)
            return;

        var recipe = RecipeDatabase.Instance.GetRecipe(currentRecipeId);
        if (recipe != null && recipe.resultItem != null)
        {
            recipeImage.sprite = recipe.resultItem.icon;
            string requirementsText = "";
            foreach (var req in recipe.requirements)
            {
                bool hasEnough = InventorySystem.Instance.HasEnough(req.item.id, req.quantity);
                string colorTag = hasEnough ? "<color=green>" : "<color=red>";
                requirementsText += $"\n{colorTag}{req.item.name} x{req.quantity}</color>";
            }

            recipeInfoText.text = $"{recipe.name}\nTime: {recipe.craftingDuration}s{requirementsText}";
        }
    }

    private void StartCrafting()
    {
        CraftingSystem.Instance.StartCrafting(currentRecipeId);
        craftButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);
    }

    private void CancelCrafting()
    {
        CraftingSystem.Instance.CancelCrafting(currentRecipeId);
        craftButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(false);
        RectTransform fillRect = progressBar.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(0, fillRect.sizeDelta.y);
    }
}
