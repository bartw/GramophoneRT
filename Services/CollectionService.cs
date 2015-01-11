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
    public class CollectionService
    {
        private readonly string _fileName = "GramophoneCollection";

        private List<MyCollectionItem> _collection;

        public CollectionService()
        {
            _collection = new List<MyCollectionItem>();
        }

        public async Task InitAsync()
        {
            await LoadAsync();
        }

        public async Task SyncAsync(IClient discogs, string tokenKey, string tokenSecret, string username)
        {
            _collection.Clear();

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

        public IEnumerable<MyCollectionItem> GetPagedItems(int page, int pageSize)
        {
            return _collection.Skip(page * pageSize).Take(pageSize);
        }

        public int Count()
        {
            return _collection.Count;
        }

        public int CopiesInCollection(int id)
        {
            var foundItem = (from item in _collection
                             where item.Id == id
                             select item).FirstOrDefault();

            if (foundItem != null)
            {
                return foundItem.InstanceIds.Count;
            }
            else
            {
                return 0;
            }
        }

        private async Task<int> SyncPageAsync(IClient discogs, string tokenKey, string tokenSecret, string username, int page)
        {
            var response = await discogs.GetFolderReleasesAsync(tokenKey, tokenSecret, username, page: page);

            if (response != null && response.Data != null)
            {
                if (response.Data.releases != null && response.Data.releases.Count > 0)
                {
                    foreach (var item in response.Data.releases)
                    {
                        var foundItem = (from collectionItem in _collection
                                         where item.id == collectionItem.Id
                                         select collectionItem).FirstOrDefault();

                        if (foundItem != null)
                        {
                            if (!foundItem.InstanceIds.Contains(item.instance_id))
                            {
                                foundItem.InstanceIds.Add(item.instance_id);
                            }
                        }
                        else
                        {
                            var myItem = new MyCollectionItem(item);
                            _collection.Add(myItem);
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
            var json = JsonConvert.SerializeObject(_collection);
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

                var collection = JsonConvert.DeserializeObject<List<MyCollectionItem>>(json);

                if (collection != null && collection.Count > 0)
                {
                    _collection = collection;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
