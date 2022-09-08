using System;
using System.Collections.Generic;

namespace Datacom.Envirohack.Services
{
    public partial class StorageService {
        static StorageService instance;
        public static StorageService Instance
        {
            get {
                if(instance == null) {
                    var storageName = Environment.GetEnvironmentVariable("StorageAccountName");
                    var storageKey = Environment.GetEnvironmentVariable("StorageAccountKey");
                    instance = new StorageService(storageName, storageKey);
                }
                
                return instance;
            }
        }
    }
}
