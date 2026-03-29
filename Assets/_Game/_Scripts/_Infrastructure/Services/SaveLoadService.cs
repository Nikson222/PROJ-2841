using System;
using System.Collections.Generic;
using System.IO;
using _Scripts.Data;
using Core.Infrastructure.SaveLoad;
using Newtonsoft.Json;
using Zenject;

namespace _Scripts._Infrastructure.Services
{
    public class SaveLoadService : IInitializable, IDisposable
    {
        private readonly List<ISavable> _savableModels;

        public SaveLoadService(List<ISavable> savableModels)
        {
            _savableModels = new List<ISavable>(savableModels);
        }

        public void Initialize()
        {
            LoadAll();
        }

        public void SaveData(ISavable model) 
        {
            var path = model.SavePath;
            var data = model.GetData();
            WriteFile(data, path);
        }

        public void DeleteData(string path)
        {
            File.Delete(path);
        }

        public void DeleteAll()
        {
            foreach (var model in _savableModels)
            {
                File.Delete(model.SavePath);
            }
        }

        public void LoadData(ISavable model)
        {
            var dataType = model.DataType;
            var data = ReadFile(model.SavePath, dataType);

            if (data != null && dataType.IsInstanceOfType(data))
            {
                model.SetData(data);
            }
        }

        private void SaveAll()
        {
            foreach (var model in _savableModels)
            {
                var data = model.GetData();
                WriteFile(data, model.SavePath);
            }
        }

        private void LoadAll()
        {
            foreach (var model in _savableModels)
            {
                var dataType = model.DataType;
                var data = ReadFile(model.SavePath, dataType);

                if (data != null && dataType.IsInstanceOfType(data))
                    model.SetData(data);
                else
                    model.SetInitialData();
            }
        }

        private void WriteFile(object data, string path)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            var jsonString = JsonConvert.SerializeObject(data, settings);
    
            File.WriteAllText(path, jsonString);
        }

        private object ReadFile(string path, Type dataType)
        {
            if (File.Exists(path))
            {
                string fileContent = File.ReadAllText(path);
                return JsonConvert.DeserializeObject(fileContent, dataType, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
            }

            return null;
        }

        public void Dispose()
        {
            SaveAll();
        }
    }
}