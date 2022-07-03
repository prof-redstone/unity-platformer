using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speedGround; //vitesse de deplacement au sol
    public float speedAir;
    public float fHorizontalDamping;
    public float jumpForce;
    public float maxSpeed; //vitesse max pour ne pas passer a travers les collisions 
    public float slidWallSpeed;

    private bool isjumping; //true si je joueur sote 
    public float fallMultiplier = 2.5f; //pour gérer le grand saut et les petits
    public float lowJumpMultiplier = 2f;

    public float fJumpPressedRememberTime; //pour garder en mémoire le saut si on appuie avant de toucher le sol et pouvoir sauter après le contacte
    private float fJumpPressedRemember;
    public float fGroundedRememberTime;
    private float fGroundedRemember;

    public float wallJumpCouldownTime;//couldown
    private float wallJumpCouldown;
    private bool isSlidingWall;

    private bool isWallJumping; 
    
    private bool isRightWall; //pour détecter quand on touche le mur droit
    public Transform rightWallCheckTop;
    public Transform rightWallCheckBottom;
    private bool isLeftWall; //pour détecter quand on touche le mur gauche
    public Transform leftWallCheckTop;
    public Transform leftWallCheckBottom;
    public LayerMask wallLayer; //layer qui sert de collision popur les wallJump. (le meme que groundLayer par defaut)
    public float wallJumpForceX; //les forces à execercer sur le player
    public float wallJumpForceY;

    private float moveInput;

    private Rigidbody2D rb;

    private bool isGrounded; //pour détecter quand on touche le sol
    public Transform groundCheckLeft;
    public Transform groundCheckRight;
    public LayerMask groundLayer; //layer qui sert de collision popur les platformes.


    public bool direction = true; //pour la direction du sprite et l'animation, true = right

    public Sprite PMoveRight; //walk 
    public Sprite PMoveLeft;
    public Sprite PJumpRight; //jump, facing up
    public Sprite PJumpLeft;
    public Sprite PFallRight; //falling 
    public Sprite PFallLeft;

    private BoxCollider2D boxCollider2d;  //collider du perso

    public bool isFreezing = false;
    private float freezTime = 0f;
    private Vector2 MouvFreezTimeSave;
    private float gravityScale;


    //Input Action, new input system

    PlayerControl inputAction;  

    private bool stateJumpKey = false;
    private string eventJumpKey;

    private bool stateDashKey = false;
    private string eventDashKey;

    private bool stateGrabKey = false;
    private string eventGrabKey;
    


    void Awake(){
        inputAction = new PlayerControl();
        inputAction.Player.mouvement.started += ctx => DirectionKeyAction(ctx.ReadValue<float>());
        inputAction.Player.mouvement.canceled += ctx => DirectionKeyAction(ctx.ReadValue<float>());
        inputAction.Player.jump.started += ctx => JumpKeyAction("Press");
        inputAction.Player.jump.canceled += ctx => JumpKeyAction("Release");
        inputAction.Player.dash.started += ctx => DashKeyAction("Press");
        inputAction.Player.dash.canceled += ctx => DashKeyAction("Release");
        inputAction.Player.grab.started += ctx => GrabKeyAction("Press");
        inputAction.Player.grab.canceled += ctx => GrabKeyAction("Release");
        inputAction.Enable();
    }

    
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
    }

    void FixedUpdate() {
        
        isGrounded = Physics2D.OverlapArea(groundCheckLeft.position, groundCheckRight.position, groundLayer); 
        isRightWall = Physics2D.OverlapArea(rightWallCheckTop.position, rightWallCheckBottom.position, wallLayer); 
        isLeftWall = Physics2D.OverlapArea(leftWallCheckTop.position, leftWallCheckBottom.position, wallLayer); 

        fJumpPressedRemember -= Time.deltaTime;
        fGroundedRemember -= Time.deltaTime;
        wallJumpCouldown -= Time.deltaTime;

        if(moveInput < 0){ //pout set la direction
            direction = false; //left
        }
        if(moveInput > 0){
            direction = true; //right 
        }

        if(isFreezing == false){
            if(isGrounded == true){ //pour aller de gauche à droite avec des vitesse différentes en fonction du milieux (sol air)
                float fHorizontalVelocity = moveInput * speedGround;
                //fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDamping, Time.deltaTime * 10f); 
                //rb.velocity = new Vector2(fHorizontalVelocity, rb.velocity.y);  
                rb.velocity = new Vector2((rb.velocity.x + fHorizontalVelocity) * 0.5f, rb.velocity.y);
            }else{
                if(!isWallJumping){
                    float fHorizontalVelocity =  moveInput * speedAir;
                    //fHorizontalVelocity += moveInput * speedAir;
                    //fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDamping, Time.deltaTime * 10f); 
                    rb.velocity = new Vector2((rb.velocity.x + fHorizontalVelocity) * 0.5f, rb.velocity.y); 
                }
            }
        }
         

        if(rb.velocity.magnitude > maxSpeed) //pour avoir une vitesse max et éviter les bug de colision 
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        if(isGrounded == true){ //pour detecter si le player est au sol
            fGroundedRemember = fGroundedRememberTime;
        }


        if(eventJumpKey == "Press"){
            fJumpPressedRemember = fJumpPressedRememberTime; //si on appuie trop top, on le garde en mémoire
        }

        if(eventJumpKey == "Release"){ //quand on relache la touche de saut
            isjumping = false;
        }


        if(isFreezing == false){  //pour bloquer le mouvement quand on change de room

            if(fGroundedRemember > 0 && fJumpPressedRemember > 0){ // pour sauter 
                isjumping = true;
                fJumpPressedRemember = 0;
                fGroundedRemember = 0;
                rb.velocity = Vector2.up * jumpForce; //appliquer le saut
            }

            if((isLeftWall || isRightWall) &&  rb.velocity.y < 0 && stateGrabKey) //pour glisser sur le mur
            {
                isSlidingWall = true;
                rb.velocity = new Vector2(0, slidWallSpeed);
            }else{
                isSlidingWall = false;
            }

            //wall jump
            if (isRightWall && !isGrounded && fJumpPressedRemember > 0 && wallJumpCouldown <= 0){
                fJumpPressedRemember = 0;
                wallJumpCouldown = wallJumpCouldownTime;
                isWallJumping = true;
                rb.velocity = new Vector2(0, 0);
                if(moveInput > 0){//si on appui dans la direction du mur
                    rb.AddForce(new Vector2(0,wallJumpForceY), ForceMode2D.Impulse);
                }else{
                    direction = false;//facing left
                    rb.AddForce(new Vector2(-wallJumpForceX,wallJumpForceY), ForceMode2D.Impulse);
                }
            }
            if (isLeftWall && !isGrounded && fJumpPressedRemember > 0 && wallJumpCouldown <= 0){
                fJumpPressedRemember = 0;
                wallJumpCouldown = wallJumpCouldownTime;
                isWallJumping = true;
                rb.velocity = new Vector2(0, 0);
                if(moveInput < 0){
                    rb.AddForce(new Vector2(0,wallJumpForceY), ForceMode2D.Impulse);
                }else{
                    direction = true;//facing right
                    rb.AddForce(new Vector2(wallJumpForceX,wallJumpForceY), ForceMode2D.Impulse);
                }
            }

            if(isWallJumping && rb.velocity.y < 0){ //quand on commence a retomber après le wall dash
                isWallJumping = false;
                rb.velocity = new Vector2((rb.velocity.x * 0.5f) + (moveInput * speedAir) /** Mathf.Pow(1f - fHorizontalDamping, Time.deltaTime * 10f)*/, rb.velocity.y);
            }

            
            if(rb.velocity.y < 0){ //pour tomber, acceleration,
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }else if(rb.velocity.y > 0 && !stateJumpKey){      //pour descendre plus rapidement si on lache le bouton plus tot, better jumping tuto
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }

        }

        

        if(freezTime > 0f){
            freezTime -= Time.deltaTime;
            if(freezTime <= 0f){  //pour remettre en mouvement quand on sort du freez
                //Debug.Log("défreez");
                isFreezing = false;
                rb.velocity = MouvFreezTimeSave;
                rb.gravityScale = gravityScale;
            }
        }



        SpriteRenderer();

        //pour mettre à 0 les evenements de bouttons
        JumpKeyAction("Reset");

    }


    void JumpKeyAction(string action){ //gere les event de touche de saut

        if(action == "Press"){
            //Debug.Log("il saute");
            stateJumpKey = true;
            eventJumpKey = "Press";
        }

        if(action == "Release"){
            //Debug.Log("il relache");
            stateJumpKey = false;
            eventJumpKey = "Release";
        }

        if(action == "Reset"){
            eventJumpKey = "null";
        }
    }

    void DashKeyAction(string action){ //gere les event de touche de dash
        if(action == "Press"){
            Debug.Log("dash ");
            //rb.AddForce(new Vector2(0,30), ForceMode2D.Impulse);
            stateDashKey = true;
            eventDashKey = "Press";
        }

        if(action == "Release"){
            Debug.Log("dé dash");
            stateDashKey = false;
            eventDashKey = "Release";
        }

        if(action == "Reset"){
            eventDashKey = "null";
        }
    }

    void GrabKeyAction(string action){ //gere les event de touche de Grab
        if(action == "Press"){
            Debug.Log("Grab ");
            //rb.AddForce(new Vector2(0,30), ForceMode2D.Impulse);
            stateGrabKey = true;
            eventGrabKey = "Press";
        }

        if(action == "Release"){
            Debug.Log("dé Grab");
            stateGrabKey = false;
            eventGrabKey = "Release";
        }

        if(action == "Reset"){
            eventGrabKey = "null";
        }
    }

    void DirectionKeyAction(float direction){
        //Debug.Log(direction);
        moveInput = direction;

        if(direction > 0f){
            moveInput = 1;
        }
        if(direction < 0f){
            moveInput = -1;
        }

    }



    void SpriteRenderer(){
        if(isGrounded == true){
            if(direction == false){ //left
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PMoveLeft;
            }
            if(direction == true){ //left
                this.gameObject.GetComponent<SpriteRenderer>().sprite = PMoveRight;
            }
        }else{
            if(isjumping == true){
                if(direction == false){ //left
                    this.gameObject.GetComponent<SpriteRenderer>().sprite = PJumpLeft;
                }
                if(direction == true){ //left
                    this.gameObject.GetComponent<SpriteRenderer>().sprite = PJumpRight;
                }
            }else{
                if(direction == false){ //left
                    this.gameObject.GetComponent<SpriteRenderer>().sprite = PFallLeft;
                }
                if(direction == true){ //left
                    this.gameObject.GetComponent<SpriteRenderer>().sprite = PFallRight;
                }
            }
        }
    }

    public void FreezPlayer(float freezT){ //pour freez le joueur quand on change de room
        //Debug.Log("changement de room");
        if(freezT > 0){
            isFreezing = true;
            freezTime = freezT;
            MouvFreezTimeSave = rb.velocity;
            rb.velocity = new Vector2(0,0);
            gravityScale = rb.gravityScale;
            rb.gravityScale = 0f;
        }
    }

}
