using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject[] buttons;
    [SerializeField] GameObject[] images;
    [SerializeField] Material screenMaterial;
    [SerializeField] Camera screenCam;
    [SerializeField] Canvas screenCanvas;
    [SerializeField] Animator canvasAnimator;
    string mission;
    bool loggedIn = false;
    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.R))
    //    {
    //        StartCoroutine(LerpColor(new Color(0.02352941f, 0, 0.4156863f, 1f)));
    //    }
    //    else if (Input.GetKeyUp(KeyCode.F))
    //    {
    //        StartCoroutine(LerpColor(new Color(0, 0, 0, 0)));
    //    }
    //}

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
        if (loggedIn == false)
        {
            screenCanvas.enabled = true;
            buttons[0].SetActive(true);
            images[0].SetActive(true);
        }
    }
    void DeactivateItems()
    {
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
        foreach (GameObject image in images)
        {
            image.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        canvasAnimator.SetTrigger("Glitch");
        if (collision.collider.tag == "Player") return;
        Collider collider = collision.GetContact(0).thisCollider;

        //fins the collider that has been hit
        int colliderIndex = -1;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == collider.gameObject)
            {
                colliderIndex = i;
                break;
            }
        }

        //used for collider index
        switch (colliderIndex)
        {
            case 0:
                print("Logged in");
                loggedIn = true;
                buttons[0].SetActive(false);
                images[0].SetActive(false);
                buttons[1].SetActive(true);
                images[1].SetActive(true);
                buttons[2].SetActive(true);
                images[2].SetActive(true);
                buttons[3].SetActive(true);
                images[3].SetActive(true);
                break;
            case 1:
                print("mission 1");
                mission = "mission 1";
                DeactivateItems();
                buttons[4].SetActive(true);
                images[4].SetActive(true);
                buttons[5].SetActive(true);
                images[5].SetActive(true);
                break;
            case 2:
                print("mission 2");
                mission = "mission 2";
                DeactivateItems();
                buttons[4].SetActive(true);
                images[4].SetActive(true);
                buttons[5].SetActive(true);
                images[5].SetActive(true);
                break;
            case 3:
                print("mission 3");
                mission = "mission 3";
                DeactivateItems();
                buttons[4].SetActive(true);
                images[4].SetActive(true);
                buttons[5].SetActive(true);
                images[5].SetActive(true);
                break;
            case 4:
                print("AcceptMission");
                //load scene by name using string of mission
                //SceneManager.LoadScene(mission);
                SceneManager.LoadScene("GunTest");
                break;
            case 5:
                print("Back");
                DeactivateItems();
                buttons[0].SetActive(false);
                images[0].SetActive(false);
                buttons[1].SetActive(true);
                images[1].SetActive(true);
                buttons[2].SetActive(true);
                images[2].SetActive(true);
                buttons[3].SetActive(true);
                images[3].SetActive(true);
                break;
            default:
                print("not in index");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(LerpColor(new Color(0.02352941f, 0, 0.4156863f, 1f)));
    }
}
