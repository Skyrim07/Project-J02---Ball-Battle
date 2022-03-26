using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SKCell;
public class UIManager : MonoSingleton<UIManager>
{
    public Animator menuAnim, endMatchAnim, beginMatchAnim, goAnim;
    public void SetNotice(Vector2 pos, string content)
    {
        GameObject go = CommonUtils.SpawnObject(CommonReference.instance.noticePrefab);
        go.transform.SetParent(CommonReference.instance.canvas, true);
        go.transform.position = pos + new Vector2(0,50);
        go.transform.Find("Text").GetComponent<Text>().text = content;

        CommonUtils.InvokeAction(2f, () =>
        {
            CommonUtils.ReleaseObject(go);
        });
    }
    public void UpdatePointText()
    {
        CommonReference.instance.playerPointText.text = FlowManager.playerScore.ToString();
        CommonReference.instance.enemyPointText.text = FlowManager.enemyScore.ToString();
    }
    public void UpdateNameText()
    {
        CommonReference.instance.playerText.text = FlowManager.instance.GetPlayerName();
        CommonReference.instance.enemyText.text = FlowManager.instance.GetEnemyName();
    }
    public void SetMenu(bool open)
    {
        SKAudioManager.instance.PlaySound("click");
        menuAnim.SetBool("Appear", open);
    }
    public void ToggleMenu()
    {
        SKAudioManager.instance.PlaySound("click");
        menuAnim.SetBool("Appear", !menuAnim.GetBool("Appear"));
    }
    public void SetBeginMatchMenu(bool open)
    {
        SKAudioManager.instance.PlaySound("click");
        beginMatchAnim.SetBool("Appear", open);
    }
    public void SetGameOverMenu(bool open)
    {
        SKAudioManager.instance.PlaySound("click");
        goAnim.SetBool("Appear", open);
    }
    public void SetEndMatchMenu(bool open)
    {
        SKAudioManager.instance.PlaySound("click");
        endMatchAnim.SetBool("Appear", open);
    }
    public void ToggleEndMatchMenu()
    {
        SKAudioManager.instance.PlaySound("click");
        endMatchAnim.SetBool("Appear", !endMatchAnim.GetBool("Appear"));
    }
}
