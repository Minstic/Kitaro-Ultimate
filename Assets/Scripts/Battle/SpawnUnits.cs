using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnUnits : MonoBehaviour
{
    [SerializeField] private GameObject SpawnPointUnits;
    [SerializeField] private GameObject SpawnPointEnemies;
    [SerializeField] private UnitController UnitDummy;
    public int enemyLimit = 50;
    private int unitLimit = 50;
    [HideInInspector] static public List<UnitController> unitList = new();
    [HideInInspector] static public List<UnitController> enemyList = new();

    void Start()
    {
        StartCoroutine(EnemySpawns());
    }
    public void SpawnUnit(string ID, bool EnemyBase, Transform spawnPosition)
    {
        if (CheckLimit(EnemyBase))
        {
            StatsObject UnitStats = Resources.Load<StatsObject>("Stats/" + ID );
            UnitController UnitInsta = Instantiate(UnitDummy);
            UnitInsta.Stats = UnitStats;
            if (EnemyBase)
            {
                enemyList.Add(UnitInsta);
                UnitInsta.IsEnemy = true;
                UnitInsta.transform.parent = SpawnPointEnemies.transform;
                if (spawnPosition != null)
                {
                    UnitInsta.transform.position = spawnPosition.position;
                    return;
                }
                UnitInsta.transform.position = SpawnPointEnemies.transform.position;
            }
            else
            {
                unitList.Add(UnitInsta);
                UnitInsta.IsEnemy = false;
                UnitInsta.transform.parent = SpawnPointUnits.transform;
                if (spawnPosition != null)
                {
                    UnitInsta.transform.position = spawnPosition.position;
                    return;
                }
                UnitInsta.transform.position = SpawnPointUnits.transform.position;
            }
        }
    }

    IEnumerator EnemySpawns()
    {
        if (PlayerPrefs.GetInt("Playground") == 1)
        {
            yield break;
        }
        SpawnUnit("Character_20470", true, null);
        yield return new WaitForSeconds(1.4f);
        while (true)
        {
            SpawnUnit("Character_20010", true, null);
            yield return new WaitForSeconds(4.2f);
            SpawnUnit("Character_20011", true, null);
            yield return new WaitForSeconds(3.8f);
        }
    }

    public bool CheckLimit(bool enemy)
    {
        if ((!enemy && unitList.Count < unitLimit) || (enemy && enemyList.Count < enemyLimit))
        {
            return true;
        }
        return false;
    }

    public void DecreaseLimitCounter(bool enemy, UnitController unit)
    {
        if (enemy)
        {
            enemyList.Remove(unit);
            return;
        }
        unitList.Remove(unit);
    }
}