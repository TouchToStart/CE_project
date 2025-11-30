using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    private bool playerInside = false;
    private ItemCollector collector;

    public string escapeSceneName = "EscapeScene"; // 이동할 씬 이름

    void Start()
    {
        // Player 오브젝트에서 ItemCollector 가져오기
        collector = GameObject.FindGameObjectWithTag("Player")
                              .GetComponent<ItemCollector>();
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            if (collector.HasEnoughItems())
            {
                Debug.Log("탈출 성공! 씬 이동");
                GoToEscapeScene();
            }
            else
            {
                int need = collector.requiredCount - collector.collected;
                Debug.Log("아이템이 부족합니다! (앞으로 " + need + "개 더 필요)");
            }
        }
    }

    void GoToEscapeScene()
    {
        SceneManager.LoadScene(escapeSceneName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}
