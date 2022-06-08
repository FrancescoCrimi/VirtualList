using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();
            bindingSource1.DataSource = new BindingFakeList();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {

        }
    }
}
