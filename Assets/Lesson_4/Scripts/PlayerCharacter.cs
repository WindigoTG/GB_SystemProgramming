using UnityEngine;
using UnityEngine.Networking;

public class PlayerCharacter : Character, IDamageable
{
    [Range(0, 100)] [SerializeField] private int health = 100;

    [Range(0.5f, 10.0f)] [SerializeField] private float movingSpeed = 8.0f;
    [SerializeField] private float acceleration = 3.0f;
    private const float gravity = -9.8f;
    private CharacterController characterController;
    private MouseLook mouseLook;

    private Vector3 currentVelocity;

    [SyncVar] int serverHealth;

    protected override FireAction fireAction { get; set; }

    public NetworkConnection connection { get; set; }

    protected override void Initiate()
    {
        base.Initiate();
        fireAction = gameObject.AddComponent<RayShooter>();
        fireAction.Reloading();
        characterController = GetComponentInChildren<CharacterController>();
        characterController ??= gameObject.AddComponent<CharacterController>();
        mouseLook = GetComponentInChildren<MouseLook>();
        mouseLook ??= gameObject.AddComponent<MouseLook>();

        CmdSetHealth(health);
    }

    public override void Movement()
    {
        if (mouseLook != null && mouseLook.PlayerCamera != null)
        {
            mouseLook.PlayerCamera.enabled = hasAuthority;
        }

        if (hasAuthority)
        {
            var moveX = Input.GetAxis("Horizontal") * movingSpeed;
            var moveZ = Input.GetAxis("Vertical") * movingSpeed;
            var movement = new Vector3(moveX, 0, moveZ);
            movement = Vector3.ClampMagnitude(movement, movingSpeed);
            movement *= Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                movement *= acceleration;
            }

            movement.y = gravity;
            movement = transform.TransformDirection(movement);

            characterController.Move(movement);
            mouseLook.Rotation();

            CmdUpdatePosition(transform.position);
            CmdUpdateRotation(transform.rotation);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, serverPosition, ref currentVelocity, movingSpeed * Time.deltaTime);
            transform.rotation = serverRotation;
        }
    }

    private void Start()
    {
        Initiate();
    }

    private void LateUpdate()
    {
        health = serverHealth;
    }

    private void OnGUI()
    {
        if (Camera.main == null)
        {
            return;
        }

        if (!hasAuthority)
            return;

        var info = $"Health: {health}\nClip: {fireAction.bulletCount}";
        var size = 12;
        var bulletCountSize = 50;
        var posX = Camera.main.pixelWidth / 2 - size / 4;
        var posY = Camera.main.pixelHeight / 2 - size / 2;
        var posXBul = Camera.main.pixelWidth - bulletCountSize * 2;
        var posYBul = Camera.main.pixelHeight - bulletCountSize;
        GUI.Label(new Rect(posX, posY, size, size), "+");
        GUI.Label(new Rect(posXBul, posYBul, bulletCountSize * 2, bulletCountSize * 2), info);
    }

    public void TakeDamage(int amount)
    {
        CmdTakeDamage(amount);
    }

    [Command]
    private void CmdTakeDamage(int amount)
    {
        serverHealth -= amount;
    }

    [Command]
    private void CmdSetHealth(int health)
    {
        serverHealth = health;
    }
}
