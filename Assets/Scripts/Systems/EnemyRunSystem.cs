using Leopotam.Ecs;

namespace ChipNDale
{
    sealed class EnemyRunSystem : IEcsRunSystem
    {
        // auto-injected fields.
        readonly EcsWorld _world = null;
        private Configuration _configuration;
        private SceneData _sceneData;
        private EcsFilter<Enemy, Rigidbody>.Exclude<Dead> _filter;


        void IEcsRunSystem.Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                ref var boss = ref _filter.Get1(index);
                ref var rigidbody = ref _filter.Get2(index);

               

                if (rigidbody.Position.x > _sceneData.width + _sceneData.width/2 || rigidbody.Position.x <-_sceneData.width - _sceneData.width/2)
                {
                    rigidbody.Static = true;
                    rigidbody.HasCollider = false;
                    entity.Get<Dead>();
                }

            }

            // add your run code here.
        }
    }
}
