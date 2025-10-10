using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var stageManager = other.GetComponent<PlayerStageManager>();
        if (stageManager != null)
        {
            stageManager.GoToNextStage();
        }
    }
}
