using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject asteroidPrefab;
    public GameObject asteroidExplosionPrefab;

    private GameController gameController;

    private float maxSpeed = 2.5f;
    private float maxX = 10.15f;
    private float maxY = 6.15f;

    private int health = 3;
    public int scale;  //size of the asteroid
    private int maxScale = 3;

    public float childAsteroidOffset = 1f;
    private float angleOffset = 30f;

    private void Awake()
    {
        scale = maxScale;
        rb = GetComponent<Rigidbody2D>();
        gameObject.tag = "Asteroid";
        gameObject.name = "Asteroid";

        //set random position
        transform.position = new Vector3(Random.Range(-maxX, maxX),Random.Range(-maxY, maxY),0);

        //set random velocity
        rb.velocity = Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(Random.Range(0, maxSpeed), 0f, 0f);

    }

    public void setGameController(GameController _gameController)
    {
        gameController = _gameController;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //prevent velocity from going over maxSpeed
        if(rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        //if asteroid goes off the edge, go to other side
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



    public void takeDamage()
    {
        //decrement asteroid health
        health -= 1;
        if(health == 0)
        {
            Die();
        }
    }
    private void Die()
    {
        GameObject asteroidExplosion = Instantiate(asteroidExplosionPrefab);
        asteroidExplosion.transform.position = transform.position;
        ParticleSystem partSys = asteroidExplosion.GetComponent<ParticleSystem>();
        partSys.Stop();

        var main = partSys.main;

        if ((scale < 3) && (scale > 0)) 
        {
            main.startSize = scale;
        }
        else if (scale == 0)
        {
            main.startSize = 0.5f;
        }

        main.simulationSpeed = 1 * (maxScale - scale + 1);
        partSys.Play();


        if (scale > 0)
        {
            spawnChildAsteroids();
            //update the number of asteroids(+4)
            gameController.numAsteroids += 4;

        }

        //update the number of asteroids(-1)
        gameController.numAsteroids -= 1;


        Destroy(gameObject);
    }

    private void spawnChildAsteroids()
    {
        Vector2[] newDirection = new Vector2[4];
        newDirection[0] = new Vector2(1, 0);
        newDirection[1] = new Vector2(-1, 0);
        newDirection[2] = new Vector2(0, 1);
        newDirection[3] = new Vector2(0, -1);

        float randAngle = Random.Range(0, 360);

        for (int i = 0; i < 4; i++)
        {
            GameObject newAsteroid = Instantiate(asteroidPrefab);
            newAsteroid.GetComponent<Asteroid>().setGameController(gameController);


            Asteroid asteroidHandle = newAsteroid.GetComponent<Asteroid>();

            newDirection[i] = Quaternion.Euler(0, 0, randAngle+Random.Range(-angleOffset,angleOffset)) * newDirection[i];
            newAsteroid.transform.position = transform.position + (Vector3)(newDirection[i] * childAsteroidOffset);
            newAsteroid.transform.localScale = transform.localScale / 2;
            asteroidHandle.scale = scale - 1;
            asteroidHandle.health = scale;
            asteroidHandle.childAsteroidOffset = childAsteroidOffset / 2;

            Rigidbody2D childRb = newAsteroid.GetComponent<Rigidbody2D>();
            childRb.mass = rb.mass / 8;
            childRb.AddForce((Vector3) newDirection[i] * childAsteroidOffset * childAsteroidOffset * childAsteroidOffset * 10);
        }
    }
}
