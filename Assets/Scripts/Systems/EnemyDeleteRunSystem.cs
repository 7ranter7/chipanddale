using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    sealed class EnemyDeleteRunSystem : IEcsRunSystem
    {
        // auto-injected fields.
        readonly EcsWorld _world = null;
        private Configuration _configuration;
        private SceneData _sceneData;
        private EcsFilter<Enemy,EnemyViewRef,Dead> _filter;


        void IEcsRunSystem.Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                ref var enemy = ref _filter.Get1(index);
                ref var view = ref _filter.Get2(index);
                //GameObject.Destroy(view.Value);
                //entity.Destroy();
            }

            // add your run code here.
        }
    }
}
