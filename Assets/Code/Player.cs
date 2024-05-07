using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public static Player player { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (player != null && player != this)
        {
            Destroy(this);
        }
        else
        {
            player = this;
        }

        UpdatePositionData();

    }


    Rigidbody rb;
    Camera mainCam;

    [Header("Position Data")]
    public EntityPosition playerPosition = new EntityPosition();

    [Header ("Movement Settings")]
    public float mouseSensitivity = 0f;
    float _xRotation = 0f;

    public float acceleration = 0f;
    public float movementSpeed = 0f;
    public float jumpForce = 0f;

    [Header ("Flags")]
    [SerializeField/*, ReadOnly*/] bool isGrounded = false;
    bool jump = false;
    bool moveLeft = false;
    bool moveRight = false;
    bool moveForward = false;
    bool moveBackward = false;

    public bool loadPositiveXChunks = false;
    public bool loadNegativeXChunks = false;

    public bool loadPositiveZChunks = false;
    public bool loadNegativeZChunks = false;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        mainCam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
        CheckIfGrounded();
        CheckInput();

        UpdatePositionData();
    }

    void FixedUpdate()
    {
        UpdateMovement();
        Jump();
    }

    void UpdatePositionData()
    {
        playerPosition.rawPosition = transform.position;

        int voxelPosX = (int)playerPosition.rawPosition.x;
        int voxelPosY = (int)playerPosition.rawPosition.y;
        int voxelPosZ = (int)playerPosition.rawPosition.z;

        int chunkPosX = (int)playerPosition.rawPosition.x;
        int chunkPosY = (int)playerPosition.rawPosition.y;
        int chunkPosZ = (int)playerPosition.rawPosition.z;

        if (playerPosition.rawPosition.x != Mathf.Abs(playerPosition.rawPosition.x))
            voxelPosX -= WorldSettings.VOXEL_SIZE;

        if (playerPosition.rawPosition.y != Mathf.Abs(playerPosition.rawPosition.y))
            voxelPosY -= WorldSettings.VOXEL_SIZE;

        if (playerPosition.rawPosition.z != Mathf.Abs(playerPosition.rawPosition.z))
            voxelPosZ -= WorldSettings.VOXEL_SIZE;

        if (playerPosition.rawPosition.x != Mathf.Abs(playerPosition.rawPosition.x))
            chunkPosX -= WorldSettings.CHUNK_WIDTH;

        if (playerPosition.rawPosition.y != Mathf.Abs(playerPosition.rawPosition.y))
            chunkPosY -= WorldSettings.CHUNK_WIDTH;

        if (playerPosition.rawPosition.z != Mathf.Abs(playerPosition.rawPosition.z))
            chunkPosZ -= WorldSettings.CHUNK_WIDTH;

        playerPosition.voxelPosition = new Vector3Int(voxelPosX, voxelPosY, voxelPosZ) / WorldSettings.VOXEL_SIZE;

        playerPosition.chunkPosition = new Vector3Int(chunkPosX, chunkPosY, chunkPosZ) / WorldSettings.CHUNK_WIDTH;


        //update x chunks
        if (playerPosition.chunkPosition.x == playerPosition.previousChunkPosition.x + 1)
            loadPositiveXChunks = true;
        else
            loadPositiveXChunks = false;
        if (playerPosition.chunkPosition.x == playerPosition.previousChunkPosition.x - 1)
            loadNegativeXChunks = true;
        else
            loadNegativeXChunks = false;

        //update z chunks
        if (playerPosition.chunkPosition.z == playerPosition.previousChunkPosition.z + 1)
            loadPositiveZChunks = true;
        else
            loadPositiveZChunks = false;
        if (playerPosition.chunkPosition.z == playerPosition.previousChunkPosition.z - 1)
            loadNegativeZChunks = true;
        else
            loadNegativeZChunks = false;

        playerPosition.previousChunkPosition = playerPosition.chunkPosition;
    }

    void CheckInput()
    {
        if (Input.GetKey(KeyCode.W))
            moveForward = true;
        else
            moveForward = false;

        if (Input.GetKey(KeyCode.A))
            moveLeft = true;
        else
            moveLeft = false;

        if (Input.GetKey(KeyCode.S))
            moveBackward = true;
        else
            moveBackward = false;

        if (Input.GetKey(KeyCode.D))
            moveRight = true;
        else
            moveRight = false;

        if (Input.GetKey(KeyCode.Space))
            jump = true;
        else
            jump = false;
    }

    void CheckIfGrounded()
    {
        Vector3 origin = transform.position + new Vector3(0f, -0.9f, 0f);
        Debug.DrawLine(origin, origin - (Vector3.up * 0.11f), Color.cyan);

        if (Physics.Raycast(origin, -Vector3.up, maxDistance: 0.11f))
            isGrounded = true;
        else
            isGrounded = false;
    }

    void Jump() //Forcemode.Impulse does not need to be called in FixedUpdate
    {
        if(jump && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void UpdateMovement() //must be called in FixedUpdate because of physics calls.
    {
        if (moveForward)
            rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);

        if (moveLeft)
            rb.AddForce(-transform.right * acceleration, ForceMode.Acceleration);

        if (moveBackward)
            rb.AddForce(-transform.forward * acceleration, ForceMode.Acceleration);

        if (moveRight)
            rb.AddForce(transform.right * acceleration, ForceMode.Acceleration);

        //cap speed
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, movementSpeed);
        rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);

       // Debug.Log("velocity: " + rb.velocity.normalized + " speed: " + rb.velocity.magnitude);
    }

    void UpdateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        mainCam.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
