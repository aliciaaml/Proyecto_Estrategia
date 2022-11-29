using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 shootDir;
    private int damage;


    public void SetUp(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        damage = 20;

        //En caso de poner un sprite y querer cambiar su orientacion:
        //////////////
        shootDir = shootDir.normalized;
        float n = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        transform.eulerAngles = new Vector3(0, 0, n);
        //////////////

        //Si hacemos que pueda disparar a cualquier lado:
        //Destroy(gameObject, 5f);  //Para que se destruya la bala despues de 5 seg
    }

    
    void Update()
    {
        float moveSpeed = 100f;
        transform.position += shootDir * moveSpeed * Time.deltaTime;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "TallWall")
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "SmallWall")
        {
            damage = 10;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
