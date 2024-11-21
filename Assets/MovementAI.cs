using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class MovementAI : MonoBehaviour
{
    public GameObject Target;
    Vector3 TargetPosition;
    public float TracingRange=10f;
    public float speed = 1f;
    public float Health;
    public float Strength=1f;
    public float Damage=1f;
    public float Range=1f;
    float AttackCd;
    float CurrentHealth;
    public float AttackSpeed;
    Rigidbody2D Rigidbody;
    Animator animator;
    float perlinValue;
    public GameObject[] UI;
    string EnemyTag;
    float TraceCD,EscapeCD;
    GameManager gameManager;
    float Size;
    Animator anim;
    public enum TraceMode
    {
        Normal,
        Flying,
        Ranger,
        Bullets,
    }
    public enum ActMode
    {
        Idle,
        Chasing,
        Attack,
        Escaping,
    }
    public TraceMode myTraceMode;
    public ActMode myActMode;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Size = GetComponent<CircleCollider2D>()? GetComponent<CircleCollider2D>().radius*transform.localScale.x : 0;
        TraceCD = 2;
        GetComponent<SpriteRenderer>().sortingOrder += Random.Range(0, 10);
        gameManager = FindObjectOfType<GameManager>();
        
        if (tag == "GreenTeam")
        {
            if (myTraceMode != TraceMode.Bullets)
            {
                GameObject ui = Instantiate(UI[0]);
                ui.transform.position = transform.position + Vector3.up;
                ui.transform.SetParent(transform);
                gameManager.GreenTeam.Add(gameObject);
            }
            EnemyTag = "RedTeam";
        }
        else
        {
            if (myTraceMode != TraceMode.Bullets)
            {
                GameObject ui = Instantiate(UI[1]);
                ui.transform.position = transform.position + Vector3.up;
                ui.transform.SetParent(transform);
                gameManager.RedTeam.Add(gameObject);
            }
            EnemyTag = "GreenTeam";
        }
        if (myTraceMode == TraceMode.Bullets)
        {
            gameObject.tag = "Untagged";
        }
        CurrentHealth = Health;
        Rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
       
        if (Target)
        {
            TraceCD = 2;
            if (AttackCd >= 0)
            {
                AttackCd -= Time.fixedDeltaTime;
            }
            TargetPosition = Target.transform.position;
            
        }
        switch (myTraceMode)
        {
            case TraceMode.Normal:
                NormalAi();
                break;
            case TraceMode.Flying:
                break;
            case TraceMode.Ranger:
                RangerAi();
                break;
            case TraceMode.Bullets:
                Collider();
                break;
        }
        
    }
    void NormalAi()
    {
        switch (myActMode)
        {
            case ActMode.Idle:


                if (!Target)
                {
                    if (animator) animator.SetBool("running", false);
                    TraceCD += Time.fixedDeltaTime;
                    if (TraceCD > 1)
                    {
                        SetTarget(FindNearestObjectWithTag(transform.position, EnemyTag, TracingRange));
                        TraceCD = 0;
                        if (Target == null)
                        {
                            if (tag == "GreenTeam")
                            {
                                Target = gameManager.GreenTeamTarget;
                            }
                            else
                            {
                                Target = gameManager.RedTeamTarget;
                            }
                        }
                    }
                }
                
                if (Target)
                {
                    myActMode = ActMode.Chasing;
                }
                else
                {
                    Vector2 T = (tag == "GreenTeam" ? gameManager.GreenCenter : gameManager.RedCenter);
                    if (Vector2.Distance(transform.position, T) > 2)
                    {
                        if (Rigidbody.velocity.magnitude < speed / 1.5f)
                            Rigidbody.AddForce(GetDirection(T) * speed);
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * ((T.x > transform.position.x) ? -1 : 1), Mathf.Abs(transform.localScale.y), 0);
                        if (animator) animator.SetBool("running", true);
                    }
                }
                break;
            case ActMode.Chasing:
                if (Target)
                {
                    if (animator) animator.SetBool("running", true);
                    if (Rigidbody.velocity.magnitude < speed / 1.5f)
                        Rigidbody.AddForce(GetDirection(TargetPosition) * speed);
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * ((TargetPosition.x > transform.position.x) ? -1 : 1), Mathf.Abs(transform.localScale.y), 0);
                    if (Vector3.Distance(TargetPosition, transform.position) <= Range + Size)
                    {
                        if (AttackCd <= 0) myActMode = ActMode.Attack;
                    }

                }
                else
                {
                    myActMode = ActMode.Idle;
                }
                break;
            case ActMode.Attack:
                if (Target)
                {
                    Target.GetComponent<MovementAI>().GetHit(Damage, GetDirection(TargetPosition), Strength, gameObject);
                    AttackCd = AttackSpeed;
                    myActMode = ActMode.Idle;
                }
                else
                {
                    myActMode = ActMode.Idle;
                }

                break;
        }
    }
    void RangerAi()
    {
        float distance = Vector3.Distance(TargetPosition, transform.position);
        switch (myActMode){
            case ActMode.Idle:
                if (!Target)
                {
                    if (animator) animator.SetBool("running", false);
                    TraceCD += Time.fixedDeltaTime;
                    if (TraceCD > 1)
                    {
                        SetTarget(FindNearestObjectWithTag(transform.position, EnemyTag, TracingRange));
                        TraceCD = 0;
                        if (Target == null)
                        {
                            if (tag == "GreenTeam")
                            {
                                Target = gameManager.GreenTeamTarget;
                            }
                            else
                            {
                                Target = gameManager.RedTeamTarget;
                            }
                        }
                    }
                }

                if (Target)
                {
                    myActMode = ActMode.Chasing;
                }
                else
                {
                    Vector2 T = (tag == "GreenTeam" ? gameManager.GreenCenter : gameManager.RedCenter);
                    if (Vector2.Distance(transform.position, T) > 0.5f)
                    {
                        if (Rigidbody.velocity.magnitude < speed / 1.5f)
                            Rigidbody.AddForce(GetDirection(T) * speed);
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * ((T.x > transform.position.x) ? -1 : 1), Mathf.Abs(transform.localScale.y), 0);
                        if (animator) animator.SetBool("running", true);
                    }
                }
                break;
            case ActMode.Chasing:
                if (Target)
                {
                    if (distance > Range)
                    {
                        if (animator) animator.SetBool("running", true);
                        if (Rigidbody.velocity.magnitude < speed / 1.5f)
                            Rigidbody.AddForce(GetDirection(TargetPosition) * speed);
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * ((TargetPosition.x > transform.position.x) ? -1 : 1), Mathf.Abs(transform.localScale.y), 0);
                    }
                    if (distance <= Range)
                    {
                        Rigidbody.velocity = Rigidbody.velocity / 2;
                        //turn in to escape mode if the target is too close
                        if (distance <= Range / 3 && Random.Range(0, 3) == 0)
                        {
                        myActMode = ActMode.Escaping;
                        break;
                        }
                        if (AttackCd <= 0) myActMode = ActMode.Attack;
                    }
                    
                }
                else
                {
                    myActMode = ActMode.Idle;
                }
                break;
            case ActMode.Attack:
                if (Target)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * ((TargetPosition.x > transform.position.x) ? -1 : 1), Mathf.Abs(transform.localScale.y), 0);
                    anim.SetTrigger("attack");
                    AttackCd = AttackSpeed;
                    myActMode = ActMode.Idle;
                }
                else
                {
                    myActMode = ActMode.Idle;
                }

                break;
            case ActMode.Escaping:
                if (Target)
                {
                    EscapeCD += Time.fixedDeltaTime;
                    if (animator) animator.SetBool("running", true);
                    //GameObject TeamPosition = FindNearestObjectWithTag(transform.position, transform.tag, TracingRange);
                    Vector2 TeamPosition = (tag == "GreenTeam" ? gameManager.GreenCenter : gameManager.RedCenter);
                    if (Vector2.Distance(transform.position, TeamPosition)>3)
                    {
                        if (Rigidbody.velocity.magnitude < speed * 2)
                            Rigidbody.AddForce(GetDirection(TeamPosition) * speed * 3f);
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * ((TeamPosition.x > transform.position.x) ? -1 : 1), Mathf.Abs(transform.localScale.y), 0);
                    }
                    else
                    {
                        if (Rigidbody.velocity.magnitude < speed * 2)
                            Rigidbody.AddForce(GetDirection(TargetPosition) * -speed * 3f);
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * ((TargetPosition.x > transform.position.x) ? 1 : -1), Mathf.Abs(transform.localScale.y), 0);
                    }
                    
                    if (distance >= Range / 2f||EscapeCD>=3)
                    {
                        myActMode = ActMode.Idle;
                        EscapeCD = 0;
                    }
                }
                else
                {
                    myActMode = ActMode.Idle;
                    EscapeCD = 0;
                }
                break;
        }
    }
    void Collider()
    {
        CurrentHealth -= Time.fixedDeltaTime/5;
        if (Target)
        {
            Target.GetComponent<MovementAI>().GetHit(Damage, GetComponent<Rigidbody2D>().velocity.normalized, Strength, gameObject);
            CurrentHealth -= 1;
            
        }
        if (CurrentHealth <= 0)
        {
            DestroyBehavior();
            CurrentHealth = 0;
        }
    }
    public void RangerAttack()
    {
        GetComponent<Shootable>().Shoot(transform, GetDirection(TargetPosition));
    }
    Vector3 GetDirection(Vector3 t)
    {
        return (t-transform.position).normalized;
    }
    public void GetHit(float Damage, Vector3 Direction, float strength,GameObject target)
    {
        SetTarget(target);
        CurrentHealth -= Damage;
        //Rigidbody.velocity = Vector3.zero;
        float angleOffset = Mathf.Atan2(Direction.y, Direction.x) + Random.Range(-20f, 20f) * Mathf.Deg2Rad; // Offset in radians
        Rigidbody.AddForce((new Vector3(Mathf.Cos(angleOffset), Mathf.Sin(angleOffset), 0)).normalized * strength,ForceMode2D.Impulse);
        if (animator) animator.SetTrigger("gethit");
        if (CurrentHealth < 0)
        {
            DestroyBehavior();
            CurrentHealth = 0;
        }
    }
    public void GetHit()
    {
        CurrentHealth -= 1;
        if (animator) animator.SetTrigger("gethit");
        if (CurrentHealth < 0)
        {
            DestroyBehavior();
            CurrentHealth = 0;
        }
    }
    public void DestroyBehavior()
    {
        if (myTraceMode == TraceMode.Bullets)
        {
            Destroy(gameObject);
        }
        if (tag == "RedTeam")
        {
            gameManager.addMoney(2);
        }
        if (tag == "GreenTeam")
        {
            gameManager.GreenTeam.Remove(gameObject);
        }
        else
        {
            gameManager.RedTeam.Remove(gameObject);
        }
        Destroy(gameObject);
    }
    
    void SetTarget(GameObject target)
    {
        Target = target;
        if (target != null)
        if (tag == "GreenTeam")
            gameManager.GreenTeamTarget = target;
        else
            gameManager.RedTeamTarget = target;
    }
    /// <summary>
    /// Finds the nearest GameObject with the specified tag.
    /// </summary>
    /// <param name="currentPosition">The position from which to search.</param>
    /// <param name="tag">The tag to search for.</param>
    /// <returns>The nearest GameObject with the specified tag, or null if none are found.</returns>
    public static GameObject FindNearestObjectWithTag(Vector3 currentPosition, string tag, float range)
    {
        // Find all objects with the specified tag
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

        // Handle case where no objects are found
        if (objectsWithTag.Length == 0)
            return null;

        GameObject nearestObject = null;
        float shortestDistance = Mathf.Infinity;

        // Iterate through all objects to find the nearest one
        foreach (GameObject obj in objectsWithTag)
        {
            float distance = Vector3.Distance(currentPosition, obj.transform.position);
            if(distance!=0)
            if (distance < shortestDistance&&distance<= range)
            {
                shortestDistance = distance;
                nearestObject = obj;
            }
        }

        return nearestObject;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == EnemyTag) SetTarget(collision.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == EnemyTag) SetTarget(collision.gameObject);
    }
}
