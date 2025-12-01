using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CasinoGame.Interfaces
{
    public interface ISaveLoadService<T>
    {
        void SaveData(T data, string identifier);
        T LoadData(string identifier);
    }
}