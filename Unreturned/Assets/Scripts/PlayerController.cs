using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D playerRb;
    public float speed = 10;
    public float horizontalInput;
    public float verticalInput;
    public int gravMode = 0;
    public bool gravOnCooldown;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        Vector3 velocity;

        if (Input.GetKey(KeyCode.Space)) {
            if (!gravOnCooldown) {
                if (verticalInput > 0 && gravMode != 2 && horizontalInput == 0)
                {
                    gravOnCooldown = true;
                    StartCoroutine(GravityCooldown());
                    StartCoroutine(GravRotate(180 + 90 * gravMode, gravMode));
                    gravMode = (gravMode + 2) % 4;
                } else if (horizontalInput < 0 && gravMode != 3 && verticalInput == 0)
                {
                    gravOnCooldown = true;
                    StartCoroutine(GravityCooldown());
                    StartCoroutine(GravRotate(-90 + 90 * gravMode, gravMode));
                    gravMode = (gravMode + 3) % 4;
                } else if (horizontalInput > 0 && gravMode != 1 && verticalInput == 0)
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

    IEnumerator GravityCooldown()
    {
        yield return new WaitForSeconds(3);
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

        Debug.Log(endRotation + " " + currentRot + " " + rotChange);

        for (int i = 0; i < 10; i++) {
            transform.Rotate(0,0,rotChange / 10);
            yield return new WaitForSeconds(0.05f);
        }
        transform.eulerAngles = new Vector3(0, 0, endRotation);
    }

}