using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Data.Repository;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace WpfApp1
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Model> list;

        public MainWindow()
        {
            InitializeComponent();
            list = new FakeModelRepository().GetAll();
            //var modelRepository = Ioc.Default.GetRequiredService<IModelRepository>();
            //list = modelRepository.GetAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            CollectionViewSource modelViewSource = (CollectionViewSource)FindResource("modelViewSource");
            // Caricare i dati impostando la proprietà CollectionViewSource.Source:
            // modelViewSource.Source = [origine dati generica]
            modelViewSource.Source = list;
        }
    }
}
