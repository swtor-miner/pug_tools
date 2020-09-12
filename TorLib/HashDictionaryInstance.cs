using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nsHashDictionary;

namespace TorLib
{
    public class HashDictionaryInstance 
    {
        private static readonly HashDictionaryInstance instance = new HashDictionaryInstance();
        public HashDictionary dictionary = new HashDictionary();
        public bool Loaded;
        //private bool disposed = false;

        static HashDictionaryInstance()
        {
        }

        HashDictionaryInstance()
        {
            dictionary.LoadHashList();
            Loaded = true;
        }

        public static HashDictionaryInstance Instance
        {            
            get
            {              
                return instance;
            }
        }

        public void Unload()
        {
            dictionary = new HashDictionary();
            Loaded = false;
            GC.Collect();
        }

        public void Load()
        {
            if(Loaded)
            {
                //Already loaded.
                return;
            }

            dictionary.LoadHashList();
            Loaded = true;
        }
    }    
}
