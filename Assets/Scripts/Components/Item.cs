using Leopotam.Ecs;

namespace ChipNDale
{
    public struct Item
    {
        public bool Free;
        public EcsEntity Owner;
        public ItemView ViewPrefab;
    }
}
