using System.Windows.Controls;

namespace Personnel_Division.VisualComponents
{
    public partial class UserInfo : UserControl
    {
        public UserInfo()
        {
            InitializeComponent();
        }

        public void FillUserData(string login, string name)
        {
            loginTextBlock.Text = $"Логин: {login}";
            nameTextBlock.Text = $"Имя: {name}";
        }
    }
}
