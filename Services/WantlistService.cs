using BeeWee.DiscogsRT;
using BeeWee.DiscogsRT.Models;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Services
{
    public class WantlistService
    {
        private readonly string _fileName = "GramophoneWantlist";

        private List<MyWant> _wantlist;

        public WantlistService()
        {
            _wantlist = new List<MyWant>();
        }

        public async Task InitAsync()
        {
            await LoadAsync();
        }

        public async Task SyncAsync(IClient discogs, string tokenKey, string tokenSecret, string username)
        {
            _wantlist.Clear();

            int page = 1;
            var pages = await SyncPageAsync(discogs, tokenKey, tokenSecret, username, page);
            //var tasks = new List<Task>();

            for (int nextPage = page + 1; nextPage <= pages; nextPage++)
            {
                //tasks.Add(SyncPageAsync(discogs, tokenKey, tokenSecret, username, nextPage));
                await SyncPageAsync(discogs, tokenKey, tokenSecret, username, nextPage);
            }

            //Task.WaitAll(tasks.ToArray());

            await SaveAsync();
        }

        public bool InWantlist(int id)
        {
            return (from want in _wantlist
                    where want.Id == id
                    select want).Count() > 0;
        }

        public IEnumerable<MyWant> GetPagedItems(int page, int pageSize)
        {
            return _wantlist.Skip(page * pageSize).Take(pageSize);
        }

        public int Count()
        {
            return _wantlist.Count;
        }

        private async Task<int> SyncPageAsync(IClient discogs, string tokenKey, string tokenSecret, string username, int page)
        {
            var response = await discogs.GetWantsAsync(tokenKey, tokenSecret, username, page: page);

            if (response != null && response.Data != null)
            {
                if (response.Data.wants != null && response.Data.wants.Count > 0)
                {
                    foreach (var item in response.Data.wants)
                    {
                        if (!InWantlist(item.id))
                        {
                            var myItem = new MyWant(item);
                            _wantlist.Add(myItem);
                        }
                    }
                }

                if (response.Data.pagination != null)
                {
                    return response.Data.pagination.pages;
                }
            }

            return 0;
        }

        private async Task SaveAsync()
        {
            var json = JsonConvert.SerializeObject(_wantlist);
            var buffer = Encoding.UTF8.GetBytes(json.ToCharArray()).AsBuffer();

            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(_fileName, CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await stream.WriteAsync(buffer);
                await stream.FlushAsync();
            }
        }

        private async Task LoadAsync()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(_fileName);
                string json;

                using (var stream = await file.OpenReadAsync())
                using (var reader = new StreamReader(stream.AsStreamForRead()))
                {
                    json = await reader.ReadToEndAsync();
                }

                var wantlist = JsonConvert.DeserializeObject<List<MyWant>>(json);

                if (wantlist != null && wantlist.Count > 0)
                {
                    _wantlist = wantlist;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
