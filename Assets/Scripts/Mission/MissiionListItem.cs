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
    private int curDoingVal;
    private int curGoalVal;

    public void Init(int id, string missionSpriteName, string missionName, string rewardSpriteName, int rewardVal, string hexColor, int goalVal, int doingVal = 0)
    {
        this.id = id;
        this.icoMission.sprite = AssetManager.Instance.atlas.GetSprite(missionSpriteName);
        this.txtMissionName.text = missionName;
        this.icoReward.sprite = AssetManager.Instance.atlas.GetSprite(rewardSpriteName);
        this.txtProcessing.text = $"{doingVal} / {goalVal}";
        curDoingVal = doingVal;
        curGoalVal = goalVal;

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
    public void UpdateUI()
    {
        // 진행도 갱신
        txtProcessing.text = $"{curDoingVal} / {curGoalVal}";

        // 게이지 업데이트
        if (curDoingVal > 0)
        {
            float per = (float)curDoingVal / curGoalVal;
            gauge.fillAmount = per;

            // 보상 받을 수 있는지 상태 업데이트
            if (per >= 1)
            {
                claimBtns.ChangeState(ClaimBtns.ClaimBtnState.Active);
            }
        }
        else
        {
            gauge.fillAmount = 0;
            claimBtns.ChangeState(ClaimBtns.ClaimBtnState.InActive); // 미션 진행 중이 아니면 비활성화
        }
    }

}
