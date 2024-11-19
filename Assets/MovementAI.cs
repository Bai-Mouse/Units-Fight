using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    float AttackCd;
    float CurrentHealth;
    public float AttackSpeed;
    Rigidbody2D Rigidbody;
    Animator animator;
    float perlinValue;
    public GameObject[] UI;
    string EnemyTag;
    float TraceCD;
    GameManager gameManager;
    public enum TraceMode
    {
        Normal,
        Flying,
        Ranger,
    }
    public enum ActMode
    {
        Idle,
        Chasing,
        Attack,
    }
    public TraceMode myTraceMode;
    public ActMode myActMode;
    // Start is called before the first frame update
    void Start()
    {
        TraceCD = 2;
        GetComponent<SpriteRenderer>().sortingOrder += Random.Range(0, 10);
        gameManager = FindObjectOfType<GameManager>();
        if (tag == "GreenTeam")
        {
            GameObject ui = Instantiate(UI[0]);
            ui.transform.position = transform.position+Vector3.up;
            ui.transform.SetParent(transform);
            EnemyTag = "RedTeam";
            gameManager.GreenTeam.Add(gameObject);
        }
        else
        {
            GameObject ui = Instantiate(UI[1]);
            ui.transform.position = transform.position + Vector3.up;
            ui.transform.SetParent(transform);
            EnemyTag = "GreenTeam";
            gameManager.RedTeam.Add(gameObject);
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
            switch (myTraceMode)
            {
                case TraceMode.Normal:NormalAi();
                    break;
                case TraceMode.Flying:
                    break;
                case TraceMode.Ranger:
                    break;
            }
        }
        else
        {
            Rigidbody.velocity = Vector3.zero;
            myActMode = ActMode.Idle;
            if (animator) animator.SetBool("running", false);
            TraceCD+=Time.fixedDeltaTime;
            if (TraceCD > 1)
            {
                SetTarget(FindNearestObjectWithTag(transform.position, EnemyTag,TracingRange));
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
    }
    void NormalAi()
    {
        switch (myActMode)
        {
            case ActMode.Idle:
                if (animator) animator.SetBool("running", false);
                myActMode=ActMode.Chasing;
                break;
            case ActMode.Chasing:
                if (animator) animator.SetBool("running", true);
                if(Rigidbody.velocity.magnitude<speed/1.5f)
                Rigidbody.AddForce(GetDirection(TargetPosition) * speed);
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * ((TargetPosition.x > transform.position.x)?-1 : 1), Mathf.Abs(transform.localScale.y), 0);
                if (Vector3.Distance(TargetPosition, transform.position)<=1)
                {
                    if (AttackCd <= 0) myActMode = ActMode.Attack;
                }
                   break;
            case ActMode.Attack:
                    Target.GetComponent<MovementAI>().GetHit(1, GetDirection(TargetPosition),3,gameObject);
                    AttackCd = AttackSpeed;
                myActMode = ActMode.Idle;
                break;
        }
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
        Destroy(gameObject);
        if (tag == "GreenTeam")
        {
            gameManager.GreenTeam.Remove(gameObject);
        }
        else
        {
            gameManager.RedTeam.Remove(gameObject);
        }
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
