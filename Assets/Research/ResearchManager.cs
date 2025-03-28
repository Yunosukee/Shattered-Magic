using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Resources;

public class ResearchManager : MonoBehaviour
{
    public static ResearchManager Instance { get; private set; }

    [SerializeField] private List<ResearchItem> allResearch = new List<ResearchItem>();
    private Dictionary<string, ResearchItem> researchDictionary = new Dictionary<string, ResearchItem>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Inicjalizacja s�ownika dla szybkiego dost�pu
        foreach (var research in allResearch)
        {
            researchDictionary[research.id] = research;
        }
    }

    public bool CanStartResearch(string researchId)
    {
        if (!researchDictionary.TryGetValue(researchId, out ResearchItem research))
            return false;

        // Sprawd� czy wymagane badania s� uko�czone
        foreach (var requiredId in research.requiredResearchIds)
        {
            if (!researchDictionary.TryGetValue(requiredId, out ResearchItem requiredResearch) || !requiredResearch.isCompleted)
                return false;
        }

        // Sprawd� wymagania umiej�tno�ci
        foreach (var skillReq in research.skillRequirements)
        {
            if (!PlayerSkills.Instance.HasSkillLevel(skillReq.skillId, skillReq.requiredLevel))
                return false;
        }

        // Sprawd� wymagania zasob�w
        foreach (var resourceReq in research.resourceRequirements)
        {
            if (!ResourceManager.Instance.HasEnoughResource(resourceReq.resourceId, resourceReq.amount))
                return false;
        }

        // Sprawd� czy masz wystarczaj�c� ilo�� punkt�w bada�
        if (!ResourceManager.Instance.HasEnoughResource("research_points", research.researchPointsCost))
            return false;

        return true;
    }

    public bool StartResearch(string researchId)
    {
        if (!CanStartResearch(researchId))
            return false;

        ResearchItem research = researchDictionary[researchId];

        // Pobierz wymagane zasoby
        foreach (var resourceReq in research.resourceRequirements)
        {
            ResourceManager.Instance.ConsumeResource(resourceReq.resourceId, resourceReq.amount);
        }

        // Pobierz punkty bada�
        ResourceManager.Instance.ConsumeResource("research_points", research.researchPointsCost);

        // Oznacz badanie jako uko�czone
        research.isCompleted = true;

        // Zastosuj efekty badania
        ApplyResearchEffects(research);

        // Powiadom UI o uko�czeniu badania
        UIManager.Instance.ShowResearchCompletedNotification(research.title);

        return true;
    }

    private void ApplyResearchEffects(ResearchItem research)
    {
        // Tutaj zaimplementuj logik� efekt�w badania
        // Przyk�ad:
        switch (research.id)
        {
            case "furnace_unlock":
                GameManager.Instance.UnlockBuilding("furnace");
                break;
            case "autonomous_gatherer":
                GameManager.Instance.UnlockBuilding("auto_gatherer");
                break;
                // itd. dla ka�dego badania
        }
    }

    public List<ResearchItem> GetAvailableResearch()
    {
        return allResearch.Where(r => !r.isCompleted && CanStartResearch(r.id)).ToList();
    }
}