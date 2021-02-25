using Leopotam.Ecs;
using UnityEngine;

namespace ChipNDale
{
    sealed class CreateMapViewSystem : IEcsRunSystem
    {
        // auto-injected fields.
        readonly EcsWorld _world = null;

        private EcsFilter<Map>.Exclude<MapViewRef> _filter;
        private Configuration _configuration;
        private SceneData _sceneData;

        public void Run()
        {
            foreach (var index in _filter)
            {
                ref var entity = ref _filter.GetEntity(index);
                var mapView = Object.Instantiate(_configuration.StaticMapViewPrefab);
                ref var viewRef = ref entity.Get<MapViewRef>();
                viewRef.Value = mapView;


                var height = _sceneData.MainCamera.orthographicSize;
                var width = height * ((float) Screen.width / (float) Screen.height);

                var wall = new GameObject();
                wall.transform.SetParent(mapView.transform);
                var box = wall.AddComponent<BoxCollider2D>();
                box.size=new Vector2(1,height*3);
                box.offset=new Vector2(-width-0.5f,0);
               

                wall = new GameObject();
                wall.transform.SetParent(mapView.transform);
                box = wall.AddComponent<BoxCollider2D>();
                box.size=new Vector2(1,height*3);
                box.offset=new Vector2(width+0.5f,0);

                wall = new GameObject();
                wall.transform.SetParent(mapView.transform);
                box = wall.AddComponent<BoxCollider2D>();
                box.size=new Vector2(width*3,1);
                box.offset=new Vector2(0,height+0.5f);


                wall = new GameObject();
                wall.layer = 12;
                wall.transform.SetParent(mapView.transform);
                box = wall.AddComponent<BoxCollider2D>();
                box.size=new Vector2(width*3,1);
                box.offset=new Vector2(0,-height+0.5f);


                foreach (var collider2D in mapView.GetComponentsInChildren<Collider2D>())
                {
                    var colliderEntity = _world.NewEntity();
                    ref var collider = ref colliderEntity.Get<UnityColliderRef>();
                    collider.Value = collider2D.GetComponent<UnityCollider>();
                    if (collider.Value == null)
                    {
                        collider.Value = collider2D.gameObject.AddComponent<UnityCollider>();
                    }

                    collider.Value.currentEntity = colliderEntity;
                }
            }
        }
    }
}
