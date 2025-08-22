using UnityEngine;

namespace GameCore.Handlers
{
    public abstract class Handler : MonoBehaviour, IHandlerable
    {
        [field: SerializeField] public string Id { get; private set; }
    }
}