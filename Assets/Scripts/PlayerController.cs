using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public AudioClip musicClipOne;
    public AudioClip musicClipTwo;
    public AudioSource musicSource;
    public AudioSource musicSource2;
    public AudioClip coinClip;

    public float playerY;
    public float playerX;

    private Rigidbody2D rb2d;
    public float speed;
    public float jumpForce;
    private bool facingRight = true;
    Animator anim;

    public Text scoreText;
    public Text livesText;
    public Text loseText;
    public Text winText;
    public Text timerText;
    public Text speedText;

    private int score;
    private int lives;
    public int state;
    private int flag;
    private int newFlag;

    private float timeLeft;
    private float gameOver;
    private int timeLeftInt; 

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        score = 0;
        lives = 3;
        flag = 0;
        newFlag = 0;
        gameOver = 0;
        loseText.text = "";
        winText.text = "";
        speedText.text = "";

        timeLeft = 30.0f;

        setText();

        musicSource.clip = musicClipOne;
        musicSource2.clip = coinClip;
        musicSource.Play();
        musicSource.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        //code to control timer
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
            gameOver = 1;
        if (timeLeft < 60 && timeLeft > 0)
        {
            timeLeftInt = (int)timeLeft;
            timerText.text = "Time Left: " + timeLeftInt.ToString();
        }

        if(gameOver == 1)
        {
            Destroy(this);
            Destroy(gameObject);
            loseText.text = "YOU LOSE";
        }

        //lets you quit game
        if (Input.GetKey("escape"))
            Application.Quit();
        //starts run animation when <- or -> are pressed
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))&& ((state != 2)))
        {
            anim.SetInteger("State", 1);
            state = 1;
        }
        if((Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)) && ((state != 2)))
        {
            anim.SetInteger("State", 0);
            state = 0;
        }
        //starts jump animation when up arrow pressed
        if(Input.GetKey(KeyCode.UpArrow))
        {
            anim.SetInteger("State", 2);
            state = 2;
        }
        /*
        if(Input.GetKeyUp(KeyCode.UpArrow))
        {
            anim.SetInteger("State", 3);
            state = 3;
        }*/

        if(score == 8 && flag == 0)
        {
            winText.text = "You Win!";
            timerText.text = "";
            timeLeft = 100f;
            flag = 1;
        }

        if (flag == 1)
        {
            musicSource.Stop();
            musicSource.clip = musicClipTwo;
            musicSource.Play();
            musicSource.loop = true;
            flag = 3;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector2 Scaler = transform.localScale;
        Scaler.x = Scaler.x * -1;
        transform.localScale = Scaler;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        Vector2 movement = new Vector2(moveHorizontal, 0);

        rb2d.AddForce(movement * speed);

        //flip code
        if (facingRight == false && moveHorizontal > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveHorizontal < 0)
        {
            Flip();
        }


    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.tag == "Ground")
        {
            if(Input.GetKey(KeyCode.UpArrow))
            {
                rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
        }
    }

    //resets jump anim to idle upon colliding with ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Ground")
        {
            anim.SetInteger("State", 0);
            state = 0;
        }


    }

    private void OnTriggerEnter2D(Collider2D other)
    {     
         //when coin encountered, 
        if (other.gameObject.CompareTag("coin"))
        {
            musicSource2.Play();
            other.gameObject.SetActive(false); //deactivate coin
            score = score + 1; //increment score
            setText();
        }
        //when powerup encountered
        else if(other.gameObject.CompareTag("powerup"))
        {
            speed = 16;
            other.gameObject.SetActive(false); //deactivate powerup
            speedText.text = "Speed Up: Active!";
        }
            //when enemy encountered
        else if (other.gameObject.CompareTag("slime"))
        {
            other.gameObject.SetActive(false); //deactive enemy
            lives = lives - 1;  //decrement lives
            setText();
        }
        //kill player if lives reach 0
        if (lives == 0)
        {
            Destroy(this);
            Destroy(gameObject);
            loseText.text = "YOU LOSE";
        }

        //transfers to level 2 if 4 coins obtained, reset lives
        if (score == 4 && newFlag == 0) 
        {
            transform.position = new Vector2(playerX, playerY);
            lives = 3;
            timeLeft = 25.0f;
            setText();
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            speedText.text = "";
            speed = 8;
            newFlag = 1;
        }
    }

    void setText()
    {
        scoreText.text = "Score: " + score.ToString();
        livesText.text = "Lives: " + lives.ToString();
    }

}
