// using UnityEngine;

// public class ExitTrigger : MonoBehaviour
// {
//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             Debug.Log("탈출 성공!");
//         }
//     }
// }



using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private bool playerInside = false;
    private ItemCollector collector;

    void Start()
    {
        collector = GameObject.FindGameObjectWithTag("Player").GetComponent<ItemCollector>();
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.R))
        {
            if (collector.AllCollected())
            {
                Debug.Log("Clear! 모든 아이템을 모았습니다!");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
            else
            {
                Debug.Log("아직 아이템을 다 모으지 않았습니다!");
            }
        }
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
