using System.Windows.Forms;

namespace SchoolDocManager
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // Передаем текущую ссылку на форму в MainMenu
            MainMenu mainMenu = new MainMenu(this);

            SelectUserControl(mainMenu);
        }

        public void SelectUserControl(UserControl userControl)
        {
            // Очищаем панель перед добавлением нового UserControl
            MainPanel.Controls.Clear();

            // Добавляем UserControl на панель
            MainPanel.Controls.Add(userControl);

            // Опционально: настройка свойств UserControl
            userControl.Dock = DockStyle.Fill;
        }
    }
}

