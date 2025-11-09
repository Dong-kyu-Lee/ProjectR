using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DashGhost : MonoBehaviour
{
    public float lifeTime = 0.25f;
    private float timer;
    private SpriteRenderer sr;
    private Color startColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifeTime;

        if (sr != null)
        {
            var c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0f, t);
            sr.color = c;
        }

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}
