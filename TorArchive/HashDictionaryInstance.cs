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
        private bool disposed = false;

        static HashDictionaryInstance()
        {
        }

        HashDictionaryInstance()
        {
            this.dictionary.LoadHashList();
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
            GC.Collect();
        }     
    }    
}
