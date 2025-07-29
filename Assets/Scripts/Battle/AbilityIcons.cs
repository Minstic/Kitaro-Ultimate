using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityIcons : MonoBehaviour
{
    public GameObject iconPrefab;

	private List<GameObject> activeIcons = new List<GameObject>();
    private Sprite[] loadedIcons;

    void Awake()
	{
		loadedIcons = Resources.LoadAll<Sprite>("AbilityIcons/TemporaryEffectIcons");
	}

	public void UpdateIcons(List<int> iconIDs, bool enemy)
	{
		foreach (var icon in activeIcons)
			Destroy(icon);

		activeIcons.Clear();

		for (int i = 0; i < iconIDs.Count; i++)
		{
            Sprite sprite = loadedIcons.FirstOrDefault(s => s.name == "TemporaryEffectIcons_" + iconIDs[i]);
			if (sprite == null) continue;

			GameObject icon = Instantiate(iconPrefab, transform);
			icon.GetComponent<SpriteRenderer>().sprite = sprite;
			float iconsDistance = enemy? -0.6f : 0.6f;
			icon.transform.localPosition = new Vector3(i*iconsDistance, 0, 0);
			activeIcons.Add(icon);
		}
	}
}