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
            constManager.CompleteConstellation();
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
                constManager.BreakConstellation();
                lastStar = null;
            }
            else
            {
                lastStar = isStar;
                isStar.StopMovement();
                isStar.starData.Position = isStar.transform.position;
                constManager.AddStar(isStar.starData);
            }
        }
    }
}
