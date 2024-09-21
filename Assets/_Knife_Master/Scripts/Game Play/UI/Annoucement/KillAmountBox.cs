using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillAmountBox : MonoBehaviour
{
    public Image border;
    public TextMeshProUGUI killAmountText;

    public void ShowBox(Player killed)
    {
        int killAmount = killed.killer.killAmount;

        if (killAmount >= 2)
        {
            border.color = Color.green;
            if (killAmount == 2)
            {
                killAmountText.text = "Double Kill";
            }
            else if (killAmount == 3)
            {
                killAmountText.text = "Tripple Kill";
            }
            else if (killAmount == 4)
            {
                killAmountText.text = "Quadra Kill";
            }
            else if (killAmount == 5)
            {
                killAmountText.text = "Penta Kill";
            }
            else if (killAmount == 6)
            {
                killAmountText.text = "Rage Kill";
            }
            else if (killAmount == 7)
            {
                killAmountText.text = "Monster Kill";
            }
            else if (killAmount == 8)
            {
                killAmountText.text = "Legendary !";
            }
            else
            {
                killAmountText.text = "GodLike !!!";
            }
            
            gameObject.SetActive(true);
        }
        
    }
}
