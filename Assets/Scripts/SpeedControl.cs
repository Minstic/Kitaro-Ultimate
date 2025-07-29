using UnityEngine.UI;
using UnityEngine;

public class SpeedControl : MonoBehaviour
{
	private bool paused = false;
	private bool speedActive = false;
	private float speedScale = 1f;
	public Image speedUpIcon;
	private Color orange;

    void Start ()
    {
        paused = false;
		if (speedUpIcon)
		{
			orange = speedUpIcon.color;
			speedUpIcon.color = Color.gray;
		}
    }
	public void TogglePause()
	{
		paused = !paused;
		Time.timeScale = paused ? 0f : speedScale;
	}
	public void ToggleSpeed()
	{
		speedActive = !speedActive;
		speedScale = speedActive ? 1.5f : 1f;
		speedUpIcon.color = speedActive ? orange : Color.gray;
		Time.timeScale = speedScale;
	}
}
