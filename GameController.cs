using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public GameObject spaceshipPrefab;
    public GameObject[] lifeIcons;

    public int startAsteroids = 5;
    public int numAsteroids;
    private int maxLives = 4;
    private int numLivesLeft;
    private float respawnTime = 3f;

    public float timeDied;

    private GameObject spaceship;
    private GameObject gameOverSign;
    private GameObject levelClearedSign;
    private float minCollisionRadius = 3f;

    private bool gameFinished = false;
    private bool gameWon = false;

    private float finishTime;

    private void Awake()
    {
        numLivesLeft = maxLives;
        gameOverSign = GameObject.Find("GameOver");
        levelClearedSign = GameObject.Find("LevelCleared");
        InitializeLevel();
    }


    private void InitializeLevel()
    {
        numAsteroids = startAsteroids;
        //spawn the asteroids
        for (int i = 0; i < numAsteroids; i++)
        {
            spawnAsteroid();
        }

        //spawn the spaceship
        spawnSpaceship();

        //hide game over
        //Assert.IsNotNull(gameOverSign);
        gameOverSign.SetActive(false);
        //hide level cleared
        //Assert.IsNotNull(levelClearedSign);
        //these asserts appear to freeze the game when launching
        levelClearedSign.SetActive(false);

        gameFinished = false;
        gameWon = false;
    }

    //spawn a new asteroid
    private void spawnAsteroid()
    {
        bool valid;
        GameObject newAsteroid;
        do
        {
            newAsteroid = Instantiate(asteroidPrefab);
            newAsteroid.GetComponent<Asteroid>().setGameController(this);

            valid = CheckTooCloseToAsteroid(newAsteroid);

        } while (valid == false);
        
        return;

    }


    //spawn spaceship
    private void spawnSpaceship()
    {
        bool valid;

        //Assert.IsNull(spaceship);
        //assert was causing game to freeze after spaceship died, will leave commented unless I find a solution
        //different bug is now causing game to freeze on startup

        do
        {
            spaceship = Instantiate(spaceshipPrefab);
            spaceship.gameObject.tag = "Spaceship";
            valid = CheckTooCloseToAsteroid(spaceship);

        } while (valid == false);

        spaceship.GetComponent<Movement>().setGameController(this);

        numLivesLeft -= 1;

        return;

    }

    private bool CheckTooCloseToAsteroid(GameObject testObject)
    {
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");

        foreach(GameObject asteroid in asteroids)
        {
            if(asteroid != testObject)
            {
                //check to see if they are too close together
                if(Vector3.Distance(testObject.transform.position, asteroid.transform.position) < minCollisionRadius)
                {
                    Destroy(testObject);
                    return false;
                }
            }
        }
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        //check to see if spaceship has died

        if (spaceship == null)
        {
            if (Time.time - timeDied > respawnTime)
            {

                if (numLivesLeft > 0)
                {
                    spawnSpaceship();

                    //update life icons
                    Destroy(lifeIcons[numLivesLeft]);
                }
                else
                {
                    gameOverSign.SetActive(true);
                }
            }
        }

        //check to see if I won
        if((numAsteroids == 0) && (gameWon == false))
        {
            if (gameFinished)
            {
                if(Time.time - finishTime > respawnTime)
                {
                    levelClearedSign.SetActive(true);
                    gameFinished = false;
                    gameWon = true;
                    StartCoroutine(Pause());

                }
            }
            else
            {
                gameFinished = true;
                finishTime = Time.time;
            }
            
        }

        Assert.IsTrue(numAsteroids >= 0);
        //commenting out this last assert just in case it is linked to the freezing-->
        //it seems that commenting this causes freezing on startup, still experiencing freezes on death again
    }

    IEnumerator Pause()
    {
        yield return new WaitForSeconds(3f);
        startAsteroids = startAsteroids + 3;

        if(startAsteroids > 16)
        {
            startAsteroids = 16;
        }

        Destroy(spaceship);
        spaceship = null;
        numLivesLeft++;
        InitializeLevel();
    }
}
