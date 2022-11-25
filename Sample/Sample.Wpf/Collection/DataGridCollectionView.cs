using CiccioSoft.VirtualList.Sample.Repository;
using CiccioSoft.VirtualList.Sample.Domain;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CiccioSoft.VirtualList.Sample.Wpf.Collection
{
    public class DataGridCollectionView : ListCollectionView
    {
        private readonly int count = 0;
        private readonly List<Model> fakes = new();
        public DataGridCollectionView()
            : base(new List<Model>())
        {
            //fakes = Ioc.Default.GetRequiredService<IModelRepository>().GetAll();
            //count = fakes.Count;
        }

        public override int Count => count;

        public override object GetItemAt(int index)
        {
            return fakes[index];
        }
    }
}
