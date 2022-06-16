using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnterLocationNotification : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI notificationText;

    private float timer = 0;
    private float showNotificationThreshold = 10;

    void Start()
    {
        Hide();
    }

    void Update()
    {
        // Return if area data not yet created
        if (AreaDataManager.Instance == null) { return; }

        // Only update if in world
        if (AreaDataManager.Instance.CurrentMapType != MapType.World)
        {
            return;
        }

        // Don't add to timer if menu is open
        if (UIModality.Instance.IsDialogueOpen ||
            UIModality.Instance.IsEscapeMenuOpen ||
            UIModality.Instance.IsInventoryOpen)
        {
            Hide();
            return;
        }

        // Don't add to timer if player is inputing directions 
        if (Input.GetAxisRaw("Vertical") != 0 ||
            Input.GetAxisRaw("Horizontal") != 0)
        {
            Hide();
            return;
        }

        timer += Time.deltaTime;
        if (timer > showNotificationThreshold)
        {
            StartCoroutine(Show());
            timer = 0;
        }
    }

    private void Hide()
    {
        StopAllCoroutines();
        notificationText.gameObject.SetActive(false);
    }

    private IEnumerator Show()
    {
        notificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);

        Hide();
    }
}
