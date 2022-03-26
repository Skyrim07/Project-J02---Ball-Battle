using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SKCell;

public class Player : MonoBehaviour
{
    public int side = 0;
    public PlayerType type = PlayerType.Attacker;
    public PlayerState state = PlayerState.Inactivated;

    public Color side0Color, side1Color;

    [SerializeField] private GameObject visualGO, carrierHighlightGO, detectionGO;
    [SerializeField] private Transform orientIndicator, ballTF;
    [SerializeField] private Image regenerateCircle;

    private Ball ball;
    private SKModelProcessing visualProcessor;
    private Collider cld;
    private Animator anim;
    private Vector3 lastPos, posDelta, oPos;
    private List<Player> playerList;

    private bool isRegenerating;
    private float regenerationTime, rTimer;

    public Player defenderTarget;
    void Start()
    {
        ball = CommonReference.instance.ball.GetComponent<Ball>();
        cld = GetComponent<Collider>();
        visualProcessor = visualGO.GetComponent<SKModelProcessing>();
        anim = visualGO.GetComponent<Animator>();
        visualProcessor.color = side==0? side0Color : side1Color;
        carrierHighlightGO .SetActive(false);
        detectionGO.SetActive(type == PlayerType.Defender);
        playerList = side == 0 ? FieldManager.instance.players_Side_0 : FieldManager.instance.players_Side_1;

        transform.rotation= Quaternion.Euler(transform.rotation.eulerAngles.x, side==0?0:180, transform.rotation.eulerAngles.z);

        state = type == PlayerType.Defender?PlayerState.Standby: PlayerState.ActiveIdle;
        lastPos = transform.position;
        oPos = transform.position;

        Inactivate(CommonReference.SPAWN_TIME_ATTACKER);
    }

    void Update()
    {
        ActiveUpdate();
        InactiveUpdate();

        posDelta = transform.position - lastPos;    
        lastPos = transform.position;
    }

