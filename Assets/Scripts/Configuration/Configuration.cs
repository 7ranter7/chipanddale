using UnityEngine;

namespace ChipNDale
{
    [CreateAssetMenu]
    public class Configuration : ScriptableObject
    {
        public BossConfiguration BossConfiguration;
        public Vector2 gravity = new Vector2(0, -0.98f);
        public PlayerConfiguration PlayerConfiguration;
        public StaticMapView StaticMapViewPrefab;
        public ItemsConfiguration ItemsConfiguration;
        public Vector2 globalMaxVelocity;
    }
}
