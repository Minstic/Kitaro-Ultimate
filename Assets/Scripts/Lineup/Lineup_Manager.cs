using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LineupManager : MonoBehaviour
{
	public GameObject buttonPrefab;
	public Transform contentPanel;
	public Image selectedImage;
	public TMP_InputField searchInput;
	public AudioController AudioController;
	private Dictionary<string, Sprite> imagesDict = new Dictionary<string, Sprite>();
	private Dictionary<string, GameObject> buttonsDict = new Dictionary<string, GameObject>();
	public Toggle Playgroundtog;

	void Awake()
	{
		LoadImages();
		searchInput.onValueChanged.AddListener(FilterImages);
		print(imagesDict.Count);

		int Playint = PlayerPrefs.GetInt("Playground");
		if (Playint == 0)
		{
			Playgroundtog.isOn = false;
			return;
		}
		Playgroundtog.isOn = true;
	}

	void LoadImages()
	{
		imagesDict.Clear();
		buttonsDict.Clear();

		StatsObject[] stats = Resources.LoadAll<StatsObject>("Stats");

		foreach (StatsObject unit in stats)
		{
			if (unit.Cost != 0)
			{
				Sprite sprite = Resources.Load<Sprite>("Icons/" + unit.name);
				string fileName = unit.name;
				imagesDict[fileName] = sprite;
				CreateButton(fileName, sprite);
			}
			
		}
	}

	void CreateButton(string imageName, Sprite sprite)
	{
		GameObject buttonObj = Instantiate(buttonPrefab, contentPanel);
		Button button = buttonObj.GetComponent<Button>();
		Image buttonImage = buttonObj.transform.Find("Image").GetComponent<Image>();
		TMP_Text Text = buttonObj.transform.Find("Name").GetComponent<TMP_Text>();

		if (buttonImage != null)
		{
			buttonImage.sprite = sprite;
		}
		if (Text != null)
		{
			Text.text = imageName;
			buttonObj.name = imageName;
		}

		button.onClick.AddListener(() => SelectImage(imageName));
		button.onClick.AddListener(() => AudioController.PlaySound());

		buttonsDict[imageName] = buttonObj;
	}

	void SelectImage(string imageName)
	{
		if (imagesDict.ContainsKey(imageName))
		{
			selectedImage.sprite = imagesDict[imageName];
		}
	}

	void FilterImages(string searchText)
	{
		searchText = searchText.ToLower();

		foreach (var entry in buttonsDict)
		{
			string imageName = entry.Key.ToLower();
			entry.Value.SetActive(imageName.Contains(searchText));
		}
	}

	public void PlaygroundActivate(bool toggle)
	{
		int toggleint = 0;
		if (toggle)
		{
			toggleint = 1;
		}
		PlayerPrefs.SetInt("Playground", toggleint);
	}
}
