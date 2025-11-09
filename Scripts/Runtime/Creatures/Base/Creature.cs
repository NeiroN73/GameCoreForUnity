using GameCore.Creatures;
using UnityEngine;

namespace Game.Creatures
{
    public abstract class Creature : MonoBehaviour, ICreature
    {
        public void Inject()
        {
            
        }

        public string Id { get; }
    }
}