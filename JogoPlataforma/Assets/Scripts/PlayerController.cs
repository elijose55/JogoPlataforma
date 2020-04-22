using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
 
 	public CharacterController2D.CharacterCollisionState2D flags;
 	public float walkSpeed = 4.0f;     // Depois de incluido, alterar no Unity Editor
 	public float jumpSpeed = 8.0f;     // Depois de incluido, alterar no Unity Editor
    public float doubleJumpSpeed = 6.0f; //Depois de incluido, alterar no Editor
 	public float gravity = 9.8f;       // Depois de incluido, alterar no Unity Editor
    public bool doubleJumped; // informa se foi feito um pulo duplo

 	public bool isGrounded;		// Se está no chão
 	public bool isJumping;		// Se está pulando
    public bool isFalling;      // Se estiver caindo
    public bool isFacingRight;      // Se está olhando para a direita

    public bool isTaunting;     // Se estiver comemorando ou alguma acao especial
    public bool isHurt;     // Se machucado
    public bool isDead;     // Se morreu/perdeu
    public LayerMask mask;  // para filtrar os layers a serem analisados

    public LayerMask maskM;  // mask da plataforma sideways
    private Animator anim;

    private Animator checkpointAnimator;
    private Animator victoryAnimator;

 	private Vector3 moveDirection = Vector3.zero; // direção que o personagem se move
 	private CharacterController2D characterController;	//Componente do Char. Controller
    public AudioClip coin;
    public AudioClip checkpoint;
    public AudioClip victoryClip;
    public AudioClip death;
    public AudioClip revive;
    public GameObject lives_text;
    public GameObject checkpoint_text;
    public GameObject points_text;
    public GameObject RestartButton;
    public GameObject HomeButton;
    public GameObject RestartButtonSmall;
    public GameObject HomeButtonSmall;
    private Transform PlayerTransform;
    public Transform Checkpoint1;
    public Transform Checkpoint0;

    public bool pause;
    public bool victory;

    public int points;
    public int lives;
    public int checkpointNumber;

    private Animator animator;
    private AudioSource audioSource1;
    private AudioSource audioSource2;
    private AudioSource audioSource3;
    private AudioSource audioSourceMain;

    public AudioClip[] footstepsSounds;
    public bool step;
    void Start()
    {
        animator = GetComponent<Animator>();
        AudioSource[] audios = GetComponents<AudioSource>();
        audioSource1 = audios[0]; //wind audio source
        audioSource2 = audios[1]; //intro audio source
        audioSource3 = audios[2]; //birds audio source
        audioSourceMain = audios[3]; //footsteps audio source
        audioSourceMain.volume = 0.35f;

        characterController = GetComponent<CharacterController2D>(); //identif. o componente
        isFacingRight = true;
        
        checkpointNumber = 0;
        points = 0;
        lives = 3;

        step = false;

        victory = false;
        pause = false;
        isDead = false;

        PlayerTransform = GameObject.Find("Player").transform;   

        lives_text = GameObject.Find("Lives_text");
        checkpoint_text = GameObject.Find("Checkpoint_text");
        points_text = GameObject.Find("Points_text");
        
        lives_text.GetComponent<Text>().text = "Vidas: " + lives.ToString();
        checkpoint_text.GetComponent<Text>().text = "Checkpoint: " + checkpointNumber.ToString();
        points_text.GetComponent<Text>().text = "Pontos: " + points.ToString();

        RestartButton = GameObject.Find("RestartButton");
        HomeButton = GameObject.Find("HomeButton");
        RestartButtonSmall = GameObject.Find("RestartButtonSmall");
        HomeButtonSmall = GameObject.Find("HomeButtonSmall");

        HomeButton.SetActive(false);
        RestartButton.SetActive(false);
    }

    IEnumerator PassPlatform(GameObject platform) {
        platform.GetComponent<EdgeCollider2D>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        platform.GetComponent<EdgeCollider2D>().enabled = true;
    }

    IEnumerator Timer(float seconds) {
        yield return new WaitForSeconds(seconds);
    }

    IEnumerator victoryWaiter() {
        yield return new WaitForSeconds(11f);
        SceneManager.LoadScene("Intro");
    }

    IEnumerator deathWaiter()
    {
        pause = true;
        yield return new WaitForSeconds(2.5f);
        //Destroy(gameObject);
        if(checkpointNumber == 1){
            PlayerTransform.position = Checkpoint1.position;
            AudioSource.PlayClipAtPoint(revive, this.gameObject.transform.position, 1f);
        }
        else if(checkpointNumber == 0){
            PlayerTransform.position = Checkpoint0.position;
            AudioSource.PlayClipAtPoint(revive, this.gameObject.transform.position, 1f);
        }
        animator.SetTrigger("isAlive");
        pause = false;
        isDead = false;  
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Coins")) {
            points += 1;
            AudioSource.PlayClipAtPoint(coin, this.gameObject.transform.position, 1.0f);
            Destroy(other.gameObject);
            points_text.GetComponent<Text>().text = "Pontos: " + points.ToString();
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Checkpoint")) {
            if (checkpointNumber == 0){
                checkpointAnimator = other.gameObject.GetComponent<Animator>();
                checkpointAnimator.SetTrigger("Animate");
                points += 3;
                checkpointNumber = 1;
                AudioSource.PlayClipAtPoint(checkpoint, this.gameObject.transform.position, 1.0f);
                audioSource2.Play();
                audioSource3.Pause();
                checkpoint_text.GetComponent<Text>().text = "Checkpoint: " + checkpointNumber.ToString();
                points_text.GetComponent<Text>().text = "Pontos: " + points.ToString();
            }
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Victory")) {
            if (!victory){
                pause = true; 
                victory = true;
                victoryAnimator = other.gameObject.GetComponent<Animator>();
                victoryAnimator.SetTrigger("Animate");
                animator.SetBool("Victory", true);
                AudioSource.PlayClipAtPoint(victoryClip, this.gameObject.transform.position, 1.0f);
                audioSource1.Pause();
                audioSource3.Pause();
                audioSource2.volume = 1.0f;
                StartCoroutine(victoryWaiter());            
            }

        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            
            //checkpointAnimator = other.gameObject.GetComponent<Animator>();
            //checkpointAnimator.SetTrigger("Animate");
            //points += 3;
            //GameObject.Find("Victory_text").GetComponent<Text>().text = "WASTED";
            if(!isDead){
                isDead = true;
                lives -= 1;
                animator.SetTrigger("isDead");
                AudioSource.PlayClipAtPoint(death, this.gameObject.transform.position, 1.0f);

                if (lives <= 0){ //Game over
                    lives_text.GetComponent<Text>().text = "Vidas: " + lives.ToString(); 
                    HomeButton.SetActive(true);
                    RestartButton.SetActive(true);
                    HomeButtonSmall.SetActive(false);
                    RestartButtonSmall.SetActive(false);
                    pause = true;
                } 
                else { //volta para o checkpoint
                    lives_text.GetComponent<Text>().text = "Vidas: " + lives.ToString();
                    StartCoroutine(deathWaiter());
                }
            }
        }
        
    }

    IEnumerator stepWaiter()
    {
        yield return new WaitForSeconds(0.3f);
        step = false;
    }


    void Update()
    {
        if(!pause) {
            moveDirection.x = Input.GetAxis("Horizontal"); // recupera valor dos controles
            moveDirection.x *= walkSpeed;

            if(Mathf.Abs(moveDirection.x/walkSpeed) > 0.1 && isGrounded) {
                if(!audioSourceMain.isPlaying && !step) {
                    step = true;
                    audioSourceMain.PlayOneShot(footstepsSounds[Random.Range(0,footstepsSounds.Length)]); //fazer um som de footstep diferente a cada vez
                    StartCoroutine(stepWaiter());
                }
            }

            // Conforme direção do personagem girar ele no eixo Y
            if (moveDirection.x < 0) {
                transform.eulerAngles = new Vector3(0,180,0);
                isFacingRight = false;
            }
            else if (moveDirection.x > 0)
            {
                transform.eulerAngles = new Vector3(0,0,0);
                isFacingRight = true;
            } // se direção em x == 0 mantenha como está a rotação

            if (isGrounded) {		     // caso esteja no chão
                moveDirection.y = 0.0f;    // se no chão nem subir nem descer

                isJumping = false;
                doubleJumped = false; // se voltou ao chão pode faz pulo duplo

                if (Input.GetButton("Jump"))
                {
                    isJumping = true;
                    animator.SetBool("isJumping", isJumping);
                    moveDirection.y = jumpSpeed;
                }
            }

            else {           // caso esteja pulando 
                if(Input.GetButtonUp("Jump") && moveDirection.y > 0)
                { // Soltando botão diminui pulo
                    moveDirection.y *= 0.5f;
                }

                if(Input.GetButtonDown("Jump") && !doubleJumped) // Segundo clique faz pulo duplo
                {
                    moveDirection.y = doubleJumpSpeed;
                    doubleJumped = true;
                }
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 8f, mask);
            if (hit.collider != null && isGrounded) {
                if(Input.GetButtonDown("Jump")) {
                    anim = hit.collider.gameObject.GetComponent<Animator>();
                    anim.SetTrigger("Animate");
                    //moveDirection.y = -jumpSpeed;
                    //StartCoroutine(PassPlatform(hit.transform.gameObject));
                }
            }

            RaycastHit2D hitM = Physics2D.Raycast(transform.position, -Vector2.up, 8f, maskM);
            if (hitM.collider != null && isGrounded) {
                transform.SetParent(hitM.transform);
            } else {
                transform.SetParent(null);
            }


            if(moveDirection.y < 0){
                isFalling = true;
            }
            else {
                isFalling = false;   
            }     


        moveDirection.y -= gravity * Time.deltaTime;	// aplica a gravidade
        characterController.move(moveDirection * Time.deltaTime);	// move personagem	

        flags = characterController.collisionState; 	// recupera flags
        isGrounded = flags.below;				// define flag de chão

        animator.SetFloat("movementX", Mathf.Abs(moveDirection.x/walkSpeed)); // +Normalizado
        animator.SetFloat("movementY", moveDirection.y);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isJumping", isJumping);
        //animator.SetBool("isTaunting", isTaunting);
        animator.SetBool("isHurt", isHurt);
        //animator.SetBool("isDead", isDead);
        animator.SetBool("isFalling", isFalling);

        }
    }
}
