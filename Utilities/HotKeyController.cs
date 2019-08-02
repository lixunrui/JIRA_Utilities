using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JIRASupport
{
    public enum RegisterStatus
    {
        SUCCESS,
        FAILURE,
    }

    public class HotKeyController
    {
        List<KeyHandler> hotKeys = new List<KeyHandler>();

        public delegate void RegisterHotKeyDelegate (KeyHandler key, RegisterStatus type);

        public event RegisterHotKeyDelegate RegisterHotKeyEvent;

        public bool RegisterHotKey(KeyHandler key)
        {
            bool _keyRegistered = false;

            if (key.Registered)
                _keyRegistered = true;

            _keyRegistered = key.Register();

            if(_keyRegistered)
            {
                RegisterHotKeyEvent?.Invoke(key, RegisterStatus.SUCCESS);
                hotKeys.Add(key);
            }
            else
                RegisterHotKeyEvent?.Invoke(key, RegisterStatus.FAILURE);

            return _keyRegistered;
        }

        public int Count()
        {
            return hotKeys.Count();
        }

        #region Abandoned Methods

       /// <summary>
       /// Do not use - abandoned
       /// </summary>
       /// <param name="key"></param>
        public void AddHotKey(KeyHandler key)
        {
            hotKeys.Add(key);
        }

        /// <summary>
        /// Do not use
        /// </summary>
        /// <param name="key"></param>
        public void UnregisterHotKey(KeyHandler key)
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
                RegisterHotKeyEvent(null, RegisterStatus.FAILURE);
            }
        }


        #endregion
    }
}
