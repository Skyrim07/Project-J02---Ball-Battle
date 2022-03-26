using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SKCell;

public class CommonReference : MonoSingleton<CommonReference>
{
    #region Game Variables
    public static float TIME_LIMIT = 140.0f;
    public static float ENERGY_REGENERATION = 0.5f;
    public static float ENERGY_MAX = 6.0f;
    public static float AI_SPAWN_RATE_ATTACKER = 0.3f;
    public static float AI_SPAWN_RATE_DEFENDER = 0.6f;
    public static int ENERGY_COST_ATTACKER = 2;
    public static int ENERGY_COST_DEFENDER = 3;
    public static float SPAWN_TIME_ATTACKER = 0.5f;
    public static float SPAWN_TIME_DEFENDER = 0.5f;
    public static float REACTIVATE_TIME_ATTACKER = 2.5f;
    public static float REACTIVATE_TIME_DEFENDER = 4.0f;
    public static float SPEED_NORMAL_ATTACKER = 1.5f;
    public static float SPEED_NORMAL_DEFENDER = 1.0f;
    public static float SPEED_CARRYING_ATTACKER = 0.75f;
    public static float SPEED_BALL = 1.5f;
    public static float SPEED_RETURN_DEFENDER = 2.0f;
    public static float DETECTION_RANGE = 0.35f;
    #endregion

    public static int LAYER_FIELD = 6;
    public static int LAYER_PLAYER = 7;
    public static int LAYER_INACTIVE =  8;
    public static int LAYER_BALL =9;

    public Transform center, left, right, bottom, top;
    public Transform playerContainer;
    public GameObject playerPrefab;
    public GameObject ball;
    public GameObject puffFx, goalPuffFx;

    public GameObject noticePrefab;
    public Transform canvas;
    public BlockProgressBar enemyBar, playerBar;
    public Text enemyText, playerText, timeText, enemyPointText, playerPointText, enemyModeText;
    public Text em_CauseText, em_winnerNameText, em_loserNameText, em_TitleText, bm_TitleText;
    public Text go_ScoreText, go_resultText;
    public GameObject go_PenaltyButton;
    public Animator noticeAnim;


    private void Start()
    {
        
    }
}
