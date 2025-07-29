using System.Collections;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour
{
    public GameObject CannonChara;
    public int CannonDMG = 5000;
    public float cooldownDuration = 50f;
	private float cooldownTimer;
    public Image cooldownMask;
    public Transform EnemyBaseSP;
    private SkeletonGraphic CannonGraphic;
    private bool CannonReady = false;
    void Start()
    {
        CannonGraphic = CannonChara.GetComponent<SkeletonGraphic>();
        cooldownTimer = cooldownDuration;
    }
    public void StartFireCannon()
    {
        if (CannonReady)
        {
            CannonReady = false;
            cooldownTimer = cooldownDuration;
            cooldownMask.fillAmount = 1f;
            cooldownMask.enabled = true;
            CannonChara.SetActive(true);
            CannonGraphic.AnimationState.SetAnimation(0, "CannonChara", false);
            StartCoroutine(WaitAnim());
        }
    }
    IEnumerator WaitAnim()
    {
        yield return new WaitForSpineAnimationComplete(CannonGraphic.AnimationState.GetCurrent(0));
        FireCannon();
    }

    void FireCannon()
    {
        CannonChara.SetActive(false);
        foreach (Transform Enemy in EnemyBaseSP)
        {
            Enemy.GetComponent<UnitController>()?.CannonAttack(CannonDMG);
        }
    }
    void Update()
    {
        if (!CannonReady)
		{
			cooldownTimer -= Time.deltaTime;
            cooldownMask.fillAmount = cooldownTimer / cooldownDuration;

			if (cooldownTimer <= 0f)
			{
				CannonReady = true;
				cooldownMask.fillAmount = 0f;
				cooldownMask.enabled = false;
			}
		}
    }
}