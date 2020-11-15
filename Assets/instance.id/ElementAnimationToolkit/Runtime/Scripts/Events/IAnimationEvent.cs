// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit         --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using UnityEditor;

namespace instance.id.EATK.Events
{
    public interface IAnimationEvent : IGlobalSubscriber
    {
        void Start();
        void Stop();
    }
}
