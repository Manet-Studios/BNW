using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float speed;

    public int id;

    private bool dispersing;

    public void Initialize(Vector2 position)
    {
        dispersing = false;
        transform.right = Camera.main.ScreenToWorldPoint(position);
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.right * speed;
    }

    public void Initialize(float delay)
    {
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
            if (collision.collider.GetComponent<Bubble>().id == id)
            {
                SpawnFX();
                Destroy(gameObject);
            }
        }
    }

    public void SpawnFX()
    {
        FindObjectOfType<BubblePop>().spawnedBubbles--;
    }
}