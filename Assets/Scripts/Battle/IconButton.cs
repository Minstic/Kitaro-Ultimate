using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [Range(1, 10)]
    public int slotCount = 1;
    public Image IconImage;
	public Image cooldownMask;
    public TMP_Text Cost;
	private float cooldownDuration = 3f;
	private float cooldownTimer = 0f;
	private bool isCoolingDown = false;
    private SpawnUnits Spawner;
    private MoneyManager MoneyManager;
    private StatsObject UnitStats;
    private int costNumber;
    private int Playground;
    private AudioController audioController;
    private void Awake()
    {
        audioController = FindFirstObjectByType<AudioController>();
        string unitID = PlayerPrefs.GetString("slot_" + (slotCount - 1));
        if (unitID == null)
        {
            unitID = "Character_00000";
        }
        Playground = PlayerPrefs.GetInt("Playground");
        ChangeUnit(unitID);
        cooldownMask.enabled = false;
        Spawner = FindObjectOfType<SpawnUnits>();
        MoneyManager = FindObjectOfType<MoneyManager>();
    }
    public void ChangeUnit(string ID)
    {
        UnitStats = Resources.Load<StatsObject>("Stats/" + ID);
        Sprite NewImage = Resources.Load<Sprite>("Icons/" + ID);
        if (NewImage == null)
        {
            NewImage = Resources.Load<Sprite>("Icons/Character_00000");
        }
        IconImage.sprite = NewImage;
        cooldownMask.sprite = NewImage;
        cooldownDuration = UnitStats.CD;
        costNumber = UnitStats.Cost;
        if (Playground == 1 && slotCount > 5)
        {
            costNumber = 0;
            cooldownDuration = 0;
        }
        Cost.text = "<sprite=0> " + costNumber;
    }
	public void StartCooldown()
	{
        if (!isCoolingDown && MoneyManager.EnoughMoney(costNumber) && Spawner.CheckLimit(PlaygroundBool()))
        {
            isCoolingDown = true;
            audioController.PlaySound();
            cooldownTimer = cooldownDuration;
            cooldownMask.fillAmount = 1f;
            cooldownMask.enabled = true;
            MoneyManager.GiveMoney(costNumber*-1);
            if (PlaygroundBool())
            {
                Spawner.SpawnUnit(UnitStats.name, true, null);
                return;
            }
            Spawner.SpawnUnit(UnitStats.name, false, null);
        }
	}

    private bool PlaygroundBool()
    {
        if (Playground == 1 && slotCount > 5)
        {
            return true;
        }
        return false;
    }
	void Update()
	{
		if (isCoolingDown)
		{
			cooldownTimer -= Time.deltaTime;
            cooldownMask.fillAmount = cooldownTimer / cooldownDuration;

			if (cooldownTimer <= 0f)
			{
				isCoolingDown = false;
				cooldownMask.fillAmount = 0f;
				cooldownMask.enabled = false;
			}
		}
	}
}
