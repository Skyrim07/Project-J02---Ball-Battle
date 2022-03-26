using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SKCell;
public class FlowManager : MonoSingleton<FlowManager>
{
    public static GameMode mode;
    public static bool isMatchOver, isGameActive;
    public static int currentMatch;
    public static int playerScore, enemyScore;

    public static float currentTime;

    private void Start()
    {
        currentMatch = 0;
        playerScore = 0;
        enemyScore = 0;
        currentTime = 0;
        isGameActive = false;

        CommonUtils.InvokeAction(0.2f, () =>
        {
            NextMatch();
        });
    }
    public void ResolveFinal()
    {
        CommonReference.instance.go_ScoreText.text = $"{playerScore} : {enemyScore}";
        CommonReference.instance.go_PenaltyButton.SetActive(playerScore == enemyScore);
        if (playerScore > enemyScore)
        {
            CommonReference.instance.go_resultText.text = $"{GetPlayerName()} Wins! Congratulations!";
        }
        else if (playerScore < enemyScore)
        {
            CommonReference.instance.go_resultText.text = $"{GetEnemyName()} Wins! Congratulations!";
        }
        else
        {
            CommonReference.instance.go_resultText.text = $"It seems to be a draw. Proceed to a PENALTY GAME to break the tie!";
        }

        UIManager.instance.SetGameOverMenu(true);
    }
    public void NextMatch()
    {
        currentMatch++;
        if (currentMatch > 5)
        {
            ResolveFinal();
            return;
        }
        CommonReference.instance.bm_TitleText.text = $"Match {currentMatch} Begin!";
        UIManager.instance.SetEndMatchMenu(false);
        UIManager.instance.SetBeginMatchMenu(true);
    }
    public void EndMatch(bool isPlayerWin, MatchEnding ending)
    {
        SKAudioManager.instance.PlaySound("goal");

        isGameActive = false;
        if (ending != MatchEnding.Timeup)
        {
            playerScore += isPlayerWin ? 1 : 0;
            enemyScore += isPlayerWin ? 0 : 1;
        }

        CommonReference.instance.enemyBar.slider.SetValue(0);
        CommonReference.instance.playerBar.slider.SetValue(0);

        UIManager.instance.UpdatePointText();
        SetupEndMatchMenu(isPlayerWin, ending);
    }

    public void SetupEndMatchMenu(bool isPlayerWin, MatchEnding ending)
    {
        string pName = GetPlayerName();
        string eName = GetEnemyName();

        CommonReference.instance.em_TitleText.text = ending==MatchEnding.Timeup? $"Match {currentMatch} Draw!" : $"Match {currentMatch} End£¡";
        CommonReference.instance.em_CauseText.text = ending == MatchEnding.Goal ? "Because of GOAL" : (ending == MatchEnding.AttackerLost ? "Because of NO ATTACKER LEFT":"Because of TIME UP");
        if (ending != MatchEnding.Timeup)
        {
            CommonReference.instance.em_loserNameText.text = isPlayerWin ? eName : pName;
            CommonReference.instance.em_winnerNameText.text = isPlayerWin ? pName : eName;
        }
        else
        {
            CommonReference.instance.em_loserNameText.text = "--";
            CommonReference.instance.em_winnerNameText.text = "--";
        }

        UIManager.instance.SetEndMatchMenu(true);
    }
    public string GetPlayerName()
    {
        string role = FieldManager.instance.isPlayerAttack ? "(Attacker)" : "(Defender)";
        return "Player 1 " + role;
    }

    public string GetEnemyName()
    {
        string role = FieldManager.instance.isPlayerAttack ? "(Defender)" : "(Attacker)";
        return (mode == GameMode.PVE ? "Enemy AI " : "Player 2 ") + role;
    }

    public void TogglePvP(bool isOn)
    {
        mode = isOn ? GameMode.PVP : GameMode.PVE;
    }
    public void TogglePvE(bool isOn)
    {
        mode = isOn ? GameMode.PVE : GameMode.PVP;
    }

    public void LoadHomeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
    }

    public void LoadPenaltyGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Penalty");
    }
}

public enum MatchEnding
{
    Goal,
    AttackerLost,
    Timeup
}
