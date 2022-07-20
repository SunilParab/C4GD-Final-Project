using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHover : MonoBehaviour
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
        maxDist = transform.position.y + maxDistChange;
        minDist = transform.position.y - maxDistChange;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            transform.Translate(Vector3.up * Time.deltaTime * speed * directionMod);
            if (transform.position.y >= maxDist)
            {
                directionMod *= -1;
                transform.position = new Vector3(transform.position.x, maxDist, transform.position.z);
                move = false;
                StartCoroutine(Wait());
            }
            else if (transform.position.y <= minDist)
            {
                directionMod *= -1;
                transform.position = new Vector3(transform.position.x, minDist, transform.position.z);
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
