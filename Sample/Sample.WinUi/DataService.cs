using CiccioSoft.VirtualList.Sample.WinUi.Database;
using CiccioSoft.VirtualList.Sample.WinUi.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.Storage;

namespace CiccioSoft.VirtualList.Sample.WinUi;

public static class DataService
{
    public static List<Model> ReadFromFile(string path)
    {
        //Windows.Storage.StorageFolder storageFolder =
        //    Windows.Storage.ApplicationData.Current.LocalFolder;
        //Windows.Storage.StorageFolder storageFolder2 =
        //    Windows.ApplicationModel.Package.Current.InstalledLocation;
        //StorageFile file = storageFolder.GetFileAsync(filePath).AsTask().Result;

        //StorageFile file = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///appsettings.json")).AsTask().Result;
        StorageFile file = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + path)).AsTask().Result;

        var json = FileIO.ReadTextAsync(file).AsTask().Result;

        //var json = File.ReadAllText(path);

        var list = JsonConvert.DeserializeObject<List<Model>>(json) ?? [];
        return list;
    }

    public static void WriteToFile(IList<Model> models, string path)
    {
        var fileContent = JsonConvert.SerializeObject(models);
        File.WriteAllText(path, fileContent, Encoding.UTF8);
    }

    public static void WriteToDb(IList<Model> models, AppDbContext dbContext)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        foreach (var item in models)
        {
            dbContext.Add(item);
        }
        dbContext.SaveChanges();
    }

    public static List<Model> Generate(int total = 10000)
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
}
