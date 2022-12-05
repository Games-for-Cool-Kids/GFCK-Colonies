using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

internal static class UniqueObjectNameGenerator
{
    private static Dictionary<string, List<string>> _existingObjectNames = new();

    public static void GiveUniqueName(GameObject gameObject)
    {
        string original_name = gameObject.name.Replace("(Clone)", "");

        List<string> existing_names;
        if (!_existingObjectNames.TryGetValue(original_name, out existing_names))
        {
            existing_names = _existingObjectNames[original_name] = new();
        }

        gameObject.name = ObjectNames.GetUniqueName(existing_names.ToArray(), original_name);
        existing_names.Add(gameObject.name);
    }
}
