using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject[] buttons;
    [SerializeField] GameObject[] images;
    [SerializeField] Material screenMaterial;
    [SerializeField] Camera screenCam;
    [SerializeField] Canvas screenCanvas;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            StartCoroutine(LerpColor(new Color(0.02352941f, 0, 0.4156863f, 1f)));
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            StartCoroutine(LerpColor(new Color(0, 0, 0, 0)));
        }
    }

    IEnumerator LerpColor(Color targetColor)
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Color currentColor = screenCam.backgroundColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            screenCam.backgroundColor = Color.Lerp(currentColor, targetColor, elapsedTime / duration);
            yield return null;
        }
        screenCam.backgroundColor = targetColor;
        yield return new WaitForSeconds(1);
        ActivateCanvas();
    }
    void ActivateCanvas()
    {
        screenCanvas.enabled = true;
        buttons[0].SetActive(true);
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider collider = collision.GetContact(0).thisCollider;
        Destroy(collision.gameObject);

        // Find the index of the collided collider in the array of buttons
        int colliderIndex = -1;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == collider.gameObject)
            {
                colliderIndex = i;
                break;
            }
        }

        // Use the collider index in a switch statement
        switch (colliderIndex)
        {
            case 0:
                print("0");
                buttons[0].SetActive(false);
                images[0].SetActive(false);
                buttons[1].SetActive(true);
                images[1].SetActive(true);
                break;
            case 1:
                print("1");
                SceneManager.LoadScene("GunTest");
                break;
            default:
                print("not in index");
                break;
        }
    }
}