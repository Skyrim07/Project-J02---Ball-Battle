using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SKCell;
public class FieldManager : MonoSingleton<FieldManager>
{
    public List<Player> players_Side_0 = new List<Player>();
    public List<Player> players_Side_1 = new List<Player>();

    private SKSlider enemyBar, playerBar;
    private Ball ball;
    private Camera cam;

    public bool isPlayerAttack;

    private float AITimer;
    void Start()
    {
        enemyBar = CommonReference.instance.enemyBar.slider;
        playerBar = CommonReference.instance.playerBar.slider;
        ball = CommonReference.instance.ball.GetComponent<Ball>();
        cam = Camera.main;
    }


    void Update()
    {
        FieldTick();
        AITick();
        EnergyRegeneration();
    }

    public void StartMatch()
    {
        isPlayerAttack = FlowManager.currentMatch % 2 == 1;

        UIManager.instance.UpdatePointText();
        UIManager.instance.UpdateNameText();
        CommonReference.instance.enemyBar.slider.SetValue(0);
        CommonReference.instance.playerBar.slider.SetValue(0);
        CommonReference.instance.enemyModeText.text = FlowManager.mode == GameMode.PVE ? "E" : "P";

        players_Side_0.Clear();
        players_Side_1.Clear();
        FlowManager.currentTime = 0;
        FlowManager.isGameActive = true;

        SpawnBall();
    }

    public void OnClickField(int side)
    {
        if (!FlowManager.isGameActive)
        {
            return;
        }

        BlockProgressBar bar = side == 0 ? CommonReference.instance.playerBar : CommonReference.instance.enemyBar;
        float cost = isPlayerAttack ? (side == 0 ? CommonReference.ENERGY_COST_ATTACKER : CommonReference.ENERGY_COST_DEFENDER) : (side == 0 ? CommonReference.ENERGY_COST_DEFENDER : CommonReference.ENERGY_COST_ATTACKER);

        if (bar.value < cost)
        {
            UIManager.instance.SetNotice(Input.mousePosition, $"You need {(int)cost} energy to deploy!");
            return;
        }
        else
        {
            bar.slider.AddValue(-cost / CommonReference.ENERGY_MAX);
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 100);
        PlayerType type = isPlayerAttack ? (side == 0 ? PlayerType.Attacker : PlayerType.Defender) : (side == 0 ? PlayerType.Defender : PlayerType.Attacker);
        SpawnPlayer(side, type, hit.point);
    }
    public void SpawnBall()
    {
        ball.gameObject.SetActive(true);
        ball.player = null;
        ball.GetComponent<Animator>().Play("BallStart");
        if (isPlayerAttack)
        {
            ball.transform.position = new Vector3(Random.Range(CommonReference.instance.left.position.x + 1, CommonReference.instance.right.position.x - 1),
                ball.transform.position.y,
                Random.Range(CommonReference.instance.center.position.z - 1, CommonReference.instance.bottom.position.z + 1));
        }
        else
        {
            ball.transform.position = new Vector3(Random.Range(CommonReference.instance.left.position.x + 1, CommonReference.instance.right.position.x - 1),
            ball.transform.position.y,
            Random.Range(CommonReference.instance.center.position.z + 1, CommonReference.instance.top.position.z - 1));
        }
    }
    public void SpawnPlayer(int side, PlayerType type, Vector3 pos)
    {
        SKAudioManager.instance.PlaySound("border");

        Player player = Instantiate(CommonReference.instance.playerPrefab).GetComponent<Player>();
        if (side == 0)
        {
            players_Side_0.Add(player);
        }
        else
        {
            players_Side_1.Add(player);
        }
        player.type = type;
        player.side = side;
        player.transform.SetParent(CommonReference.instance.playerContainer, true);
        player.transform.position = new Vector3(pos.x, player.transform.position.y, pos.z);
    }

    public void AITick()
    {
        if (FlowManager.mode != GameMode.PVE)
            return;
        if (!FlowManager.isGameActive)
        {
            return;
        }

        AITimer += Time.deltaTime;
        if (AITimer >= 1.0f)
        {
            AITimer = 0.0f;
            float rand = Random.value;
            float rate = isPlayerAttack ? CommonReference.AI_SPAWN_RATE_DEFENDER : CommonReference.AI_SPAWN_RATE_ATTACKER;
            if (rand < rate)
            {
                AISpawn();
            }
        }
    }

