using System;
using System.IO;
using CasinoGame.Interfaces;

namespace CasinoGame.Services
{
    public class FileSystemSaveLoadService : ISaveLoadService<string>
    {
        private readonly string _savePath;

        public FileSystemSaveLoadService(string savePath)
        {
            _savePath = savePath ?? throw new ArgumentNullException(nameof(savePath));

            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
        }

        public void SaveData(string data, string identifier)
        {
            try
            {
                string filePath = Path.Combine(_savePath, $"{identifier}.txt");
                File.WriteAllText(filePath, data);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save data: {ex.Message}", ex);
            }
        }

        public string LoadData(string identifier)
        {
            try
            {
                string filePath = Path.Combine(_savePath, $"{identifier}.txt");

                if (!File.Exists(filePath))
                {
                    return null;
                }

                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load data: {ex.Message}", ex);
            }
        }
    }
}