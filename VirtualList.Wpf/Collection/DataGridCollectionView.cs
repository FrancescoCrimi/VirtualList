using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Data.Repository;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CiccioSoft.VirtualList.Wpf.Collection
{
    public class DataGridCollectionView : ListCollectionView
    {
        private readonly int count;
        private readonly List<Model> fakes;
        public DataGridCollectionView()
            : base(new List<Model>())
        {
            fakes = Ioc.Default.GetRequiredService<IModelRepository>().GetAll();
            count = fakes.Count;
        }

        public override int Count
        {
            get
            {
                return count;
            }
        }

        public override object GetItemAt(int index)
        {
            return fakes[index];
        }
    }
}
