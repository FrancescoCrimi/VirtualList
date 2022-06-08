using CiccioSoft.VirtualList.Data;
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
            list = new FakeModelRepository(10000).GetModels();
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
