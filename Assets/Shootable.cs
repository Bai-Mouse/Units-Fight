using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    public GameObject Bullet;
    public float BulletSpeed=5f;
    public void Shoot(Transform owner,Vector2 direciton)
    {

        GameObject b = Instantiate(Bullet);
        b.tag = owner.tag;
        b.transform.position= owner.transform.position;
        if(!b.GetComponent<Rigidbody2D>())
        b.AddComponent<Rigidbody2D>();
        b.GetComponent<Rigidbody2D>().AddForce(direciton * BulletSpeed);
        b.transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(direciton.y, direciton.x)*Mathf.Rad2Deg);
        
    }
}
