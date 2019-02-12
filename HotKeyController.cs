using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIRASupport
{
    enum RegisterType
    {
        ADD,
        REMOVE,
        REMOVEALL,
    }

    internal class HotKeyController
    {
        List<KeyHandler> hotKeys = new List<KeyHandler>();

        public delegate void RegisterHotKeyDelegate (KeyHandler key, RegisterType type);

        public event RegisterHotKeyDelegate RegisterHotKeyEvent;

        public void RegisterHotKey()
        {
            bool stateChange = false;
            foreach (KeyHandler key in hotKeys)
            {
                if (key.Registered)
                    continue;

                // if the key is not register through here, then we don't unregister it
                key.Register();
                stateChange = true;
            }
            if (RegisterHotKeyEvent != null && stateChange)
            {
                RegisterHotKeyEvent(null, RegisterType.ADD);
            }
        }

        public void AddHotKey(KeyHandler key)
        {
            hotKeys.Add(key);
        }

        private void RemoveHotKey(KeyHandler key)
        {
            if (key.Unregiser())
            {
                hotKeys.Remove(key);
            }
        }

        public void UnregisterAllHotKey()
        {
            // unregister 
            bool stateChange = false;
            foreach (KeyHandler key in hotKeys)
            {
                if (!key.Registered)
                    continue;

                key.Unregiser();
                stateChange = true;
            }

            //hotKeys.Clear();

            if (RegisterHotKeyEvent != null && stateChange)
            {
                RegisterHotKeyEvent(null, RegisterType.REMOVEALL);
            }
        }
    }
}
