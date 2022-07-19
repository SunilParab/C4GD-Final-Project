using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D playerRb;
    public float speed = 10;
    public float horizontalInput;
    public int gravMode = 0;
    public bool gravOnCooldown;
    public float camRotationTime = 0.5f;
    public int camRotationIntervals = 40;
    public GameObject spawnPoint;
    public GameObject mainCamera;
    public GameObject weapon;
    public bool facedLeft;
    private SpriteRenderer spriteRenderer;
    public bool alive = true;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        spawnPoint = GameObject.Find("SpawnPoint");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (alive) {
            horizontalInput = Input.GetAxis("Horizontal");
            Vector3 velocity;

            if (Input.GetKey(KeyCode.Space)) {
                if (!gravOnCooldown) {
                    if (checkDirs("Up"))
                    {
                        gravOnCooldown = true;
                        StartCoroutine(GravityCooldown());
                        StartCoroutine(GravRotate(180 + 90 * gravMode, gravMode));
                        gravMode = (gravMode + 2) % 4;
                    } else if (checkDirs("Left"))
                    {
                        gravOnCooldown = true;
                        StartCoroutine(GravityCooldown());
                        StartCoroutine(GravRotate(-90 + 90 * gravMode, gravMode));
                        gravMode = (gravMode + 3) % 4;
                    } else if (checkDirs("Right"))
                    {
                        gravOnCooldown = true;
                        StartCoroutine(GravityCooldown());
                        StartCoroutine(GravRotate(90 + 90 * gravMode, gravMode));
                        gravMode = (gravMode + 1) % 4;
                    }
                }
            } else {
                switch (gravMode)
                {
                    case 0:
                        velocity = playerRb.velocity;
                        velocity.x = (speed * horizontalInput);
                        playerRb.velocity = velocity;
                        break;
                    case 2:
                        velocity = playerRb.velocity;
                        velocity.x = (speed * horizontalInput * -1);
                        playerRb.velocity = velocity;
                        break;
                    case 3:
                        velocity = playerRb.velocity;
                        velocity.y = (speed * horizontalInput * -1);
                        playerRb.velocity = velocity;
                        break;
                    case 1:
                        velocity = playerRb.velocity;
                        velocity.y = (speed * horizontalInput);
                        playerRb.velocity = velocity;
                        break;
                }

            }

            if (facedLeft && horizontalInput > 0)
            {
                facedLeft = false;
                spriteRenderer.flipX = false;
                weapon.GetComponent<SpriteRenderer>().flipX = false;
                weapon.GetComponent<BoxCollider2D>().offset = new Vector2(weapon.GetComponent<BoxCollider2D>().offset.x * -1, weapon.GetComponent<BoxCollider2D>().offset.y);
            }
            else if (!facedLeft && horizontalInput < 0) {
                facedLeft = true;
                spriteRenderer.flipX = true;
                weapon.GetComponent<SpriteRenderer>().flipX = true;
                weapon.GetComponent<BoxCollider2D>().offset = new Vector2(weapon.GetComponent<BoxCollider2D>().offset.x * -1, weapon.GetComponent<BoxCollider2D>().offset.y);
            }
        
            switch (gravMode)
            {
                case 0:
                    playerRb.AddForce(Physics.gravity * 1);
                    break;
                case 2:
                    playerRb.AddForce(Physics.gravity * -1);
                    break;
                case 3:
                    playerRb.AddForce(Quaternion.Euler(0, 0, -90) * Physics.gravity);
                    break;
                case 1:
                    playerRb.AddForce(Quaternion.Euler(0, 0, 90) * Physics.gravity);
                    break;
            }
        }

    }

    bool checkDirs(string mainDir)
    {
        switch (mainDir)
        {
            case "Left":
                return ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && !(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)));
            case "Right":
                return ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && !(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A)));
            case "Up":
                return ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && !(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)));
            case "Down":
                return ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && !(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)));
        }
        return true;
    }

    IEnumerator GravityCooldown()
    {
        yield return new WaitForSeconds(1);
        gravOnCooldown = false;
    }

    IEnumerator GravRotate(int endRotation, int oldMode)
    {
        if (endRotation < 0) {
            endRotation += 360;
        }

        int currentRot = 0;

        switch (oldMode)
        {
            case 0:
                currentRot = 0;
                break;
            case 2:
                currentRot = 180;
                break;
            case 3:
                currentRot = 270;
                break;
            case 1:
                currentRot = 90;
                break;
        }

        int rotChange = (endRotation - currentRot);

        if (rotChange > 180)
        {
            rotChange -= 360;
        }

        if (rotChange < -180)
        {
            rotChange += 360;
        }

        for (int i = 0; i < camRotationIntervals; i++) {
            transform.Rotate(0,0,rotChange / camRotationIntervals);
            yield return new WaitForSeconds(camRotationTime/camRotationIntervals);
        }
        transform.eulerAngles = new Vector3(0, 0, endRotation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alive && other.gameObject.CompareTag("KillZone"))
        {
            StartCoroutine(Respawn());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (alive && other.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        alive = false;
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(1);
        playerRb.constraints = RigidbodyConstraints2D.None;
        playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        gravMode = 0;
        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;
        alive = true;
    }

}
