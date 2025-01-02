using UnityEngine;

public class ChaseState : MonoBehaviour, IAIState
{
    private GameObject target; // Цель для преследования
    private EnemyMovement enemyMovement; // Ссылка на компонент EnemyMovement
    private float stopDistance = 2f; // Расстояние, на котором враг остановится перед целью
    private EnemyPerception enemyPerception;
    // Метод входа в состояние, принимает объект цели
    
    public void EnterState(GameObject owner)
    {
        enemyPerception = gameObject.GetComponent<EnemyPerception>();
        GameObject chaseTarget = enemyPerception.lastSeenTarget;
        enemyMovement = owner.GetComponent<EnemyMovement>();

        if (chaseTarget != null)
        {
            target = chaseTarget;
        }
        else
        {
            Debug.LogError("ChaseState: Цель для преследования не передана!");
        }
    }

    public string GetStateName()
    {
        return "Chase";
    }

    public void UpdateState()
    {
        if (target == null || enemyMovement == null) return;

        float distanceToTarget = Vector3.Distance(enemyMovement.transform.position, target.transform.position);

        // Если цель в пределах досягаемости, двигаемся к ней
        if (distanceToTarget > stopDistance)
        {
            enemyMovement.SetTarget(target);
        }
        else
        {
            // Здесь можно добавить логику, например, переход в состояние атаки
            Debug.Log("Цель достигнута!");
        }
    }

    public void ExitState()
    {
        // Очищаем цель при выходе из состояния
        target = null;
    }
}
