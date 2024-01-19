using CiccioSoft.VirtualList.Sample.Uwp.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace CiccioSoft.VirtualList.Sample.Uwp.Database
{
    public class SampleDataService
    {
        public List<Model> Generate(int total = 10000)
        {
            var list = new List<Model>(total);
            for (var i = 1; i <= total; i++)
            {
                var str_build = new StringBuilder();
                var random = new Random();
                char letter;
                for (var l = 0; l < 7; l++)
                {
                    var flt = random.NextDouble();
                    var shift = Convert.ToInt32(Math.Floor(26 * flt));
                    letter = Convert.ToChar(shift + 65);
                    str_build.Append(letter);
                }
                var model = new Model((uint)i, str_build.ToString());
                list.Add(model);
            }
            return list;
        }

        public List<Model> ReadFromFile(string filePath)
        {
            //Windows.Storage.StorageFolder storageFolder =
            //    Windows.Storage.ApplicationData.Current.LocalFolder;
            //Windows.Storage.StorageFolder storageFolder =
            //    Windows.ApplicationModel.Package.Current.InstalledLocation;
            //StorageFile file = storageFolder.GetFileAsync(filePath).AsTask().Result;

            //StorageFile file = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///appsettings.json")).AsTask().Result;
            StorageFile file = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + filePath)).AsTask().Result;

            var fileContent = FileIO.ReadTextAsync(file).AsTask().Result;
            return JsonConvert.DeserializeObject<List<Model>>(fileContent);
        }
    }
}
