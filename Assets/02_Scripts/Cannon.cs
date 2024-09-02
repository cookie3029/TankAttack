using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private float force = 1200.0f;
    [SerializeField] private GameObject expEffect;

    // 발사한 유저의 식별 ID
    public int shooterID;

    void Awake()
    {
        expEffect = Resources.Load<GameObject>("BigExplosion");
    }

    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * force);
        Destroy(this.gameObject, 5.0f);
    }

    private void OnCollisionEnter(Collision coll)
    {
        var obj = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(obj, 5.0f);
        Destroy(this.gameObject);
    }
}