    private void InactiveUpdate()
    {
        if (state == PlayerState.Inactivated)
        {
            if (isRegenerating)
            {
                rTimer += Time.deltaTime;
                regenerateCircle.fillAmount = rTimer / regenerationTime;
                if (regenerateCircle.fillAmount >= 1)
                {
                    isRegenerating = false;
                    regenerateCircle.fillAmount = 0;
                }
            }
        }
        if (type == PlayerType.Attacker)
        {
        }
        else //defender
        {
            if (state == PlayerState.Inactivated)
            {
                float dist = Vector3.Distance(oPos, transform.position);
                if (dist > 0.01)
                {
                    MoveTo(oPos, CommonReference.SPEED_RETURN_DEFENDER * Time.deltaTime);
                }
                else
                {
                    if (FlowManager.currentTime > CommonReference.TIME_LIMIT - 15.0f)
                        Activate();
                    orientIndicator.gameObject.SetActive(false);
                }
            }
        }
    }
    private void ActiveUpdate()
    {
        if (type == PlayerType.Attacker)
        {
            if (state == PlayerState.ActiveIdle) // if not holding the ball
            {
                if (ball.player == null && !ball.isPassing) // if the ball is free
                {
                    gameObject.layer = CommonReference.LAYER_PLAYER;
                    MoveTo(ball.transform, CommonReference.SPEED_NORMAL_ATTACKER * Time.deltaTime);
                }
                else // if no ball to chase
                {
                    gameObject.layer = CommonReference.LAYER_INACTIVE;
                    int dir = side == 0 ? 1 : -1;
                    MoveTo(new Vector3(transform.position.x, transform.position.y, side == 0 ? CommonReference.instance.top.position.z : CommonReference.instance.bottom.position.z),
                        CommonReference.SPEED_NORMAL_ATTACKER * Time.deltaTime);
                }
            }
            else if (state == PlayerState.ActiveCarrying)
            {
                gameObject.layer = CommonReference.LAYER_PLAYER;
                Transform opponentGate = side == 0 ? CommonReference.instance.top : CommonReference.instance.bottom;
                if (FlowManager.currentTime > CommonReference.TIME_LIMIT - 15.0f)
                    MoveTo(opponentGate, CommonReference.SPEED_NORMAL_ATTACKER * Time.deltaTime);
                else
                    MoveTo(opponentGate, CommonReference.SPEED_CARRYING_ATTACKER * Time.deltaTime);
                ball.transform.position = ballTF.position;
               // ball.transform.Translate(posDelta);
            }

        }
        else //defender
        {
            if(state == PlayerState.Standby)
            {

            }
            else if (state == PlayerState.ActiveChasing)
            {
                if (defenderTarget == null || defenderTarget.state == PlayerState.Inactivated)
                {
                    detectionGO.SetActive(true);
                    state = PlayerState.Standby;
                    return;
                }

                MoveTo(defenderTarget.transform, CommonReference.SPEED_NORMAL_DEFENDER * Time.deltaTime);
            }
        }
    }
    private void MoveTo(Vector3 targetPos, float delta)
    {
        Vector3 dir = targetPos - transform.position;
        int sign = dir.z >= 0 ? 1 : -1;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, angle, 0), 0.3f);
        transform.Translate(sign*transform.forward * Mathf.Abs(delta));
    }
    private void MoveTo(Transform tf, float delta)
    {
        Vector3 dir = tf.position - transform.position;
        int sign = dir.z>=0?1:-1;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, angle, 0), 0.3f);
        transform.Translate(sign*transform.forward*Mathf.Abs(delta));
    }

    public void OnTouchBall()
    {
        if (type == PlayerType.Attacker)
        {
            if (state == PlayerState.ActiveIdle && ball.player==null)
            {
                SKAudioManager.instance.PlaySound("click2");

                ball.player = this;
                carrierHighlightGO.SetActive(true);
                anim.SetTrigger("Pop");

                CommonUtils.StartProcedure(SKCurve.QuadraticIn, 0.2f, (f) =>
                {
                if (ball && ballTF)
                    ball.transform.position = new Vector3(Mathf.Lerp(ball.transform.position.x, ballTF.position.x, f), ball.transform.position.y, Mathf.Lerp(ball.transform.position.z, ballTF.position.z, f));
                }, (f) =>
                {
                    state = PlayerState.ActiveCarrying;
                });
            }
        }
        else //defender
        {

        }
    }

    public void OnTouchDetectionZone(Player player)
    {
        if (type == PlayerType.Attacker)
            return;
        if (!ball.player == player)
            return;

        anim.SetTrigger("Pop");
        if (state == PlayerState.Standby)
        {
            detectionGO.SetActive(false);
            state = PlayerState.ActiveChasing;
            defenderTarget = player;
        }
    }
    public void OnExitDetectionZone(Player player)
    {
        if (type == PlayerType.Attacker)
            return;
        if (state == PlayerState.ActiveChasing)
        {
            detectionGO.SetActive(true);
            state = PlayerState.Standby;
            defenderTarget = null;
        }
    }
    public void OnCarry()
    {

    }

    public void PassBallToPlayer(Player player)
    {
        ball.player = null;
        float dist = Vector3.Distance(player.ballTF.position, transform.position);
        CommonUtils.StartProcedure(SKCurve.LinearIn, 0.8f, (f) =>
          {
              if (player && player.ballTF)
              {
                  ball.isPassing = true;
                  ball.transform.position = new Vector3(ball.transform.position.x + (player.ballTF.position.x - ball.transform.position.x) * f, ball.transform.position.y, ball.transform.position.z + (player.ballTF.position.z - ball.transform.position.z) * f);
              }
          }, (f) =>
          {
              ball.isPassing = false;
              if (player)
              {
                  player.OnTouchBall();
              }
          });
    }

    public void OnCaught(Player other)
    {
        SKAudioManager.instance.PlaySound("click2");
        if(type == PlayerType.Attacker) //attacker caught by opponent defender
        {
            Inactivate(CommonReference.REACTIVATE_TIME_ATTACKER);
            carrierHighlightGO.SetActive(false);
            PassBallToNearestPlayer();
        }
        else //defender catches opponent attacker
        {
            Inactivate(CommonReference.REACTIVATE_TIME_DEFENDER);
        }
    }

    private void PassBallToNearestPlayer()
    {
        Player targetPlayer = null;
        float minDistance = float.MaxValue;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i] == this)
                continue;

            float dist = Vector3.Distance(transform.position, playerList[i].transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                targetPlayer = playerList[i];
            }
        }
        if (targetPlayer != null && targetPlayer.state != PlayerState.Inactivated)
        {
            PassBallToPlayer(targetPlayer);
        }
        else
        {
            FieldManager.instance.OnAttackerFail();
        }
    }

    public void OnTouchOpponentFence()
    {
        if (ball.player == this)
        {
            FieldManager.instance.SpawnBall();
            //PassBallToNearestPlayer();
        }
        Inactivate();
        GameObject fx= CommonUtils.SpawnObject(CommonReference.instance.puffFx);
        fx.transform.position = transform.position;
        CommonUtils.InvokeAction(2f, () =>
        {
            CommonUtils.ReleaseObject(fx);
        });

        playerList.Remove(this);
        if (side == 0)
        {
            FieldManager.instance.players_Side_0.Remove(this);
        }
        else
        {
            FieldManager.instance.players_Side_1.Remove(this);
        }
        Destroy(gameObject);
    }

    public void OnTouchOpponentGoal()
    {
        Inactivate();
        GameObject fx = CommonUtils.SpawnObject(CommonReference.instance.goalPuffFx);
        fx.transform.position = transform.position;
        CommonUtils.InvokeAction(2f, () =>
        {
            CommonUtils.ReleaseObject(fx);
        });

        playerList.Remove(this);
        Destroy(gameObject);
    }
    public void Activate()
    {
        cld.enabled = true;
        detectionGO.SetActive(type == PlayerType.Defender);
        orientIndicator.gameObject.SetActive(true);
        gameObject.layer = CommonReference.LAYER_PLAYER;

        if(state !=PlayerState.ActiveCarrying)
            state = type==PlayerType.Attacker? PlayerState.ActiveIdle:PlayerState.Standby;
        SetVisualSaturation(1);
        SetVisualAlpha(1.0f);
    }

    public void Inactivate(float time = -1)
    {
        if (ball.player == this)
        {
            ball.player = null;
        }
        cld.enabled = false;
        detectionGO.SetActive(false);
        gameObject.layer = CommonReference.LAYER_INACTIVE;
        state = PlayerState.Inactivated;
        SetVisualSaturation(0);
        SetVisualAlpha(0.5f);
        if (time > 0)
        {
            StartRegenerate(time);
            CommonUtils.InvokeAction(time, () =>
            {
                Activate();
            });
        }
    }

    public void StartRegenerate(float time)
    {
        rTimer = 0;
           isRegenerating = true;
        regenerationTime = time;
    }
    public void SetVisualSaturation(float target)
    {
        float oValue = visualProcessor.saturation;
        CommonUtils.StartProcedure(SKCurve.QuadraticIn, 0.5f, (f) =>
        {
            visualProcessor.saturation = oValue+(target-oValue)*f;
        });
    }
    public void SetVisualAlpha(float target)
    {
        float oValue = visualProcessor.color.a;
        CommonUtils.StartProcedure(SKCurve.QuadraticIn, 0.5f, (f) =>
        {
            visualProcessor.color.a = oValue + (target - oValue) * f;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Border")) //touch the fence
        {
            if((other.transform.position.z > CommonReference.instance.center.position.z && side == 0)||(other.transform.position.z<CommonReference.instance.center.position.z && side == 1))
            {
                //touch the opponent fence
                OnTouchOpponentFence();
            }
        }

        if (other.tag.Equals("Player"))
        {
            Player player = other.GetComponent<Player>();
            if(type == PlayerType.Defender && player.type == PlayerType.Attacker && side!=player.side) //when defender caughts opponent attacker
            {
                OnCaught(player);
                player.OnCaught(this);
            }
        }
    }
}

public enum PlayerType
{
    Attacker,
    Defender
}

public enum PlayerState
{
    Inactivated,
    ActiveIdle,
    ActiveCarrying,
    
    Standby,
    ActiveChasing
}
