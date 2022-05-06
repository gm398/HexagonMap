using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{

    [SerializeField] Transform cam;
    CharacterController characterController;
    [SerializeField] UnitController unitController;
    float speed = 1;
    bool isGrounded;

    Vector3 velocity;
    float hInput, vInput;

    bool isJumping;
    float jumpHeight = .75f;
    [SerializeField] float jumpGravity = -10;
    [SerializeField] float fallGravity = -20;
    float gravity = 0f;

    HexCoordinates hexCoords;
    HexGrid hexGrid;
    LayerMask layers;
    bool underPlayerControl = false;

    AttackType attackType;
    int range;
    bool ranged;
    bool canAttackAir;
    float attackSpeed;
    bool canAttack = true;
    List<Hex> currentAim = new List<Hex>();
    Vector3 facingDirection = new Vector3(0, 0, 0);
    Hex hexFaced = null;

    bool overViableHex;
    private void Awake()
    {
        unitController = this.GetComponent<UnitController>();
        characterController = this.GetComponent<CharacterController>();
        hexCoords = this.gameObject.GetComponent<HexCoordinates>();
        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        speed = unitController.GetSpeed();
        jumpHeight = unitController.GetHeightStep() + .3f;
    }
    // Update is called once per frame
    void Update()
    {
        if (!underPlayerControl) { return; }
        isGrounded = characterController.isGrounded;
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        isJumping = Input.GetButton("Jump");
       

        Gravity();
        GroundMovement();
        GetCurrentFacingDirection();
        

        CheckCurrentHex();
        if (Input.GetKey("mouse 0"))
        {
            if (canAttack)
            {
                Attack();
            }
        }

    }
    private void LateUpdate()
    {
        if (!underPlayerControl) { return; }
        ClearTargeting();
        if (!overViableHex) { return; }
        if (ranged)
        {
           TargetFaced();
           if(hexFaced != null)
            {
                if (hexFaced.DistanceFromHex(unitController.GetcurrentHex()) <= range)
                {
                    ShowTargeting(hexFaced.GetHexCoordinates().GetHexCoordsRQS());
                }
            }
        }
        else
        {
            ShowTargeting(hexCoords.GetHexCoordsRQS());
        }
        
    }
    void CheckCurrentHex()
    {
        float moveMultiplier = 1;
        if (!unitController.CanFly())
        {
            moveMultiplier = unitController.GetcurrentHex().GetMoveDificulty();
        }
        characterController.Move(velocity * Time.deltaTime / moveMultiplier);
        overViableHex = true;
        hexCoords.ConvertToHexCords();
        if(hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out Hex current))
        {
            if (!current.Equals(unitController.GetcurrentHex())){
                bool viableHex =  (unitController.CanFly() || (!unitController.CanFly() && current.IsTraversable()));
                if (current.GetOccupant() == null && viableHex)
                {
                    current.SetOccupent(this.gameObject);
                    unitController.SetCurrentHex(current);
                    this.GetComponent<VisionController>().UpdateVision(current);
                }
                else if(!this.gameObject.Equals(current.GetOccupant()))
                {
                    
                    velocity.x *= -2f;
                    velocity.z *= -2f;
                    characterController.Move(velocity * Time.deltaTime);
                    overViableHex = false;
                    Physics.SyncTransforms();
                }
            }
        }
        
        characterController.Move(velocity * Time.deltaTime / unitController.GetcurrentHex().GetMoveDificulty());
        Physics.SyncTransforms();

    }

    void GetCurrentFacingDirection()
    {
        facingDirection = attackType.GetFacingDirection(transform.eulerAngles.y);
    }

    void ShowTargeting(Vector3 center)
    {
        foreach (Vector3 v in attackType.GetShapeFacingDirection(facingDirection))
        {
            if(hexGrid.GetHex(v + center, out Hex h))
            {
                currentAim.Add(h);
            }
        }
        foreach(Hex h in currentAim)
        {
            if (h.IsVisible())
            {
                h.HighlightHex(true);
            }
        }
    }
    void ClearTargeting()
    {
        foreach (Hex h in currentAim)
        {
            h.HighlightHex(false);
        }
        currentAim.Clear();
    }

    void TargetFaced()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit))
        {
            GameObject tempResult = hit.collider.gameObject;
            HexCoordinates tempCoords = tempResult.GetComponentInParent<HexCoordinates>();
            if(tempCoords != null)
            {
                hexGrid.GetHex(tempCoords.GetHexCoordsRQS(), out hexFaced);
            }
        }
    }
    
    void Attack()
    {
        if (ranged)
        {
            TargetFaced();
            if (hexFaced != null)
            {
                if (hexFaced.DistanceFromHex(unitController.GetcurrentHex()) <= range)
                {
                    attackType.PlayerAttack(hexFaced.GetHexCoordinates().GetHexCoordsRQS(), facingDirection, layers, canAttackAir);
                    canAttack = false;
                    Invoke("ResetAttack", 1 / attackSpeed);
                }
            }
        }
        else
        {
            attackType.PlayerAttack(hexCoords.GetHexCoordsRQS(), facingDirection, layers, canAttackAir);
            canAttack = false;
            Debug.Log("attacked");
            Invoke("ResetAttack", 1 / attackSpeed);
        }
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    void GroundMovement()
    {
        Vector3 move = transform.right * hInput + transform.forward * vInput;
        velocity.x = move.x * speed;
        velocity.z = move.z * speed;

        if (velocity.y > 0) { gravity = jumpGravity; }
        else { gravity = fallGravity; }
        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * jumpGravity);
        }

    }

    void Gravity()
    {
        velocity.y += gravity * Time.deltaTime;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

    }



    public void SetDirectControl(bool control) {
        underPlayerControl = control;
        ClearTargeting();
        layers = unitController.GetEnemyLayers();
        UnitCombatController combatcontroller = this.GetComponent<UnitCombatController>();
        attackType = combatcontroller.GetAttackType();
        ranged = combatcontroller.IsRanged();
        range = combatcontroller.GetRange();
        canAttackAir = combatcontroller.CanAttackAir();
        attackSpeed = combatcontroller.GetAttackSpeed();
        this.transform.rotation = Quaternion.identity;
    }
}
