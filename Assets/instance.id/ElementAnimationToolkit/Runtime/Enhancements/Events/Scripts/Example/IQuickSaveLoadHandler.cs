using instance.id.EATK.Events;

namespace Example
{
    public interface IQuickSaveLoadHandler : IGlobalSubscriber
    {
        void HandleQuickSave();
        void HandleQuickLoad();
    }
}
