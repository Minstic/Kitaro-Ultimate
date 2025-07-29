using Spine.Unity;
using UnityEngine;

public class Animation_Viewer : MonoBehaviour
{
	private SkeletonAnimation Anim;
	private Vector2 EnemyScale;
	private string RootAnimPath = "Animations"; // 🔹 La carpeta "Resources" se omite en el path
	private string Skin = "Normal";
	private int PreviousAnimID = 0;

	private void Start()
	{
		Anim = GetComponent<SkeletonAnimation>();
		EnemyScale = new Vector2(-1, 1);
	}

	public void ChangeAnim(int ID)
	{
		switch (ID)
		{
			case 0: Anim.AnimationState.SetAnimation(0, "move", true); break;
			case 1: Anim.AnimationState.SetAnimation(0, "wait", true); break;
			case 2: Anim.AnimationState.SetAnimation(0, "attack", true); break;
			case 3: Anim.AnimationState.SetAnimation(0, "damage", true); break;
		}
		PreviousAnimID = ID;
	}

	public void BecomeEnemy(bool enemy)
	{
		if (enemy)
		{
			transform.localScale = EnemyScale;
			transform.localPosition = new Vector3(-0.5f, 0, 0);
			Skin = "Reverse";
		}
		else
		{
			transform.localScale = Vector3.one;
			transform.localPosition = new Vector3(0.5f, 0, 0);
			Skin = "Normal";
		}
		Reload();
	}

	public void ChangeUnit(string UnitName)
	{
		string UnitPath = RootAnimPath + "/" + UnitName + "/" + UnitName;
		SkeletonDataAsset UnitAnim = Resources.Load<SkeletonDataAsset>(UnitPath + "_SkeletonData");
		TextAsset SKJson = Resources.Load<TextAsset>(UnitPath);

		Anim.skeletonDataAsset = UnitAnim;
		Anim.skeletonDataAsset.skeletonJSON = SKJson;
		Reload();
	}

	private void Reload()
	{
		Anim.initialSkinName = "default";
		Anim.Initialize(true);
		bool skinExists = Anim.skeleton.Data.FindSkin(Skin) != null;
		Anim.initialSkinName = skinExists ? Skin : "default";
		Anim.Initialize(true);
		ChangeAnim(PreviousAnimID);
	}
}
