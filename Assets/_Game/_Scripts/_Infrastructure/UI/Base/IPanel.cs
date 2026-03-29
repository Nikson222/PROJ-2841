using System;

namespace _Scripts._Infrastructure.UI.Base
{
    public interface IPanel
    {
        void Open();
        void Close(Action onClosed = null);
    }
}