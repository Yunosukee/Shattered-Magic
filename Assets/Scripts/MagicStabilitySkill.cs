using UnityEngine;

public class MagicStabilitySkill : MonoBehaviour
{
    public static MagicStabilitySkill Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public MagicStability RollStability(int skillLevel)
    {
        float roll = Random.Range(0f, 100f);

        // Szanse bazowane na poziomie skilla
        if (skillLevel >= 100 && roll < 0.05f * skillLevel) return MagicStability.Stable;
        if (skillLevel >= 50 && roll < 0.1f * skillLevel) return MagicStability.Fluctuating;
        if (skillLevel >= 25 && roll < 0.3f * skillLevel) return MagicStability.Unstable;
        if (skillLevel >= 10 && roll < 0.5f * skillLevel) return MagicStability.Volatile;

        return MagicStability.Chaotic;
    }
}