using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;

    private Rigidbody2D rb;
    private float thrust = 1f;
    private float turnSpeed = 180;
    private Vector3 shipDirection = new Vector3(0, 1, 0);
    private float maxX = 9.05f;
    private float maxY = 5.05f;
    private float bulletSpeed = 15f;

    private GameController gameController;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.name = "Spaceship";

        //set random position
        transform.position = new Vector3(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY), 0);
    }

    public void setGameController(GameController _gameController)
    {
        gameController = _gameController;
    }

    // Update is called once per frame
    void Update()
    {
        float turnAngle;


        if (Input.GetKey("j")){
            //turn left
            turnAngle = turnSpeed * Time.deltaTime;
            transform.Rotate(0, 0, turnAngle);
            shipDirection = Quaternion.Euler(0, 0, turnAngle) * shipDirection;
        }

        if (Input.GetKey("l"))
        {
            //turn right
            turnAngle = -turnSpeed * Time.deltaTime;
            transform.Rotate(0, 0, turnAngle);
            shipDirection = Quaternion.Euler(0, 0, turnAngle) * shipDirection;        }
        if (Input.GetKey("k"))
        {
            //thrust
            rb.AddForce(shipDirection*thrust);
        }
        if (Input.GetKeyDown("space"))
        {
            //shoot
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation * Quaternion.Euler(0,0,90);
            bullet.GetComponent<Rigidbody2D>().velocity = shipDirection * bulletSpeed;
        }


        //if ship goes off the edge, go to other side
        if (transform.position.x < -maxX)
        {
            transform.position = new Vector2(maxX, transform.position.y);
        }
        else if (transform.position.x > maxX)
        {
            transform.position = new Vector2(-maxX, transform.position.y);
        }
        //out of bounds y
        if (transform.position.y < -maxY)
        {
            transform.position = new Vector2(transform.position.x, maxY);
        }
        else if (transform.position.y > maxY)
        {
            transform.position = new Vector2(transform.position.x, -maxY);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Asteroid")
        {
            gameController.timeDied = Time.time;
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
