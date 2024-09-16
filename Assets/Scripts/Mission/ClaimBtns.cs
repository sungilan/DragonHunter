using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaimBtns : MonoBehaviour
{
    public enum ClaimBtnState
    {
        None = -1,
        Active = 0,
        InActive,
        Selected
    }

    public GameObject[] arrBtns;
    [SerializeField]
    private ClaimBtnState currentState = ClaimBtnState.None;
    public void ChangeState(ClaimBtnState state)
    {
        foreach (var btn in this.arrBtns)
        {
            btn.SetActive(false);
        }
        this.arrBtns[(int)state].SetActive(true);
    }
}
