using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;

public class Grenadier : MilitaryUnit, IAOEAttacker
{
    [SerializeField] private GameObject grenade;
    [SerializeField] private ParticleSystem explosionParticles;
    [field: SerializeField] public UnitAOEAttackConfigSO AOEAttackConfigSO { get; private set; }

    private Transform grenadeParent;
    private Vector3 defaultGrenadePosition; //紀錄炸彈在parent上的位置

    //AOE相關
    private Collider[] damageablColliders;

    protected override void Awake()
    {
        base.Awake(); //呼叫父類的Awake方法
        if (grenade == null || explosionParticles == null)
        {
            Debug.Log($"Grenadier {name} is missing a grenade or explosion particles!");
            return;
        }
        // 初始化
        defaultGrenadePosition = grenade.transform.localPosition;
        grenadeParent = grenade.transform.parent;
    }

    protected override void Start()
    {
        base.Start();
        //初始化AOE傷害collider陣列的長度
        damageablColliders = new Collider[AOEAttackConfigSO.MaxAOEHitNumber];
    }

    //Animation Event，別刪除唷!
    public void OnThrowGrenadeAnimation()
    {
        //丟出離開parent
        grenade.transform.SetParent(null);

        //計算目標起點與終點
        Vector3 startPosition = grenade.transform.position;
        Vector3 endPosition = grenade.transform.position + grenade.transform.forward * 3; //預設下往前丟3倍的距離

        if (behaviorAgent.GetVariable("TargetGameObject", out BlackboardVariable<GameObject> targetGameObject)
            && targetGameObject != null)
        {
            endPosition = targetGameObject.Value.transform.position + Vector3.up; //丟到該目標位置
        }
        else if (behaviorAgent.GetVariable("TargetLocation", out BlackboardVariable<Vector3> targetLocation))
        {
            //如果目標死掉了，依然丟到該位置上，才自然?!
            endPosition = targetLocation;
        }

        //開始移動到該目標
        StartCoroutine(AnimateGrenadeMovement(startPosition, endPosition));

    }
    private IEnumerator AnimateGrenadeMovement(Vector3 startPosition, Vector3 endPosition)
    {
        float time = 0;
        const float speed = 2;

        while (time < 1)
        {
            grenade.transform.position = Vector3.Lerp(startPosition, endPosition, time);
            time += Time.deltaTime * speed;
            yield return null; //出去等待下一偵
        }

        //炸彈到位後，炸彈粒子跑到目標點上撥放爆炸效果
        explosionParticles.transform.SetParent(null); //粒子系統離開parent
        explosionParticles.transform.position = endPosition;
        explosionParticles.Play();

        //給範圍傷害
        GiveAoeDamage(endPosition);

        //最後炸彈重新回到手上，準備下一次丟
        grenade.transform.SetParent(grenadeParent);
        grenade.transform.localPosition = defaultGrenadePosition;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        //因為這兩個物件可能在該單位死掉時，脫離出去了
        //因此還得手動清除，確保萬無一失唷!
        Destroy(grenade);
        Destroy(explosionParticles.gameObject);
    }
    public void GiveAoeDamage(Vector3 AoeCenterPoint)
    {
        int hits = Physics.OverlapSphereNonAlloc(
            AoeCenterPoint,
            AOEAttackConfigSO.AOERasius,
            damageablColliders,
            AOEAttackConfigSO.DamageableLayers
        );
        for (int i = 0; i < hits; i++)
        {
            if (damageablColliders[i].TryGetComponent(out IDamageable nearbyDamageable))
            {
                nearbyDamageable.TakeDamage(CalculateAOEDamage(AoeCenterPoint, nearbyDamageable.Transform.position));
            }
        }
    }
    //計算AOE傷害的方法，傳入爆炸中心點、目標位置
    //爆炸中心接受全傷害，越往外圍傷害愈小
    public int CalculateAOEDamage(Vector3 impactPoint, Vector3 targetPosition)
    {
        float distance = Vector3.Distance(impactPoint, targetPosition);
        int damage = unitSO.AttackConfigSO.Damage;
        return Mathf.Clamp(Mathf.CeilToInt(damage * (1 - distance / AOEAttackConfigSO.AOERasius)), 0, damage);
    }
}