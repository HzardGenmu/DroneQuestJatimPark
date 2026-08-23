using UnityEngine;
using System.Collections.Generic;

public class TutorialTargetRegistry : MonoBehaviour
{
    public static TutorialTargetRegistry Instance;

    [System.Serializable]
    public class TargetEntry
    {
        public string id;
        public Transform target;
    }

    [Header("Initial Targets")]
    public List<TargetEntry> targets =
        new List<TargetEntry>();

    private Dictionary<string, List<Transform>> lookup;

    private void Awake()
    {
        Instance = this;

        lookup =
            new Dictionary<string, List<Transform>>();

        foreach (var t in targets)
        {
            Register(t.id, t.target);
        }
    }

    // GET ALL TARGETS WITH SAME ID
    public List<Transform> GetTargets(string id)
    {
        if (lookup.TryGetValue(id, out var list))
        {
            return list;
        }

        Debug.LogWarning(
            $"Tutorial targets not found: {id}"
        );

        return new List<Transform>();
    }

    // GET FIRST TARGET ONLY
    public Transform GetTarget(string id)
    {
        var targets = GetTargets(id);

        if (targets.Count > 0)
        {
            return targets[0];
        }

        return null;
    }

    public void Register(string id, Transform target)
    {
        if (target == null)
            return;

        if (!lookup.ContainsKey(id))
        {
            lookup[id] =
                new List<Transform>();
        }

        if (!lookup[id].Contains(target))
        {
            lookup[id].Add(target);
        }
    }

    public void Unregister(
        string id,
        Transform target
    )
    {
        if (!lookup.ContainsKey(id))
            return;

        lookup[id].Remove(target);

        if (lookup[id].Count == 0)
        {
            lookup.Remove(id);
        }
    }
}