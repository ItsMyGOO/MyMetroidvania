using UnityEngine;

public class GroundSensor : MonoBehaviour {
    [SerializeField] ColliderType collisionType;

    Vector2 colliderSize_Origin;
    CapsuleCollider2D capsule;
    BoxCollider2D box;
    CircleCollider2D circle;

    Rigidbody2D body;

    [field: SerializeField] public bool OnGround { get; private set; }
    [field: SerializeField] public bool OnOneWay { get; private set; }

    int solidLayer, oneWayLayer;
    public LayerMask SolidMask { get; private set; }
    public LayerMask OnewayMask { get; private set; }
    public LayerMask AllMask { get; private set; }

    void Awake() {
        body = transform.parent.GetComponent<Rigidbody2D>();

        const string solid = "Solid", oneWay = "OneWay";

        solidLayer = LayerMask.NameToLayer(solid);
        oneWayLayer = LayerMask.NameToLayer(oneWay);

        SolidMask = LayerMask.GetMask(solid);
        OnewayMask = LayerMask.GetMask(oneWay);
        AllMask = SolidMask | OnewayMask;
    }

    void Start() {
        switch (collisionType) {
            case ColliderType.Capsule:
                capsule = GetComponent<CapsuleCollider2D>();
                colliderSize_Origin = capsule.size;
                break;
            case ColliderType.Box:
                box = GetComponent<BoxCollider2D>();
                colliderSize_Origin = box.size;
                break;
            case ColliderType.Circle:
                circle = GetComponent<CircleCollider2D>();
                var radius = circle.radius;
                colliderSize_Origin = new Vector2(radius, radius);
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (!IsSenseTarget(other.gameObject.layer)) return;

        bool isDropping = body.velocity.y <= 0.01f;
        if (!isDropping) return;

        CheckCollision(other, other.gameObject.layer);
    }

    void OnCollisionStay2D(Collision2D other) {
        if (!IsSenseTarget(other.gameObject.layer)) return;
        CheckCollision(other, other.gameObject.layer);
    }

    void OnCollisionExit2D(Collision2D other) {
        if (!IsSenseTarget(other.gameObject.layer)) return;
        OnGround = false;
        OnOneWay = false;
    }

    bool IsSenseTarget(int nLayer) {
        return nLayer == solidLayer || nLayer == oneWayLayer;
    }

    void CheckCollision(Collision2D collision, int nLayer) {
        bool contactResult = false;
        for (int i = 0; i < collision.contactCount; i++) {
            const float overlapDistance = 0.35f;
            // todo debug
            // Debug.DrawRay(collision.GetContact(i).point, collision.GetContact(i).normal);
            contactResult |= collision.GetContact(i).normal.y >= overlapDistance;
        }

        if (nLayer == solidLayer) OnGround |= contactResult;
        else if (nLayer == oneWayLayer) OnOneWay |= contactResult;

        if (OnOneWay) OnGround = true;
    }

    enum ColliderType {
        Capsule,
        Box,
        Circle,
    }
}