using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{

    private float maxDist;
    private float minDist;
    public float maxDistChange = 8;
    public float speed = 20f;
    public int directionMod = 1;
    public float endDelay;
    public bool move = true;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        maxDist = transform.position.x + maxDistChange;
        minDist = transform.position.x - maxDistChange;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed * directionMod);
            if (transform.position.x >= maxDist)
            {
                directionMod *= -1;
                spriteRenderer.flipX = !spriteRenderer.flipX;
                transform.position = new Vector3(maxDist, transform.position.y, transform.position.z);
                move = false;
                StartCoroutine(Wait());
            }
            else if (transform.position.x <= minDist)
            {
                directionMod *= -1;
                spriteRenderer.flipX = !spriteRenderer.flipX;
                transform.position = new Vector3(minDist, transform.position.y, transform.position.z);
                move = false;
                StartCoroutine(Wait());
            }
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        move = true;
    }

}
