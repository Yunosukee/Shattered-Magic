using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class ResourceNodeUI : MonoBehaviour
{
    public static ResourceNodeUI Instance { get; private set; }
    private Image nodeImage;
    private Image progressBar;
    private Button startButton;
    private Button stopButton;
    private TextMeshProUGUI nodeInfoText;
    private uint currentNodeId = 1;

    void Awake()
    {
        Instance = this;
        Debug.Log("[ResourceNodeUI] Initialized");
    }

    void Start()
    {
        CreateNodeUI();
        SetupButtonListeners();
        UpdateNodeDisplay();
        Debug.Log("[ResourceNodeUI] UI Created");
    }

    void Update()
    {
        if (ResourceGatherer.Instance != null && ResourceGatherer.Instance.isGathering)
        {
            UpdateProgressBar();
        }
    }

    private void SetupButtonListeners()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartGathering);

        if (stopButton != null)
            stopButton.onClick.AddListener(StopGathering);

        if (progressBar != null)
        {
            RectTransform fillRect = progressBar.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(0, fillRect.sizeDelta.y);
        }

        if (stopButton != null)
            stopButton.gameObject.SetActive(false);
    }

    private void UpdateProgressBar()
    {
        float progress = ResourceGatherer.Instance.GetGatheringProgress();
        RectTransform fillRect = progressBar.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(200 * progress, fillRect.sizeDelta.y);
    }

    private void CreateNodeUI()
    {
        // Node selection panel
        CreateNodeSelection();

        // Main container
        GameObject container = new GameObject("ResourceNode");
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

        // Create components inside containers
        CreateNodeImage(imageContainer.transform);
        CreateNodeInfo(infoContainer.transform);
        CreateProgressBar(progressContainer.transform);
        CreateButtons(buttonsContainer.transform);
    }

    private void CreateNodeImage(Transform parent)
    {
        GameObject nodeImageObj = new GameObject("NodeImage");
        nodeImageObj.transform.SetParent(parent, false);
        nodeImage = nodeImageObj.AddComponent<Image>();
        RectTransform imageRect = nodeImage.GetComponent<RectTransform>();
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.sizeDelta = Vector2.zero;
    }

    private void CreateNodeInfo(Transform parent)
    {
        GameObject infoTextObj = new GameObject("NodeInfo");
        infoTextObj.transform.SetParent(parent, false);
        nodeInfoText = infoTextObj.AddComponent<TextMeshProUGUI>();
        nodeInfoText.fontSize = 14;
        nodeInfoText.alignment = TextAlignmentOptions.Center;
        nodeInfoText.color = Color.white;

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
        fillRect.sizeDelta = new Vector2(progressRect.rect.width, 0);
    }
    private void CreateButtons(Transform parent)
    {
        CreateButton("StartButton", "Start Gathering", new Vector2(0.25f, 0), parent, ref startButton);
        CreateButton("StopButton", "Stop Gathering", new Vector2(0.25f, 0), parent, ref stopButton);
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

        // Fix text
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

        // Make button interactive
        button.targetGraphic = buttonImage;
        buttonRef = button;
    }

    private void UpdateNodeIcon()
    {
        var nodeData = ResourceNodeDatabase.Instance.GetNodeData(currentNodeId);
        if (nodeData != null)
        {
            nodeImage.sprite = nodeData.icon;
            nodeImage.color = Color.white;
        }
    }

    private void StartGathering()
    {
        var nodeData = ResourceNodeDatabase.Instance.GetNodeData(currentNodeId);
        if (SkillSystem.Instance.GetSkillLevel(nodeData.requiredSkillId) < nodeData.requiredLevel)
        {
            ToastNotification.Instance.ShowToast("Not enough level!", ToastType.Error);
            return;
        }

        ResourceGatherer.Instance.StartGathering(currentNodeId);
        startButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
    }

    private void StopGathering()
    {
        ResourceGatherer.Instance.StopGathering();
        startButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
        RectTransform fillRect = progressBar.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(0, fillRect.sizeDelta.y);
    }

    private void CreateNodeSelection()
    {
        GameObject selectionPanel = new GameObject("NodeSelection");
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

        // Tworzenie przycisków dla ka¿dego node'a
        foreach (var node in ResourceNodeDatabase.Instance.nodes)
        {
            CreateNodeButton(selectionPanel.transform, node);
        }
    }

    private void CreateNodeButton(Transform parent, ResourceNodeData nodeData)
    {
        GameObject buttonObj = new GameObject($"NodeButton_{nodeData.id}");
        buttonObj.transform.SetParent(parent, false);

        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.sprite = nodeData.icon;
        buttonImage.color = Color.white;

        button.onClick.AddListener(() => SelectNode(nodeData.id));
    }

    private void SelectNode(uint nodeId)
    {
        currentNodeId = nodeId;
        UpdateNodeIcon();
        UpdateNodeInfo();
    }

    private void UpdateNodeInfo()
    {
        var nodeData = ResourceNodeDatabase.Instance.GetNodeData(currentNodeId);
        if (nodeData != null)
        {
            nodeInfoText.text = $"{nodeData.name}\nLevel: {nodeData.requiredLevel}\nTime: {nodeData.gatherTime}s";
        }
    }

    public void ResetGatheringState()
    {
        startButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
        RectTransform fillRect = progressBar.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(0, fillRect.sizeDelta.y);
    }

    private void UpdateNodeDisplay()
    {
        var nodeData = ResourceNodeDatabase.Instance.GetNodeData(currentNodeId);
        if (nodeData != null)
        {
            UpdateNodeIcon();
            UpdateNodeInfo();
            Debug.Log($"[ResourceNodeUI] Updated display for node: {nodeData.name}");
        }
    }
}