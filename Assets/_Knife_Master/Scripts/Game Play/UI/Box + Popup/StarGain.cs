using TMPro;
using UnityEngine;

public class StarGain : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankStar;
    [SerializeField] private TextMeshProUGUI killStar;
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI countStar;
    [SerializeField] private TextMeshProUGUI adsStar;

    public void Init(int r, int kt, int ks, int c, int a)
    {
        rankStar.text = "+" + r;
        killText.text = kt + " kills";
        killStar.text = "+" + ks;
        countStar.text = "+" + c;
        adsStar.text = "+" + a;
    }
}
