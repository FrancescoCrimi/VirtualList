using System;
using System.Windows.Forms;

namespace VirtualList.Forms
{
    public partial class MainView : Form, IMainView
    {
        public MainView()
        {
            InitializeComponent();
            bindingSource1.DataSource = new BindingFakeList();
        }

        public event EventHandler? LoadEvent;
        public event EventHandler? CloseEvent;
        public event EventHandler? SearchEvent;

        private void MainView_Load(object sender, EventArgs e) => LoadEvent?.Invoke(sender, e);

        private void MainView_FormClosing(object sender, FormClosingEventArgs e) => CloseEvent?.Invoke(sender, e);

        private void SearchButton_Click(object sender, EventArgs e) => SearchEvent?.Invoke(sender, e);
    }
}