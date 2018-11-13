using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float speed;

    public int id;

    public bool dispersing;

    public float castRadius;

    public LayerMask eggLayers;

    public void Initialize(Vector2 position)
    {
        if (gameObject == null)
        {
            return;
        }
        dispersing = false;
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = (position - (Vector2)transform.position) * speed;
    }

    public void Initialize(float delay)
    {
        if (gameObject == null)
        {
            return;
        }

        dispersing = true;
        rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine(Delay(delay));
    }

    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay * 1.2f);
        dispersing = false;
        rb2d.isKinematic = true;
    }

    public void Update()
    {
        if (gameObject == null)
        {
            return;
        }
        if (!dispersing)
        {
            rb2d.velocity = rb2d.velocity.normalized * speed;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (dispersing)
        {
            return;
        }

        if (collision.collider.CompareTag("Bubble"))
        {
            rb2d.isKinematic = true;
            rb2d.velocity = Vector2.zero;
            if (collision.collider.GetComponent<Bubble>().id == id)
            {
                ReactToEgg(castRadius);
            }
        }
    }

    public void ReactToEgg(float cas)
    {
        Destroy(gameObject);

        Collider2D[] eggs = Physics2D.OverlapCircleAll(transform.position, cas, eggLayers);
        foreach (Collider2D coll in eggs)
        {
            if (coll == null)
            {
                continue;
            }

            Bubble bubble = coll.GetComponent<Bubble>();
            if (bubble.id == id)
            {
                bubble.ReactToEgg(cas / 2f);
            }
        }
    }
}