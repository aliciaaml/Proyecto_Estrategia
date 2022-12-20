using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    Pathfinding pathfinding;
    private Vector3 shootDir;
    private int damage;
    private PathNode destinationNode;

    void Start()
    {
        pathfinding = Pathfinding.Instance;
    }

    public void SetUp(Vector3 shootDir, PathNode node)
    {
        this.shootDir = shootDir;
        damage = 20;
        destinationNode = node;

        //En caso de poner un sprite y querer cambiar su orientacion:
        //////////////
        //shootDir = shootDir.normalized;
        //float n = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        //if (n < 0) n += 360;
        //transform.eulerAngles = new Vector3(0, 0, n);
        //////////////
        Destroy(gameObject, 5f);
    }

    
    void Update()
    {
        float moveSpeed = 100f;
        transform.position += shootDir * moveSpeed * Time.deltaTime;

        //pathfinding.GetGrid().GetXY(transform.position + shootDir, out int x, out int y);
        //if (pathfinding.GetNode(x, y) == destinationNode)
        //{
        //    Destroy(gameObject);
        //}
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
