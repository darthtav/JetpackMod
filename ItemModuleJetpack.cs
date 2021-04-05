using ThunderRoad;
using UnityEngine;

namespace Jetpack
{
    class ItemModuleJetpack : ItemModule
    {
        public float thrust = 50f;


        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemJetpack>();
        }
    }
}
