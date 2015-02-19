using System;

namespace Sungiant.Djinn
{
    public abstract class Singleton<T> 
        where T : class
    {
        static readonly object mutex = new object();
        
        static volatile T instance;
        
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (mutex)
                    {
                        if (instance == null)
                        {
                            instance = Activator.CreateInstance(typeof(T), true) as T;
                        }
                    }
                }
                
                return instance;
            }
        }
        
        public static void Create(T zInstance)
        {
            if (instance == null)
            {
                instance = zInstance;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        
        public static void Destroy()
        {
            instance = null;
        }
        
        public static Boolean IsNull()
        {
            return instance == null;
        }
    }


}

