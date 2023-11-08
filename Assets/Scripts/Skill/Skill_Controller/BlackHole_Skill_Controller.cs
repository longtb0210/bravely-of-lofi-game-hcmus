using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;

    private int amountOfAttacks = 4;
    private float cloneAttackCoolDown = .3f;
    private float cloneAttackTimer;

    public List<Transform> targets = new List<Transform>();
    public List<GameObject> createHotKey = new List<GameObject>();

    public void SetupBlackHole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCoolDown)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCoolDown = _cloneAttackCoolDown;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), growSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }
    }

    private void ReleaseCloneAttack()
    {
        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;

        PlayerManager.instance.player.MakeTransprent(true);
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer >= 0 || !cloneAttackReleased)
        {
            return;
        }

        cloneAttackTimer = cloneAttackCoolDown;

        int randomIndex = Random.Range(0, targets.Count);

        float xOffset = Random.Range(0, 100) > 50 ? 2 : -2;

        SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
        amountOfAttacks--;

        if (amountOfAttacks <= 0)
        {
            Invoke("FinishBlackHoleAbility", 1);
        }
    }

    private void FinishBlackHoleAbility()
    {
        PlayerManager.instance.player.ExitBlackHoleAbility();
        canShrink = true;
        cloneAttackReleased = false;
    }

    private void DestroyHotKeys()
    {
        if (createHotKey.Count <= 0)
            return;

        for (int i = 0; i < createHotKey.Count; i++)
            Destroy(createHotKey[i]);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>()!= null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);
            CreateHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)=> collision.GetComponent<Enemy>()?.FreezeTime(false);
   

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("Not enough hot keys in a key code list!");
            return;
        }

        if (!canCreateHotKeys)
            return;

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        BlackHole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<BlackHole_HotKey_Controller>();

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
