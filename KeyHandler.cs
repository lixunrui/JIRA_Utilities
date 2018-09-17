using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JIRAFolderOpener
{
    public static class Constants
    {
        //modifiers
        public const int NOMOD = 0x0000;
        public const int ALT = 0x0001;
        public const int CTRL = 0x0002;
        public const int SHIFT = 0x0004;
        public const int WIN = 0x0008;

        //windows message id for hotkey
        public const int WM_HOTKEY_MSG_ID = 0x0312;
    }

    internal class KeyHandler
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //[DllImport("kernel32.dll")]
        //static extern uint GetLastError();

        int key;
        IntPtr hWnd;
        int id;
        int modifier;

        bool _registered;
        public bool Registered
        {
            get { return _registered; }
        }
        public event EventHandler RegisterChanged;

        public KeyHandler(int modifier, Keys key, IntPtr Handle)
        {
            this.key = (int)key;
            this.hWnd = Handle;
            this.modifier = modifier;
            id = this.GetHashCode();
            _registered = false;
        }

        public KeyHandler(Keys key, IntPtr Handle)
        {
            this.key = (int)key;
            this.hWnd = Handle;
            this.modifier = 0x0005; // ALT + SHIFT
            id = this.GetHashCode();
            _registered = false;
        }

        public override int GetHashCode()
        {
            return key ^ hWnd.ToInt32();
        }

        public bool Register()
        {

            bool registered = false;

          //  UnregisterHotKey(hWnd, id);
            
            registered = RegisterHotKey(hWnd, id, modifier, key);

            if (registered)
            {
                _registered = true;
                if (RegisterChanged != null)
                    RegisterChanged(this, null);
            }


            return registered;
        }

        public bool Unregiser()
        {
            bool registered =  UnregisterHotKey(hWnd, id);
            if (registered)
            {
                _registered = false;
                if (RegisterChanged != null)
                    RegisterChanged(this, null);
            }
            return registered;
        }

    }
}