    public void AISpawn()
    {
        if (isPlayerAttack) //AI defends
        {
            List<Player> enemyList = players_Side_0;

            BlockProgressBar bar = CommonReference.instance.enemyBar;
            float cost = CommonReference.ENERGY_COST_DEFENDER;

            if (bar.value < cost)
            {
                return;
            }
            else
            {
                bar.slider.AddValue(-cost / CommonReference.ENERGY_MAX);
            }
            Vector3 pos;
            if (enemyList.Count > 0)
            {
                Player p = enemyList[Random.Range(0, enemyList.Count)];
                pos = new Vector3(p.transform.position.x,
           0,
           Random.Range(CommonReference.instance.center.position.z + 1, CommonReference.instance.top.position.z - 1));
            }
            else
            {
                pos = new Vector3(Random.Range(CommonReference.instance.left.position.x + 1, CommonReference.instance.right.position.x - 1),
            0,
            Random.Range(CommonReference.instance.center.position.z + 1, CommonReference.instance.top.position.z - 1));
            }
            PlayerType type = PlayerType.Defender;
            SpawnPlayer(1, type, pos);
        }
        else //AI attacks
        {
            BlockProgressBar bar = CommonReference.instance.enemyBar;
            float cost = CommonReference.ENERGY_COST_ATTACKER;

            if (bar.value < cost)
            {
                return;
            }
            else
            {
                bar.slider.AddValue(-cost / CommonReference.ENERGY_MAX);
            }
            Vector3 pos = new Vector3(Random.Range(CommonReference.instance.left.position.x + 1, CommonReference.instance.right.position.x - 1),
            0,
            Random.Range(CommonReference.instance.center.position.z + 1, CommonReference.instance.top.position.z - 1));
            PlayerType type = PlayerType.Attacker;
            SpawnPlayer(1, type, pos);
        }
    }
    public void FieldTick()
    {
        if (!FlowManager.isGameActive)
        {
            return;
        }

        FlowManager.currentTime += Time.deltaTime;
        float timeLeft = CommonReference.TIME_LIMIT - FlowManager.currentTime;
        CommonReference.instance.timeText.text = (((int)timeLeft) / 60).ToString("d2") + ":" + (((int)timeLeft) % 60).ToString("d2");
        if (FlowManager.currentTime > CommonReference.TIME_LIMIT)
        {
            OnTimerEnd();
        }
    }

    public void OnGoal(GameObject goal)
    {
        int side = goal.transform.position.z > CommonReference.instance.center.position.z ? 1 : 0;
        if (ball.player != null)
        {
            ball.player.OnTouchOpponentGoal();
        }
        ball.gameObject.SetActive(false);

        ClearAllPlayers();

        FlowManager.instance.EndMatch(side == 1, MatchEnding.Goal);
    }

    private void ClearAllPlayers()
    {
        foreach (var item in players_Side_0)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in players_Side_1)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void OnAttackerFail()
    {
        ClearAllPlayers();
        FlowManager.instance.EndMatch(!isPlayerAttack, MatchEnding.AttackerLost);
    }
    public void OnTimerEnd()
    {
        ClearAllPlayers();
        FlowManager.instance.EndMatch(true, MatchEnding.Timeup);
    }
    public void AddEnergy(int side, float blockValue)
    {
        SKSlider bar = side == 0 ? playerBar : enemyBar;
        bar.AddValue(blockValue / CommonReference.ENERGY_MAX);
    }
    public void EnergyRegeneration()
    {
        if (FlowManager.isGameActive)
        {
            float buff = 1;
            if(FlowManager.currentTime > CommonReference.TIME_LIMIT - 15.0f)
            {
                buff = 2;
            }

            AddEnergy(0, CommonReference.ENERGY_REGENERATION * Time.deltaTime * buff);
            AddEnergy(1, CommonReference.ENERGY_REGENERATION * Time.deltaTime* buff);
        }
    }
}

public enum GameMode
{
    PVP,
    PVE
}
