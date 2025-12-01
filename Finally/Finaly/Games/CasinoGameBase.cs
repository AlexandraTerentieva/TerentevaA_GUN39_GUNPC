using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoGame.Games
{
    public abstract class CasinoGameBase
    {
        public event Action OnWin;
        public event Action OnLoose;
        public event Action OnDraw;

        protected CasinoGameBase()
        {
            FactoryMethod();
        }

        protected abstract void FactoryMethod();
        public abstract void PlayGame();

        protected void OnWinInvoke() => OnWin?.Invoke();
        protected void OnLooseInvoke() => OnLoose?.Invoke();
        protected void OnDrawInvoke() => OnDraw?.Invoke();
    }
}

