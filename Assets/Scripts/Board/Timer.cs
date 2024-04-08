using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float TimeLeft;
    public bool TimerOn = false;

    void Start()
    {
    }

    void Update()
    {
        if (TimerOn)
        {
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);
            }
            else
            {
                UIManager.instance.HideCards();
                Destroy(GameObject.FindWithTag("CancelAttackButton"));
                GameManager.instance.EndTurn();
            }
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime/ 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        GetComponent<TextMeshProUGUI>().text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
}
