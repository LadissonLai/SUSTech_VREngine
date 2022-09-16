using VRTK;

namespace VRTKExtensions
{
    public interface IFocusAble
    {
        void OnInteractableObjectFocusEnter(InteractableObjectEventArgs e);
        void OnInteractableObjectFocusExit(InteractableObjectEventArgs e);
        void OnInteractableObjectFocusSet(InteractableObjectEventArgs e);
    }
}

