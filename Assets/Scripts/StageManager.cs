using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    public EnemyMove enemyMove;

    // �������� ������ ����Ʈ
    private List<GameObject> items = new List<GameObject>();
    // Enemy ��ü�� ��ġ ������ �����ϱ� ���� ������ ����
    private Dictionary<GameObject, Vector3> enemyPositions = new Dictionary<GameObject, Vector3>();

    private void Start()
    {
        // ���� ���� �� �����۰� ������ ������ �����Ͽ� ����
        GameObject[] itemArray = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in itemArray)
        {
            items.Add(item);
        }

        GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyArray)
        {
            enemyPositions.Add(enemy, enemy.transform.position);
        }
    }

    public void ResetItemsAndEnemies()
    {
        
        foreach (GameObject item in items)
        {
            // ������ Ȱ��ȭ
            item.SetActive(true);
        }

        // ������ �ʱ� ���·� �ǵ���
        foreach (KeyValuePair<GameObject, Vector3> enemyPosition in enemyPositions)
        {
            GameObject enemy = enemyPosition.Key;
            Vector3 initialPosition = enemyPosition.Value;
           
            // ���� ���� �ʱ�ȭ
            enemy.SetActive(true);
            enemy.GetComponent<EnemyMove>()?.ResetEnemy();
            enemy.transform.position = initialPosition; // ���� ��ġ�� �ʱ� ��ġ�� ����           
        }
    }
}
