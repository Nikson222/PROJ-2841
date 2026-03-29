using System;

namespace Core.Infrastructure.SaveLoad
{
    public interface ISavable
    {
        Type DataType { get; }
        string SavePath { get; }
        
        object GetData();
        void SetData(object data);
        void SetInitialData();
    }
}