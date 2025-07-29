using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
    }
    public void ChangeScene(string sceneName)
    {
        TeamAbilities.atkPowerEnemies = 1;
        TeamAbilities.atkPowerUnits = 1;
        TeamAbilities.atkPowerTraitEnemies = 0;
        SceneManager.LoadScene(sceneName);
    }
}
