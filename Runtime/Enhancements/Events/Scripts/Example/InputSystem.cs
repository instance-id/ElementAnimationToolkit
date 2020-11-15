using instance.id.EATK.Events;
using UnityEngine;

namespace Example
{
    public class InputSystem : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                EventBus.RaiseEvent<IQuickSaveLoadHandler>(h => h.HandleQuickSave());
            }
            
            if (Input.GetKeyDown(KeyCode.L))
            {
                EventBus.RaiseEvent<IQuickSaveLoadHandler>(h => h.HandleQuickLoad());
            }
        }
    }
}
