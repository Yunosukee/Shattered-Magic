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

        // Inicjalizacja s³ownika dla szybkiego dostêpu
        foreach (var research in allResearch)
        {
            researchDictionary[research.id] = research;
        }
    }

    public bool CanStartResearch(string researchId)
    {
        if (!researchDictionary.TryGetValue(researchId, out ResearchItem research))
            return false;

        // SprawdŸ czy wymagane badania s¹ ukoñczone
        foreach (var requiredId in research.requiredResearchIds)
        {
            if (!researchDictionary.TryGetValue(requiredId, out ResearchItem requiredResearch) || !requiredResearch.isCompleted)
                return false;
        }

        // SprawdŸ wymagania umiejêtnoœci
        foreach (var skillReq in research.skillRequirements)
        {
            if (!PlayerSkills.Instance.HasSkillLevel(skillReq.skillId, skillReq.requiredLevel))
                return false;
        }

        // SprawdŸ wymagania zasobów
        foreach (var resourceReq in research.resourceRequirements)
        {
            if (!ResourceManager.Instance.HasEnoughResource(resourceReq.resourceId, resourceReq.amount))
                return false;
        }

        // SprawdŸ czy masz wystarczaj¹c¹ iloœæ punktów badañ
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

        // Pobierz punkty badañ
        ResourceManager.Instance.ConsumeResource("research_points", research.researchPointsCost);

        // Oznacz badanie jako ukoñczone
        research.isCompleted = true;

        // Zastosuj efekty badania
        ApplyResearchEffects(research);

        // Powiadom UI o ukoñczeniu badania
        UIManager.Instance.ShowResearchCompletedNotification(research.title);

        return true;
    }

    private void ApplyResearchEffects(ResearchItem research)
    {
        // Tutaj zaimplementuj logikê efektów badania
        // Przyk³ad:
        switch (research.id)
        {
            case "furnace_unlock":
                GameManager.Instance.UnlockBuilding("furnace");
                break;
            case "autonomous_gatherer":
                GameManager.Instance.UnlockBuilding("auto_gatherer");
                break;
                // itd. dla ka¿dego badania
        }
    }

    public List<ResearchItem> GetAvailableResearch()
    {
        return allResearch.Where(r => !r.isCompleted && CanStartResearch(r.id)).ToList();
    }
}