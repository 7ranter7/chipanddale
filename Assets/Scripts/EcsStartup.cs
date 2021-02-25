using Leopotam.Ecs;
#if UNITY_EDITOR
using Leopotam.Ecs.UnityIntegration;
#endif
using UnityEngine;

namespace ChipNDale
{
    internal sealed class EcsStartup : MonoBehaviour
    {
        private EcsSystems _systemsFixedUpdate;
        private EcsSystems _systemsUpdate;
        private EcsWorld _world;
        [SerializeField] private Configuration Configuration;
        [SerializeField] private SceneData SceneData;


        private void Start()
        {
            SceneData.height = SceneData.MainCamera.orthographicSize;
            SceneData.width = SceneData.height * ((float) Screen.width / (float) Screen.height);
            Application.targetFrameRate = 60;
            // void can be switched to IEnumerator for support coroutines.

            _world = new EcsWorld();
            _systemsUpdate = new EcsSystems(_world);
            _systemsFixedUpdate = new EcsSystems(_world);
#if UNITY_EDITOR
            EcsWorldObserver.Create(_world);
            EcsSystemsObserver.Create(_systemsUpdate);
            EcsSystemsObserver.Create(_systemsFixedUpdate);
#endif

            _systemsFixedUpdate
                //.Add(new CollidePlayerRunSystem())
                
                
                //.Add(new PhysicsCollisionRunSystem())
                //.Add(new PhysicsCollisionHandlerRunSystem())
                .Add(new EnemyRunSystem())
                .Add(new BossRunSystem())
                .Add(new ItemCollisionsRunSystem())
                .Add(new ItemRunSystem())
                .Add(new UnityCollisionHandler())
                .Add(new EnemyMoveViewRunSystem())
                .Add(new BossMoveViewRunSystem())
                .Add(new PlayerMoveViewRunSystem())
                .Add(new ItemMoveViewRunSystem())
                .Add(new PhysicsMoveRunSystem())
                .Add(new PlayerMoveUnityRigidbodyRunSystem())
                .Add(new ItemMoveUnityRigidbodyRunSystem())
                .Add(new BossMoveUnityRigidbodyRunSystem())
                .Add(new EnemyMoveUnityRigidbodyRunSystem())
                .Add(new BossDeleteRunSystem())
                .Add(new EnemyDeleteRunSystem())
                
                .Inject(Configuration)
                .Inject(SceneData)
                .Init();
            _systemsUpdate
                // register your systems here, for example:
                .Add(new FightInitSystem())
                .Add(new CreateMapViewSystem())
                .Add(new CreateBossViewSystem())
                .Add(new CreatePlayerViewSystem())
                .Add(new CreateItemsViewRunSystem())
                .Add(new CreateEnemyViewSystem())
                .Add(new PlayerControlRunSystem())
                .Add(new PlayerRunSystem())
                
                // register one-frame components (order is important), for example:
                // .OneFrame<TestComponent1> ()
                // .OneFrame<TestComponent2> ()

                // inject service instances here (order doesn't important), for example:
                .Inject(Configuration)
                .Inject(SceneData)
                // .Inject (new NavMeshSupport ())
                .Init();
            
        }

        private void Update()
        {
            _systemsUpdate?.Run();
        }

        private void FixedUpdate()
        {
            _systemsFixedUpdate?.Run();
        }

        private void OnDestroy()
        {
            if (_systemsUpdate != null)
            {
                _systemsFixedUpdate.Destroy();
                _systemsFixedUpdate = null;
                _systemsUpdate.Destroy();
                _systemsUpdate = null;
                _world.Destroy();
                _world = null;
            }
        }
    }
}
