using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D playerRb;
    public float speed = 10;
    public float horizontalInput;
    public float verticalInput;
    public float gravMode = 0;
    public bool spaceHeld;
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceHeld = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            spaceHeld = false;
        }

        if (spaceHeld) {
            if (!gravOnCooldown) {
                if (verticalInput < 0)
                {
                    gravMode = 0;
                    gravOnCooldown = true;
                    StartCoroutine(GravityCooldown());
                    StartCoroutine(GravRotate(0));
                } else if (verticalInput > 0)
                {
                    gravMode = 2;
                    gravOnCooldown = true;
                    StartCoroutine(GravityCooldown());
                    StartCoroutine(GravRotate(180));
                } else if (horizontalInput < 0)
                {
                    gravMode = 3;
                    gravOnCooldown = true;
                    StartCoroutine(GravityCooldown());
                    StartCoroutine(GravRotate(-90));
                } else if (horizontalInput > 0)
                {
                    gravMode = 1;
                    gravOnCooldown = true;
                    StartCoroutine(GravityCooldown());
                    StartCoroutine(GravRotate(90));
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

    IEnumerator GravRotate(int endRotation)
    {
        if (endRotation < 0) {
            endRotation += 360;
        }

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

        float rotChange = (endRotation - currentRot);

        //Debug.Log(endRotation + " " + currentRot + " " + rotChange);

        for (int i = 0; i < 30; i++) {
            transform.Rotate(0,0,rotChange / 30);
            yield return new WaitForSeconds(0.05f);
        }
        transform.eulerAngles = new Vector3(0, 0, endRotation);
    }

}
