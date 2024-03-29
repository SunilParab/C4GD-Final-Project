using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

    public bool swinging;
    public bool cooldown;
    public BoxCollider2D hitbox;
    public Animator animator;
   
    // Start is called before the first frame update
    void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
        hitbox.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Fire1") > 0 && !cooldown)
        {
            cooldown = true;
            swinging = true;
            animator.SetBool("Swinging", true);
            hitbox.enabled = true;
            StartCoroutine(Swing());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (swinging && other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.5f);
        swinging = false;
        animator.SetBool("Swinging", false);
        hitbox.enabled = false;
        yield return new WaitForSeconds(0.5f);
        cooldown = false;
    }
}
