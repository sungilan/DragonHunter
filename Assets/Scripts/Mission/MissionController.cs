using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    public RectTransform contents;
    public MissionListItem listItemPrefab;
    public static GameInfo gameInfo;
    private List<MissionListItem> missionListItems = new List<MissionListItem>();


    void Start()
    {
        PlayerPrefs.DeleteAll();
        //Info를 로드
        var json = PlayerPrefs.GetString("game_info");
        if (string.IsNullOrEmpty(json))
        {
            MissionController.gameInfo = new GameInfo();
            MissionController.gameInfo.missionInfos.Add(new MissionInfo(0, 10000));
            var gameInfoJson = JsonConvert.SerializeObject(gameInfo);
            PlayerPrefs.SetString("game_info", gameInfoJson);
        }
        else
        {
            MissionController.gameInfo = JsonConvert.DeserializeObject<GameInfo>(json);
        }

        //데이터 로드
        DataManager.Instance.LoadData<MissionData>("Data/mission_data");
        DataManager.Instance.LoadData<RewardData>("Data/reward_data");
        for (int i = 0; i < 5; i++)
        {
            var listItem = this.CreateListItem();
            missionListItems.Add(listItem);
            int itemId = listItem.id;
            listItem.btnClaim.onClick.AddListener(() => {
                this.Claim(itemId);
            });

            var missionData = DataManager.Instance.GetData<MissionData>(i);
            var rewardData = DataManager.Instance.GetData<RewardData>(missionData.reward_id);
            var missionName = string.Format(missionData.mission_name,missionData.goal_val.ToString("#,###"));
            var hexColor = string.Format("#{0}", rewardData.hex_color);
            var foundMission = MissionController.gameInfo.missionInfos.Find(x => x.id == missionData.id);

            if (foundMission != null)
            {
                listItem.Init(foundMission.id, missionData.sprite_name, missionName, rewardData.sprite_name, missionData.reward_val, hexColor, missionData.goal_val, foundMission.doingVal);
            }
            else
            {
                listItem.Init(missionData.id, missionData.sprite_name, missionName, rewardData.sprite_name, missionData.reward_val, hexColor, missionData.goal_val);
            }
        }

        
    }

    private void Claim(int id)
    {
        var missionData = DataManager.Instance.GetData<MissionData>(id);
        var rewardData = DataManager.Instance.GetData<RewardData>(missionData.reward_id);
        var foundMissionInfo = MissionController.gameInfo.missionInfos.Find(x => x.id == id);
        if (foundMissionInfo != null)
        {
            if(foundMissionInfo.doingVal == missionData.goal_val)
            {
                var foundListItem = missionListItems.Find(x => x.id == foundMissionInfo.id);
                if(foundListItem != null)
                {
                    foundListItem.claimBtns.ChangeState(ClaimBtns.ClaimBtnState.Selected);
                }
            }
        }
    }    

    private MissionListItem CreateListItem()
    {
        var listItemGo = Instantiate(this.listItemPrefab);
        listItemGo.transform.SetParent(contents);
        listItemGo.transform.localScale = Vector3.one;
        return listItemGo.GetComponent<MissionListItem>();
    }
}
