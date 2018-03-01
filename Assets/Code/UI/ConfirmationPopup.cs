using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmationPopup : MonoBehaviour
{
    public Product prod;
    public Text Message;

    public void Accept()
    {
        if (UserUtilities.User.CurrentPoints >= prod.TokenValue)
        {
            UserUtilities.SpendPoints((int)prod.TokenValue);
            UIManager.Instance.IndicateScore((int)prod.TokenValue, false);
            SoundManager.Instance.PlaySound("CoinCollect");

            GameManager.Organizations Org = (GameManager.Organizations)Enum.Parse(typeof(GameManager.Organizations), prod.Organization.ToString());
            if (GameManager.Instance.MiniGameEnabledOrgs.Contains(Org))
            {
                GameManager.Instance.Unlock(Org);
            }

            UIManager.Instance.DisplayNotification(true, "You have successfully redeemed " + prod.ProductName + " for " + prod.TokenValue + " Tokens.");
            GameManager.Instance.analytics.LogEvent(new EventHitBuilder().SetEventCategory("Reward Redemption").SetEventAction(prod.ProductName).SetEventValue(prod.TokenValue));
        }
        else
        {
            SoundManager.Instance.PlaySound("error");
            UIManager.Instance.DisplayNotification(true, "You do not have enough tokens for this purchase.");
        }
        Destroy(gameObject);
    }

	public void Decline()
    {
        SoundManager.Instance.PlaySound("error");
        Destroy(gameObject);
    }
}
