using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JIRASupport
{
    public static class Constants
    {
        public enum ConstantKeys
        {
            NOMOD   = 0x0000,
            ALT     = 0x0001,
            CTRL    = 0x0002,   
            SHIFT   = 0x0004,
            WIN     = 0x0008,
            WM_HOTKEY_MSG_ID    = 0x0312,
        }

        public static string[] strConstantKeys = new string[] 
        {
            "NOMODE",
            "ALT",
            "CTRL",
            "SHIFT",
            "WIN",
            "WM_HOTKEY_MSG_ID"
        };
    }

    public class KeyHandler
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

        public string Key { get => key.ToString(); }

        bool _registered;
        public bool Registered
        {
            get { return _registered; }
        }
        public event EventHandler RegisterChanged; // not used

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
