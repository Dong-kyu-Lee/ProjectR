using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWarpPoint : MonoBehaviour
{
    public SceneType sceneType;
    public string sceneName;
    private bool isTriggered = false;
    private SpriteRenderer spriteRenderer;
    private Color original;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        original = spriteRenderer.color;
    }

    private void Update()
    {
        if (isTriggered && Input.GetKeyDown(KeyCode.E))
        {
            GameManager.Instance.MoveScene(sceneType, sceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;
        isTriggered = true;
        spriteRenderer.color = Color.red;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isTriggered) return;
        isTriggered = false;
        spriteRenderer.color = original; 
    }
}
