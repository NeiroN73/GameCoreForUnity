using GameCore.Creatures;
using UnityEngine;

namespace Game.Creatures
{
    public abstract class Behaviour : MonoBehaviour, IBehaviour
    {
        public void Inject()
        {
            
        }

        public string Id { get; }
    }
}