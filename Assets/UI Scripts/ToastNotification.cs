using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ToastNotification : MonoBehaviour
{
    public static ToastNotification Instance { get; private set; }
    [SerializeField] private ItemDatabase itemDatabase;
    private RectTransform toastContainer;
    private float animationDuration = 0.5f;
    private float displayDuration = 2f;
    private Queue<ToastData> toastQueue = new Queue<ToastData>();
    private bool isShowingToast = false;

    private struct ToastData
    {
        public string message;
        public uint itemId;
        public int amount;
        public ToastType type;
        public bool hasItem;
    }

    void Awake()
    {
        Instance = this;
        CreateToastContainer();
        Debug.Log("[ToastNotification] Initialized");
    }

    private void CreateToastContainer()
    {
        GameObject container = new GameObject("ToastContainer");
        container.transform.SetParent(transform, false);

        toastContainer = container.AddComponent<RectTransform>();
        toastContainer.anchorMin = new Vector2(0, 0);
        toastContainer.anchorMax = new Vector2(1, 0);
        toastContainer.pivot = new Vector2(0.5f, 0);
        toastContainer.sizeDelta = new Vector2(0, 50);
        toastContainer.anchoredPosition = Vector2.zero;
    }

    public void ShowToast(string message, ToastType type)
    {
        var toastData = new ToastData
        {
            message = message,
            type = type,
            hasItem = false
        };

        EnqueueToast(toastData);
    }

    public void ShowToast(uint itemId, int amount, string message, ToastType type)
    {
        var toastData = new ToastData
        {
            message = message,
            itemId = itemId,
            amount = amount,
            type = type,
            hasItem = true
        };

        EnqueueToast(toastData);
    }

    private void EnqueueToast(ToastData toastData)
    {
        toastQueue.Enqueue(toastData);
        if (!isShowingToast)
        {
            ShowNextToast();
        }
    }

    private void ShowNextToast()
    {
        if (toastQueue.Count == 0)
        {
            isShowingToast = false;
            return;
        }

        isShowingToast = true;
        var toastData = toastQueue.Dequeue();
        GameObject toast = toastData.hasItem ?
            CreateToastUI(toastData.itemId, toastData.amount, toastData.message, toastData.type) :
            CreateToastUI(toastData.message, toastData.type);

        StartCoroutine(AnimateToast(toast));
    }

    private GameObject CreateToastUI(string message, ToastType type)
    {
        GameObject toast = new GameObject("Toast");
        toast.transform.SetParent(toastContainer, false);

        RectTransform toastRect = toast.AddComponent<RectTransform>();
        toastRect.sizeDelta = new Vector2(160, 60);

        Color backgroundColor = type switch
        {
            ToastType.Success => new Color(0.1f, 0.4f, 0.1f, 0.9f),
            ToastType.Error => new Color(0.4f, 0.1f, 0.1f, 0.9f),
            _ => new Color(0.1f, 0.1f, 0.4f, 0.9f)
        };

        Image background = toast.AddComponent<Image>();
        background.color = backgroundColor;
        background.sprite = Resources.Load<Sprite>("UI/RoundedRect");
        background.type = Image.Type.Sliced;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(toast.transform, false);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = message;
        text.fontSize = 16;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(10, 5);
        textRect.offsetMax = new Vector2(-10, -5);

        return toast;
    }

    private GameObject CreateToastUI(uint itemId, int amount, string message, ToastType type)
    {
        GameObject toast = new GameObject("Toast");
        toast.transform.SetParent(toastContainer, false);

        RectTransform toastRect = toast.AddComponent<RectTransform>();
        toastRect.sizeDelta = new Vector2(160, 60);

        Color backgroundColor = type switch
        {
            ToastType.Success => new Color(0.1f, 0.4f, 0.1f, 0.9f),
            ToastType.Error => new Color(0.4f, 0.1f, 0.1f, 0.9f),
            _ => new Color(0.1f, 0.1f, 0.4f, 0.9f)
        };

        Image background = toast.AddComponent<Image>();
        background.color = backgroundColor;
        background.sprite = Resources.Load<Sprite>("UI/RoundedRect");
        background.type = Image.Type.Sliced;

        HorizontalLayoutGroup layout = toast.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 5, 5);
        layout.spacing = 10;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(toast.transform, false);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(48, 48);

        Image icon = iconObj.AddComponent<Image>();

        // Try to get item sprite, use white square if null
        if (itemDatabase != null)
        {
            var sprite = itemDatabase.GetItemSprite(itemId);
            icon.sprite = sprite;
            icon.color = Color.white;
        }
        else
        {
            // Create white square as fallback
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            icon.sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }
        icon.preserveAspect = true;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(toast.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(80, 48);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = $"{message}\n+{amount}";
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Left;

        return toast;
    }

    private IEnumerator AnimateToast(GameObject toast)
    {
        CanvasGroup canvasGroup = toast.AddComponent<CanvasGroup>();
        RectTransform rect = toast.GetComponent<RectTransform>();

        RectTransform canvasRect = transform.root.GetComponent<RectTransform>();
        float minY = -canvasRect.rect.height / 2;
        float targetY = minY + 50f;

        rect.anchoredPosition = new Vector2(0, minY);
        canvasGroup.alpha = 0;

        float elapsed = 0;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            rect.anchoredPosition = Vector2.Lerp(new Vector2(0, minY), new Vector2(0, targetY), t);
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);

            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        elapsed = 0;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            canvasGroup.alpha = Mathf.Lerp(1, 0, t);

            yield return null;
        }

        Destroy(toast);
        ShowNextToast();
    }
}