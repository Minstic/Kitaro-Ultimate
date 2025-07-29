using TMPro;
using UnityEngine;

public class EffectModule : MonoBehaviour
{
    public string effectDescription1 = "";
    public string effectDescription2 = "";
    public string effectDescription3 = "";
    public TMP_Text text1;
    public TMP_Text text2;
    public TMP_Text text3;

    public void DrawEffects()
    {
        text1.text = effectDescription1;
        text2.text = effectDescription2;
        text3.text = effectDescription3;
    }
}
