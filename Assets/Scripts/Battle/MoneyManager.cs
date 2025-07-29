using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    private float Money;
    public int MaxMoney = 16500;
    public TMP_Text MoneyText;
    
    public bool EnoughMoney(int unitCost)
    {
        if (Money >= unitCost)
        {
            return true;
        }
        return false;
    }
    public void GiveMoney(int Amount)
    {
        Money += Amount;
    }
    void Update()
    {
        if (Money < MaxMoney)
        {
            Money += Time.deltaTime*250.5f;
        }
        else if (Money > MaxMoney)
        {
            Money = MaxMoney;
        }
        MoneyText.text = "<sprite=0> " + ((int)Money).ToString() + "/" + (int)MaxMoney;
    }
}
