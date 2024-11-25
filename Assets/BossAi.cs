using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class BossAi : MonoBehaviour
{
    MovementAI MovementAI;
    GameManager gameManager;
    // Start is called before the first frame update
    float StateTime;
    Animator animator;
    Collider2D mycollider;
    float rby,z;
    Rigidbody2D rb;
    int teamcount;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        MovementAI = GetComponent<MovementAI>();
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        mycollider = GetComponent<Collider2D>();
    }
    Vector3 GetDirection(Vector3 t)
    {
        return (t - transform.position).normalized;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (MovementAI.tag == "GreenTeam") teamcount = gameManager.GreenTeam.Count;
        else teamcount = gameManager.RedTeam.Count;
        if (MovementAI.myActMode == MovementAI.ActMode.None)
        {
            if (mycollider.isTrigger == false)
            {
                animator.SetTrigger("jump");
                animator.SetBool("fall", false);
                mycollider.isTrigger = true;
                z = 0;
                rby = 0.5f;
            }
            else
            {
                float distance = Vector2.Distance(transform.position, MovementAI.TargetPosition);
                if(MovementAI.myTraceMode== MovementAI.TraceMode.Normal)
                    rb.AddForce(GetDirection(MovementAI.TargetPosition) * MovementAI.speed * 12);
                else
                    rb.AddForce(GetDirection(MovementAI.TargetPosition) * MovementAI.speed * -12);
                if (rby <= 0) animator.SetBool("fall",true);
                z += rby;
                transform.position += Vector3.up * rby;
                rby -= Time.fixedDeltaTime;
                if (z <= 0)
                {
                    rb.velocity = Vector2.zero;
                    transform.position -= Vector3.up * z;
                    mycollider.isTrigger = false;
                    animator.SetBool("fall", false);
                    MovementAI.myActMode = MovementAI.ActMode.Idle;
                }
            }

        }
        else
        {
            switch (MovementAI.myTraceMode)
            {
                case MovementAI.TraceMode.Normal:
                    MovementAI.Range = 1;
                    if (MovementAI.myActMode == MovementAI.ActMode.Chasing)
                    {
                        StateTime += Time.fixedDeltaTime;
                        if (StateTime > 5)
                        {
                            MovementAI.myActMode = MovementAI.ActMode.None;
                            StateTime = 0;
                            if (teamcount <= 1 || Random.Range(0, 2) == 0)
                            {
                                MovementAI.myTraceMode = MovementAI.TraceMode.Ranger;
                            }
                            else
                            {
                                MovementAI.myTraceMode = MovementAI.TraceMode.Healer;
                                MovementAI.Target = null;
                            }
                            print(MovementAI.myTraceMode);
                        }
                    }
                    break;
                case MovementAI.TraceMode.Healer:
                    MovementAI.Range = 8;
                    if (MovementAI.myActMode == MovementAI.ActMode.Escaping)
                    {
                        MovementAI.Target = null;
                        MovementAI.myActMode = MovementAI.ActMode.Idle;
                        MovementAI.myTraceMode = MovementAI.TraceMode.Normal;
                        break;
                    }
                    if (teamcount<= 1)
                    {
                        MovementAI.Target = null;
                        MovementAI.myActMode = MovementAI.ActMode.None;
                        MovementAI.myTraceMode = MovementAI.TraceMode.Normal;
                        break;
                    }
                    if(MovementAI.myActMode == MovementAI.ActMode.Idle|| MovementAI.myActMode == MovementAI.ActMode.Chasing)
                    {
                        StateTime += Time.fixedDeltaTime;
                    }
                    if (StateTime > 2)
                    {
                        MovementAI.myActMode = MovementAI.ActMode.None;
                        MovementAI.Target = null;
                        StateTime = 0;
                        if (Random.Range(0, 2) == 0)
                        {
                            MovementAI.myTraceMode = MovementAI.TraceMode.Ranger;
                        }
                        else
                        {
                            MovementAI.myTraceMode = MovementAI.TraceMode.Normal;
                        }
                    }
                    break;
                case MovementAI.TraceMode.Ranger:
                    MovementAI.Range = 10;
                    if (MovementAI.myActMode == MovementAI.ActMode.Escaping)
                    {
                        MovementAI.myActMode = MovementAI.ActMode.Idle;
                        MovementAI.myTraceMode = MovementAI.TraceMode.Normal;
                    }
                    StateTime += Time.fixedDeltaTime;
                    if (StateTime > 5)
                    {
                        MovementAI.myActMode = MovementAI.ActMode.None;
                        StateTime = 0;
                        if (teamcount <= 1 || Random.Range(0, 2) == 0)
                        {
                            MovementAI.myTraceMode = MovementAI.TraceMode.Normal;
                        }
                        else
                        {
                            MovementAI.Target = null;
                            MovementAI.myTraceMode = MovementAI.TraceMode.Healer;
                        }
                    }
                    break;
            }
        }
    }
}
