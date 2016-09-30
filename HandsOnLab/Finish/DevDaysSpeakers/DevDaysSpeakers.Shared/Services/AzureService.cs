﻿using DevDaysSpeakers.Services;
using DevDaysSpeakers.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Diagnostics;

[assembly: Dependency(typeof(AzureService))]
namespace DevDaysSpeakers.Services
{
    public class AzureService
    {
        public MobileServiceClient Client { get; set; } = null;
        IMobileServiceSyncTable<Speaker> table;

        public async Task Initialize()
        {
            if (Client?.SyncContext?.IsInitialized ?? false)
                return;

            var appUrl = "https://montemagnospeakers.azurewebsites.net";

            //Create our client
            Client = new MobileServiceClient(appUrl);

            //InitialzeDatabase for path
            var path = InitializeDatabase();

            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);

            //Define table
            store.DefineTable<Speaker>();

            //Initialize SyncContext
            await Client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            table = Client.GetSyncTable<Speaker>();
        }

        private string InitializeDatabase()
        {
#if __ANDROID__ || __IOS__
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
#endif
            SQLitePCL.Batteries.Init();

            var path = "syncstore.db";

#if __ANDROID__
            path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), path);

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
#endif

            return path;
        }



        public async Task<IEnumerable<Speaker>> GetSpeakers()
        {
            await Initialize();
            await SyncSpeakers();
            return await table.OrderBy(s => s.Name).ToEnumerableAsync();
        }


        public async Task SyncSpeakers()
        {
            try
            {
                await Client.SyncContext.PushAsync();
                await table.PullAsync("allSpeakers", table.CreateQuery());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync speakers, that is alright as we have offline capabilities: " + ex);
            }

        }
    }
}