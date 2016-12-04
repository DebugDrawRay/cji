using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float acceleration;

    private PlayerActions actions;
    private Rigidbody rigid;

    private ConstellationManager constManager;

    //Star tracking
    private StarController lastStar;

    public GameObject PlayerModel;

    [Header("Player Colors")]
    public Material NormalMat;
    public Material PinkMat;
    public Material BlueMat;
    public Material GreenMat;
    public Material YellowMat;

    public static PlayerController Instance;

    [Header("Comet")]
    public int cometLayer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        actions = PlayerActions.BindAll();
        rigid = GetComponent<Rigidbody>();
        constManager = ConstellationManager.Instance;
    }

    void Update()
    {
        CompleteConstellationListener();
    }

    void FixedUpdate()
    {
        Movement();
    }

    void CompleteConstellationListener()
    {
        if(actions.PrimaryAction.WasPressed)
        {
            if(constManager.CompleteConstellation())
            {
                ChangeColor(GameData.StarType.None);
                lastStar = null;
            }
        }
    }
    void Movement()
    {
        Vector2 dir = actions.Move.Value * speed;
        Vector2 newSpeed = Vector2.Lerp(rigid.velocity, dir, acceleration);

        rigid.velocity = newSpeed;
    }

    void OnTriggerEnter(Collider hit)
    {
        StarController isStar = hit.GetComponent<StarController>();

        if(isStar)
        {
            if(lastStar != null && lastStar.theStarType != isStar.theStarType)
            {
                Camera.main.GetComponent<CameraController>().DoScreenShake();
                constManager.BreakConstellation();
                ChangeColor(GameData.StarType.None);
                lastStar = null;
            }
            else
            {
                lastStar = isStar;
                isStar.StopMovement();
                //isStar.starBoing.GetComponent<StarBoingController>().StartGrowing();
                isStar.starBoing.gameObject.SetActive(true);//active the star boing

                StarController isStarStarCont = isStar.GetComponent<StarController>();
                isStarStarCont.delayBeforeSecondBoingTimer = isStarStarCont.delayBeforeSecondBoingTimerBase;//start the timer for the 2nd star boing

                isStar.starData.Position = isStar.transform.position;
                ChangeColor(isStar.theStarType);
                constManager.AddStar(isStar.starData);
            }
        }

        if (hit.gameObject.layer == cometLayer)
        {
            //KILL THE WORLD 
            GameController.TriggerEndGame();
        }

    }

    public void ChangeColor(GameData.StarType type)
    {
        Renderer render = PlayerModel.GetComponent<Renderer>();
        switch (type)
        {
            case GameData.StarType.Circle:
                render.material = BlueMat;
                break;
            case GameData.StarType.Square:
                render.material = PinkMat;
                break;
            case GameData.StarType.Star:
                render.material = YellowMat;
                break;
            case GameData.StarType.Triangle:
                render.material = GreenMat;
                break;
            case GameData.StarType.None:
                render.material = NormalMat;
                break;
        }
    }
}
