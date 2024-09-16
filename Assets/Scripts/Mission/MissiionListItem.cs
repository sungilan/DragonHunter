using System.Security.Claims;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionListItem : MonoBehaviour
{
    public Image icoMission;
    public TextMeshProUGUI txtMissionName;
    public TextMeshProUGUI txtRewardVal;
    public TextMeshProUGUI txtProcessing;
    public Image icoReward;
    public Button btnClaim;
    public ClaimBtns claimBtns;
    public Image gauge;
    public int id;

    public void Init(int id, string missionSpriteName, string missionName, string rewardSpriteName, int rewardVal, string hexColor, int goalVal, int doingVal = 0)
    {
        this.id = id;
        this.icoMission.sprite = AssetManager.Instance.atlas.GetSprite(missionSpriteName);
        this.txtMissionName.text = missionName;
        this.icoReward.sprite = AssetManager.Instance.atlas.GetSprite(rewardSpriteName);
        this.txtProcessing.text = $"{doingVal} / {goalVal}";

        Color color;
        ColorUtility.TryParseHtmlString(hexColor, out color);
        this.txtRewardVal.text = rewardVal.ToString();
        this.txtRewardVal.color = color;

        if(doingVal > 0 )
        {
            var per = (float)doingVal / (float)goalVal;
            gauge.fillAmount = per;

            if(per >= 1)
            {
                this.claimBtns.ChangeState(ClaimBtns.ClaimBtnState.Active);
            }
        }
        else
        {
            gauge.fillAmount = 0;
        }
        
    }
}
