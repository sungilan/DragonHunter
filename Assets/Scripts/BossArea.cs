using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArea : MonoBehaviour
{
    public CutSceneManager cutSceneManager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !cutSceneManager.hasPlayedCutscene)
        {
            cutSceneManager.PlayBossCutscene();
        }
    }
}
