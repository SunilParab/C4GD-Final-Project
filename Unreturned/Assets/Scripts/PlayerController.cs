using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D playerRb;
    public float speed = 10;
    public float horizontalInput;
    public int gravMode = 0;
    public bool gravOnCooldown;
    public float camRotationTime = 0.5f;
    public int camRotationIntervals = 40;
    public GameObject mainCamera;
    public GameObject weapon;
    public bool facedLeft;
    private SpriteRenderer spriteRenderer;
    public bool alive = true;
    public bool gravCharged;
    public float maxSpeed;
    public float gravCoef;
    public bool inCannon;
    public float cannonSpeed = 20;
    private Vector3 dirToMouse;
    public List<GameObject> colliders = new List<GameObject>();
    public List<GameObject> walls = new List<GameObject>();
    public bool cannonLaunch;
    public bool cannonOnCooldown;
    public float cannonCooldown;
    public bool cannonCharged;
    public Animator animator;
    public Animator weaponAnimator;
    private AudioSource playerAudio;
    public GameObject gravIndicator;
    public GameObject cannonIndicator;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        animator.SetBool("Walking", false);
        if (alive)
        {
            if (Input.GetAxis("Fire2") > 0 && !inCannon && !cannonOnCooldown && cannonCharged)
            {
                inCannon = true;

                Vector3 posInScreen = mainCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
                dirToMouse = (posInScreen - transform.position);
                dirToMouse = new Vector3(dirToMouse.x, dirToMouse.y, 0);
                dirToMouse.Normalize();

                switch (gravMode)
                {
                    case 0:
                        dirToMouse *= 1;
                        break;
                    case 2:
                        dirToMouse *= -1;
                        break;
                    case 3:
                        dirToMouse = Quaternion.Euler(0, 0, -90) * dirToMouse * -1;
                        break;
                    case 1:
                        dirToMouse = Quaternion.Euler(0, 0, 90) * dirToMouse * -1;
                        break;
                }

                StartCoroutine(CannonActive());
            }
            if (inCannon || cannonLaunch)
            {
                if (cannonLaunch)
                {
                    transform.Translate(dirToMouse * cannonSpeed);
                } else
                {
                    Vector3 posInScreen = mainCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
                    dirToMouse = (posInScreen - transform.position);
                    dirToMouse = new Vector3(dirToMouse.x, dirToMouse.y, 0);
                    dirToMouse.Normalize();

                    switch (gravMode)
                    {
                        case 0:
                            dirToMouse *= 1;
                            break;
                        case 2:
                            dirToMouse *= -1;
                            break;
                        case 3:
                            dirToMouse = Quaternion.Euler(0, 0, -90) * dirToMouse * -1;
                            break;
                        case 1:
                            dirToMouse = Quaternion.Euler(0, 0, 90) * dirToMouse * -1;
                            break;
                    }
                }
            }
            else
            {
                horizontalInput = Input.GetAxis("Horizontal");
                if (horizontalInput != 0)
                {
                    animator.SetBool("Walking", true);
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    if (!gravOnCooldown && gravCharged)
                    {
                        if (checkDirs("Up"))
                        {
                            gravIndicator.GetComponent<SpriteRenderer>().color = Color.red;
                            gravOnCooldown = true;
                            gravCharged = false;
                            playerRb.velocity = Vector3.zero;
                            StartCoroutine(GravityCooldown());
                            StartCoroutine(GravRotate(180 + 90 * gravMode, gravMode));
                            gravMode = (gravMode + 2) % 4;
                            playerAudio.Play(0);
                        }
                        else if (checkDirs("Left"))
                        {
                            gravIndicator.GetComponent<SpriteRenderer>().color = Color.red;
                            gravOnCooldown = true;
                            gravCharged = false;
                            playerRb.velocity = Vector3.zero;
                            StartCoroutine(GravityCooldown());
                            StartCoroutine(GravRotate(-90 + 90 * gravMode, gravMode));
                            gravMode = (gravMode + 3) % 4;
                            playerAudio.Play(0);
                        }
                        else if (checkDirs("Right"))
                        {
                            gravIndicator.GetComponent<SpriteRenderer>().color = Color.red;
                            gravOnCooldown = true;
                            gravCharged = false;
                            playerRb.velocity = Vector3.zero;
                            StartCoroutine(GravityCooldown());
                            StartCoroutine(GravRotate(90 + 90 * gravMode, gravMode));
                            gravMode = (gravMode + 1) % 4;
                            playerAudio.Play(0);
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
        if (gravCharged)
        {
            gravIndicator.GetComponent<SpriteRenderer>().color = Color.green;
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
            animator.SetBool("CannonShot", false);
            weaponAnimator.SetBool("Hide", false);
            StartCoroutine(CannonCooldown());
        }
        if (alive && other.gameObject.CompareTag("KillZone"))
        {
            StartCoroutine(Respawn());
        }
        else if (other.gameObject.CompareTag("Portal"))
        {
            other.gameObject.GetComponent<LevelPortal>().NextLevel();
        }
        else if (other.gameObject.CompareTag("Spring"))
        {
            playerRb.velocity = Vector3.zero;
            playerRb.AddForce(other.gameObject.transform.up * other.gameObject.GetComponent<SpringController>().springConstant, ForceMode2D.Impulse);
            colliders.Add(other.gameObject);
            StartCoroutine(tempGrav(other.gameObject));
            other.gameObject.GetComponent<SpringController>().animator.Play("Work");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (inCannon)
        {
            inCannon = false;
            cannonLaunch = false;
            animator.SetBool("CannonShot", false);
            weaponAnimator.SetBool("Hide", false);
            StartCoroutine(CannonCooldown());
        }
        if (alive && other.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(Respawn());
        } else if (other.gameObject.CompareTag("Ground"))
        {
            if (!colliders.Contains(other.gameObject)) 
            { 
                colliders.Add(other.gameObject);
                walls.Add(other.gameObject);
            }
            gravCharged = true;
            cannonCharged = true;
            if (!gravOnCooldown)
            {
                gravIndicator.GetComponent<SpriteRenderer>().color = Color.green;
            }
            if (!cannonOnCooldown) {
                cannonIndicator.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            colliders.Remove(other.gameObject);
            walls.Remove(other.gameObject);
        }
        if (!(colliders.Count > 0))
        {
            gravCharged = false;
            gravIndicator.GetComponent<SpriteRenderer>().color = Color.red;
        }
        if (!(walls.Count > 0))
        {
            cannonCharged = false;
            cannonIndicator.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    IEnumerator tempGrav(GameObject spring)
    {
        gravCharged = true;
        if (!gravOnCooldown)
        {
            gravIndicator.GetComponent<SpriteRenderer>().color = Color.green;
        }
        yield return new WaitForSeconds(1);
        colliders.Remove(spring);
        if (!(colliders.Count > 0))
        {
            gravCharged = false;
            gravIndicator.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    IEnumerator Respawn()
    {
        alive = false;
        animator.SetBool("Alive", false);
        weaponAnimator.SetBool("Hide", true);
        playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(1);
        Scene thisScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(thisScene.name);
    }

    IEnumerator CannonActive()
    {
        weaponAnimator.SetBool("Hide", true);
        animator.SetBool("CannonStart", true);
        yield return new WaitForSeconds(1);
        if (Input.GetAxis("Fire2") > 0)
        {
            animator.SetBool("CannonCharged", true);
            animator.SetBool("CannonStart", false);
            while (Input.GetAxis("Fire2") > 0)
            {
                yield return new WaitForSeconds(0.01f);
            }
            cannonLaunch = true;
            animator.SetBool("CannonCharged", false);
            animator.SetBool("CannonShot", true);
            cannonIndicator.GetComponent<SpriteRenderer>().color = Color.red;

            yield return new WaitForSeconds(0.2f);
            if ((colliders.Count > 0))
            {
                inCannon = false;
                cannonLaunch = false;
                animator.SetBool("CannonShot", false);
                weaponAnimator.SetBool("Hide", false);
                StartCoroutine(CannonCooldown());
            }

        } else
        {
            inCannon = false;
            weaponAnimator.SetBool("Hide", false);
        }
        animator.SetBool("CannonStart", false);
    }

    IEnumerator CannonCooldown()
    {
        cannonOnCooldown = true;
        yield return new WaitForSeconds(cannonCooldown);
        cannonOnCooldown = false;
        if (cannonCharged)
        {
            cannonIndicator.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

}
