using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Spine.Unity;

public class ScrollViewImageSelector : MonoBehaviour
{
	public GameObject buttonPrefab; // Prefab de los botones con imágenes
	public Transform contentPanel; // Panel dentro del ScrollView
	public Image selectedImage; // Imagen seleccionada
	public TMP_InputField searchInput; // Campo de búsqueda
	public AudioController AudioController;
	[SerializeField] private SkeletonAnimation Anim;

	private Dictionary<string, Sprite> imagesDict = new Dictionary<string, Sprite>();
	private Dictionary<string, GameObject> buttonsDict = new Dictionary<string, GameObject>(); // Guardamos los botones creados

	void Start()
	{
		LoadImages();
		searchInput.onValueChanged.AddListener(FilterImages); // Filtrar en tiempo real
	}

	void LoadImages()
	{
		imagesDict.Clear();
		buttonsDict.Clear();

		// 🔹 Carga todos los iconos desde la carpeta Resources/Icons
		Sprite[] sprites = Resources.LoadAll<Sprite>("Icons");

		foreach (Sprite sprite in sprites)
		{
			string fileName = sprite.name; // 🔹 Nombre del archivo sin extensión
			imagesDict[fileName] = sprite;
			CreateButton(fileName, sprite);
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
		button.onClick.AddListener(() => Anim.GetComponent<Animation_Viewer>().ChangeUnit(imageName));
		button.onClick.AddListener(() => AudioController.PlaySound());

		buttonsDict[imageName] = buttonObj; // Guardar botón en el diccionario
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
			entry.Value.SetActive(imageName.Contains(searchText)); // Mostrar solo los que coincidan
		}
	}
}
