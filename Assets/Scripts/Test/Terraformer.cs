using UnityEngine;
using World;

namespace Test
{
    public class Terraformer : MonoBehaviour
    {
        public void Update()
        {
            var gameWorld = GameManager.Instance.World;

            var block = gameWorld.GetBlockUnderMouse();
            if (block == null)
                return;

            if (Input.GetMouseButtonDown(0))
                gameWorld.DigBlock(block);
            else if (Input.GetMouseButtonDown(1))
                gameWorld.AddBlock(block.worldPosition + Vector3.up);
        }
    }
}
