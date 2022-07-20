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
    public bool gravCharged;
    public float maxSpeed;
    public bool touchedOther;
    public float gravCoef;
    public bool inCannon;
    public float cannonSpeed = 20;
    private Vector3 dirToMouse;
    private List<Collision2D> colliders = new List<Collision2D>();
    public bool cannonLaunch;
    public bool cannonOnCooldown;
    public float cannonCooldown;
    public bool cannonCharged;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        spawnPoint = GameObject.Find("SpawnPoint");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (alive)
        {
            if (Input.GetAxis("Fire2") > 0 && !inCannon && !cannonOnCooldown && cannonCharged)
            {
                inCannon = true;
                Vector3 posInScreen = Camera.main.WorldToScreenPoint(transform.position);
                dirToMouse = (Input.mousePosition - posInScreen);
                dirToMouse.Normalize();
                StartCoroutine(CannonActive());
            }
            if (inCannon || cannonLaunch)
            {
                if (cannonLaunch)
                {
                    transform.Translate(dirToMouse * cannonSpeed);
                }
            }
            else
            {
                horizontalInput = Input.GetAxis("Horizontal");
                if (Input.GetKey(KeyCode.Space))
                {
                    if (!gravOnCooldown && gravCharged)
                    {
                        if (checkDirs("Up"))
                        {
                            gravOnCooldown = true;
                            gravCharged = false;
                            playerRb.velocity = Vector3.zero;
                            StartCoroutine(GravityCooldown());
                            StartCoroutine(GravRotate(180 + 90 * gravMode, gravMode));
                            gravMode = (gravMode + 2) % 4;
                        }
                        else if (checkDirs("Left"))
                        {
                            gravOnCooldown = true;
                            gravCharged = false;
                            playerRb.velocity = Vector3.zero;
                            StartCoroutine(GravityCooldown());
                            StartCoroutine(GravRotate(-90 + 90 * gravMode, gravMode));
                            gravMode = (gravMode + 3) % 4;
                        }
                        else if (checkDirs("Right"))
                        {
                            gravOnCooldown = true;
                            gravCharged = false;
                            playerRb.velocity = Vector3.zero;
                            StartCoroutine(GravityCooldown());
                            StartCoroutine(GravRotate(90 + 90 * gravMode, gravMode));
                            gravMode = (gravMode + 1) % 4;
                        }
                    }
                }
                else
                {
                    switch (gravMode)
                    {
                        case 0:
                            transform.Translate(transform.right * Time.deltaTime * speed * horizontalInput);
                            break;
                        case 2:
                            transform.Translate(transform.right * Time.deltaTime * speed * horizontalInput * -1);
                            break;
                        case 3:
                            transform.Translate(transform.up * Time.deltaTime * speed * horizontalInput);
                            break;
                        case 1:
                            transform.Translate(transform.up * Time.deltaTime * speed * horizontalInput * -1);
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
                else if (!facedLeft && horizontalInput < 0)
                {
                    facedLeft = true;
                    spriteRenderer.flipX = true;
                    weapon.GetComponent<SpriteRenderer>().flipX = true;
                    weapon.GetComponent<BoxCollider2D>().offset = new Vector2(weapon.GetComponent<BoxCollider2D>().offset.x * -1, weapon.GetComponent<BoxCollider2D>().offset.y);
                }

                switch (gravMode)
                {
                    case 0:
                        playerRb.AddForce(Physics.gravity * 1 * gravCoef);
                        break;
                    case 2:
                        playerRb.AddForce(Physics.gravity * -1 * gravCoef);
                        break;
                    case 3:
                        playerRb.AddForce(Quaternion.Euler(0, 0, -90) * Physics.gravity * gravCoef);
                        break;
                    case 1:
                        playerRb.AddForce(Quaternion.Euler(0, 0, 90) * Physics.gravity * gravCoef);
                        break;
                }
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
        yield return new WaitForSeconds(camRotationTime);
        gravOnCooldown = false;
        if (colliders.Count > 0)
        {
            gravCharged = true;
        }
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
        if (inCannon)
        {
            inCannon = false;
            cannonLaunch = false;
            StartCoroutine(CannonCooldown());
        }
        if (alive && other.gameObject.CompareTag("KillZone"))
        {
            StartCoroutine(Respawn());
        }
        else if (other.gameObject.CompareTag("Spring"))
        {
            playerRb.velocity = Vector3.zero;
            playerRb.AddForce(other.gameObject.transform.up * 80, ForceMode2D.Impulse);
            StopCoroutine("tempGrav");
            StartCoroutine("tempGrav");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (inCannon)
        {
            inCannon = false;
            cannonLaunch = false;
            StartCoroutine(CannonCooldown());
        }
        if (alive && other.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(Respawn());
        } else if (other.gameObject.CompareTag("Ground"))
        {
            if (!colliders.Contains(other)) 
            { 
                colliders.Add(other);
            }
            gravCharged = true;
            cannonCharged = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            colliders.Remove(other);
        }
        if (!(colliders.Count > 0))
        {
            gravCharged = false;
            cannonCharged = false;
        }
    }

    IEnumerator tempGrav()
    {
        gravCharged = true;
        yield return new WaitForSeconds(1);
        if (!(colliders.Count > 0))
        {
            gravCharged = false;
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

    IEnumerator CannonActive()
    {
        yield return new WaitForSeconds(1);
        if (Input.GetAxis("Fire2") > 0)
        {
            while(Input.GetAxis("Fire2") > 0)
            {
                yield return new WaitForSeconds(0.01f);
            }
            cannonLaunch = true;
        } else
        {
            inCannon = false;
        }
    }

    IEnumerator CannonCooldown()
    {
        cannonOnCooldown = true;
        yield return new WaitForSeconds(cannonCooldown);
        cannonOnCooldown = false;
    }

}
