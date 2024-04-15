using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    public EnemyMove enemyMove;

    // 아이템을 저장할 리스트
    private List<GameObject> items = new List<GameObject>();
    // Enemy 객체와 위치 정보를 연결하기 위한 데이터 구조
    private Dictionary<GameObject, Vector3> enemyPositions = new Dictionary<GameObject, Vector3>();

    private void Start()
    {
        // 게임 시작 시 아이템과 적들의 정보를 수집하여 저장
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
            // 아이템 활성화
            item.SetActive(true);
        }

        // 적들을 초기 상태로 되돌림
        foreach (KeyValuePair<GameObject, Vector3> enemyPosition in enemyPositions)
        {
            GameObject enemy = enemyPosition.Key;
            Vector3 initialPosition = enemyPosition.Value;
           
            // 적들 상태 초기화
            enemy.SetActive(true);
            enemy.GetComponent<EnemyMove>()?.ResetEnemy();
            enemy.transform.position = initialPosition; // 적의 위치를 초기 위치로 설정           
        }
    }
}
