using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using Ookii.Dialogs.WinForms;
using static SchoolDocManager.MainMenu;
using System.Threading;
using Markdig;

namespace SchoolDocManager
{
    public partial class MainMenu : UserControl
    {
        private MainForm mainForm;

        private Dictionary<Control, Rectangle> originalSizes = new Dictionary<Control, Rectangle>();
        private Dictionary<Control, float> originalFontSizes = new Dictionary<Control, float>();
        private float originalFormWidth;
        private float originalFormHeight;

        // Указываем путь к файлу, где лежит и будет пересохраняться файл JSON
        string filePath = "PathFolder.json";

        public MainMenu(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        public class PathFolder
        {
            public string path { get; set; }
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            DisplayPathFromFile(filePath);

            originalFormWidth = this.Width;
            originalFormHeight = this.Height;

            foreach (Control ctrl in this.Controls)
            {
                originalSizes[ctrl] = new Rectangle(ctrl.Location, ctrl.Size);
                originalFontSizes[ctrl] = ctrl.Font.Size; // Запоминаем оригинальный размер шрифта
            }
        }

        private void ListFiles_Click(object sender, EventArgs e)
        {
            mainForm.SelectUserControl(new ListFilesPanel(mainForm));
        }

        private void SearchFiles_Click(object sender, EventArgs e)
        {
            mainForm.SelectUserControl(new SearchFiles(mainForm));
        }

        private void UpdateData_Click(object sender, EventArgs e) // запись в базу (таблицу)
        {
            string directoryPath = FolderPath.Text;

            // Создаем экземпляр DirectoryScanner
            DirectoryScanner scanner = new DirectoryScanner(ConfigurationManager.ConnectionStrings["FilesDB"].ConnectionString);

            // Записываем папки в базу данных
            scanner.SaveFoldersAndFilesToDatabase(directoryPath);

            MessageBox.Show("Папка успешно просканированна и данные записаны.", "Данные обновленны");
        }

        private void DisplayPathFromFile(string filePath) //Отобразить путь
        {
            // Чтение JSON строки из файла
            string json = File.ReadAllText(filePath);

            // Десериализация JSON строки в объект PathFolder
            PathFolder pathFolder = JsonConvert.DeserializeObject<PathFolder>(json);

            // Обновляем текст метки
            FolderPath.Text = pathFolder.path;

        }

        private void FolderPatChanged()
        {
            var pathFolder = new PathFolder
            {
                path = FolderPath.Text
            };

            string json = JsonConvert.SerializeObject(pathFolder, Formatting.Indented);

            // Записываем строку JSON в файл
            File.WriteAllText(filePath, json);
        }

        private void FolderPath_TextChanged(object sender, EventArgs e)
        {
            FolderPatChanged();
        }

        private void selectFolderButton_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new VistaFolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Выберите папку";
                folderBrowserDialog.UseDescriptionForTitle = true; // Использует описание в заголовке

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // Получаем выбранный путь к папке
                    string filePathFromDialog = folderBrowserDialog.SelectedPath;
                    FolderPath.Text = filePathFromDialog;

                }
            }

        }

        private void MainMenu_Resize(object sender, EventArgs e)
        {
            float scaleWidth = this.Width / originalFormWidth;
            float scaleHeight = this.Height / originalFormHeight;
            float scaleFactor = Math.Min(scaleWidth, scaleHeight); // Сохраняем пропорции

            foreach (Control ctrl in this.Controls)
            {
                if (originalSizes.ContainsKey(ctrl))
                {
                    Rectangle original = originalSizes[ctrl];

                    // Масштабируем расположение и размер элемента
                    ctrl.Location = new Point((int)(original.X * scaleWidth), (int)(original.Y * scaleHeight));
                    ctrl.Size = new Size((int)(original.Width * scaleWidth), (int)(original.Height * scaleHeight));

                    // Масштабируем шрифт, но не позволяем ему стать меньше 1
                    float newFontSize = originalFontSizes[ctrl] * scaleFactor;
                    newFontSize = Math.Max(newFontSize, 1); // Минимальный размер шрифта - 1

                    ctrl.Font = new Font(ctrl.Font.FontFamily, newFontSize, ctrl.Font.Style);
                }
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Получаем Markdown из ресурсов
                byte[] mdBytes = Properties.Resources.help;
                string markdownText = Encoding.UTF8.GetString(mdBytes);

                // 2. Конвертируем в HTML с поддержкой таблиц
                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()  // Включаем поддержку таблиц и других расширений
                    .Build();

                string htmlContent = Markdig.Markdown.ToHtml(markdownText, pipeline);

                // 3. Добавляем базовые стили
                string fullHtml = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                body {{
                    font-family: 'Segoe UI', Arial, sans-serif;
                    font-size: 20px;               /* Основной размер шрифта */
                    line-height: 1.6;
                    padding: 20px;
                    color: #333;
                    margin: 0;
                }}
                h1, h2, h3 {{
                    color: #002137;
                    margin-top: 1em;
                    margin-bottom: 0.5em;
                }}
                table {{
                    border-collapse: collapse;
                    width: 100%;
                    margin: 1em 0;
                }}
                th, td {{
                    border: 1px solid #ddd;
                    padding: 8px 12px;
                    text-align: left;
                }}
                th {{
                    background-color: #f2f2f2;
                    font-weight: bold;
                }}
                tr:nth-child(even) {{
                    background-color: #f9f9f9;
                }}
                code {{
                    background: #f4f4f4;
                    padding: 2px 4px;
                    border-radius: 3px;
                    font-family: Consolas, monospace;
                }}
                a {{
                    color: #0066cc;
                    text-decoration: none;
                }}
                a:hover {{
                    text-decoration: underline;
                }}
            </style>
        </head>
        <body>
            {htmlContent}
        </body>
        </html>";

                // 4. Создаем и настраиваем форму
                var helpForm = new Form
                {
                    Width = 850,
                    Height = 650,
                    Text = "Справка DocFinder",
                    StartPosition = FormStartPosition.CenterParent,
                    Icon = Properties.Resources.icon_app  // Сохраняем иконку приложения
                };

                var webBrowser = new WebBrowser
                {
                    Dock = DockStyle.Fill,
                    AllowNavigation = false,  // Запрещаем переход по ссылкам
                    DocumentText = fullHtml
                };

                // Обработчик для безопасного открытия ссылок
                webBrowser.DocumentCompleted += (s, ev) =>
                {
                    foreach (HtmlElement link in webBrowser.Document.Links)
                    {
                        link.Click += (sndr, evt) =>
                        {
                            string href = link.GetAttribute("href");
                            if (href.StartsWith("mailto:") || href.StartsWith("tel:"))
                            {
                                System.Diagnostics.Process.Start(href);
                            }
                            evt.ReturnValue = false;  // Отменяем стандартное поведение
                        };
                    }
                };

                helpForm.Controls.Add(webBrowser);
                helpForm.Show(this);  // Показываем как модальное окно
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии справки:\n{ex.Message}",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}

