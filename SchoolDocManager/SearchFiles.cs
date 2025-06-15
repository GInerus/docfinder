using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Presentation;
using System.Threading;
using NPOI.HSSF.UserModel; // Для .xls
using NPOI.XSSF.UserModel; // Для .xlsx
using NPOI.SS.UserModel;

namespace SchoolDocManager
{
    public partial class SearchFiles : UserControl
    {
        private MainForm mainForm; // Основная форма
        private Dictionary<System.Windows.Forms.Control, Rectangle> originalSizes = new Dictionary<System.Windows.Forms.Control, Rectangle>();
        private Dictionary<System.Windows.Forms.Control, float> originalFontSizes = new Dictionary<System.Windows.Forms.Control, float>();
        private float originalFormWidth;
        private float originalFormHeight;

        // Словарь для хранения иконок файлов
        private static readonly Dictionary<string, Image> FileIcons = new Dictionary<string, Image>
        {
            { ".txt", Properties.Resources.txt_icon },
            { ".m4a", Properties.Resources.m4a_icon },
            { ".ini", Properties.Resources.ini_icon },
            { ".jpg", Properties.Resources.jpg_icon },
            { ".mp3", Properties.Resources.mp3_icon },
            { ".docx", Properties.Resources.docx_icon },
            { ".zip", Properties.Resources.zip_icon },
            { ".png", Properties.Resources.png_icon },
            { ".xls", Properties.Resources.xls_file },
            { ".xlsx", Properties.Resources.xls_file },
            { ".ppt", Properties.Resources.ppt_icon },
            { ".pptx", Properties.Resources.ppt_icon },
            { ".py", Properties.Resources.py_icon}
            // Добавьте другие расширения и иконки по мере необходимости
        };

        // Конструктор, принимающий MainForm
        public SearchFiles(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm; // Инициализация mainForm

            // Подписываемся на событие DoubleClick для ListBox
            ResultsListBox.DoubleClick += ResultsListBox_DoubleClick;

            // Настройка ListBox для поддержки многострочных элементов
            ResultsListBox.DrawMode = DrawMode.OwnerDrawVariable;
            ResultsListBox.DrawItem += ResultsListBox_DrawItem;
            ResultsListBox.MeasureItem += ResultsListBox_MeasureItem;

            // Увеличиваем размер шрифта
            ResultsListBox.Font = new System.Drawing.Font("Segoe UI", 10);

            // Подписываемся на событие KeyDown для SearchTextBox
            SearchTextBox.KeyDown += SearchTextBox_KeyDown;

            // Загружаем первые 15 файлов при запуске
            LoadInitialFiles();
        }

        // Обработчик события KeyDown для SearchTextBox
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Если нажата клавиша Enter
            if (e.KeyCode == Keys.Enter)
            {
                // Вызываем метод поиска
                SearchButton_Click(sender, e);
            }
        }

        // Загрузка первых 15 файлов при запуске
        private void LoadInitialFiles()
        {
            List<string> filePaths = GetFilePathsFromDatabase().Take(15).ToList();
            DisplayFiles(filePaths);
        }

        // Отображение файлов в ListBox
        private void DisplayFiles(List<string> filePaths)
        {
            ResultsListBox.Items.Clear();
            foreach (string filePath in filePaths)
            {
                ResultsListBox.Items.Add($"Файл: {Path.GetFileName(filePath)}{Environment.NewLine}Путь: {filePath}");
            }
        }

        // Обработчик двойного клика на элементе ListBox
        private void ResultsListBox_DoubleClick(object sender, EventArgs e)
        {
            if (ResultsListBox.SelectedItem != null)
            {
                string selectedItem = ResultsListBox.SelectedItem.ToString();
                string filePath = selectedItem.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[1]
                    .Replace("Путь: ", "");

                try
                {
                    Process.Start(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть файл: {ex.Message}");
                }
            }
        }

        // Обработчик нажатия кнопки "Поиск"
        private void SearchButton_Click(object sender, EventArgs e)
        {
            string searchText = SearchTextBox.Text;
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Введите текст для поиска.");
                return;
            }

            List<string> filePaths = GetFilePathsFromDatabase();
            List<string> results = SearchInFiles(filePaths, searchText);

            ResultsListBox.Items.Clear();
            foreach (string result in results)
            {
                ResultsListBox.Items.Add(result);
            }
        }

        // Метод для получения путей к файлам из базы данных
        private List<string> GetFilePathsFromDatabase()
        {
            List<string> filePaths = new List<string>();

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["FilesDB"].ConnectionString))
            {
                connection.Open();
                string query = @"
                    SELECT f.FolderPath, fl.FileName
                    FROM Folders f
                    JOIN Files fl ON f.IdForder = fl.FolderId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string folderPath = reader["FolderPath"].ToString();
                            string fileName = reader["FileName"].ToString();
                            string fullPath = Path.Combine(folderPath, fileName);
                            filePaths.Add(fullPath);
                        }
                    }
                }
            }

            return filePaths;
        }

        // Метод для поиска текста в файлах
        private List<string> SearchInFiles(List<string> filePaths, string searchText)
        {
            List<string> results = new List<string>();

            foreach (string filePath in filePaths)
            {
                if (File.Exists(filePath))
                {
                    string extension = Path.GetExtension(filePath).ToLower();

                    switch (extension)
                    {
                        case ".txt":
                            results.AddRange(SearchInTextFile(filePath, searchText));
                            break;
                        case ".docx":
                        case ".doc":
                            results.AddRange(SearchInWordFile(filePath, searchText));
                            break;
                        case ".xlsx":
                        case ".xls":
                            results.AddRange(SearchInExcelFile(filePath, searchText));
                            break;
                        case ".pptx":
                        case ".ppt":
                            results.AddRange(SearchInPowerPointFile(filePath, searchText));
                            break;
                        default:
                            // Неподдерживаемый формат
                            break;
                    }
                }
            }

            return results;
        }

        // Поиск в текстовых файлах
        private List<string> SearchInTextFile(string filePath, string searchText)
        {
            List<string> results = new List<string>();

            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    results.Add($"Файл: {Path.GetFileName(filePath)}{Environment.NewLine}Путь: {filePath}{Environment.NewLine}Строка: {i + 1}");
                }
            }

            return results;
        }

        // Поиск в Word
        private List<string> SearchInWordFile(string filePath, string searchText)
        {
            List<string> results = new List<string>();

            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, false))
            {
                var body = doc.MainDocumentPart.Document.Body;
                foreach (var paragraph in body.Elements<Paragraph>())
                {
                    if (paragraph.InnerText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        results.Add($"Файл: {Path.GetFileName(filePath)}{Environment.NewLine}Путь: {filePath}{Environment.NewLine}Текст: {paragraph.InnerText}");
                    }
                }
            }

            return results;
        }

        // Поиск в Excel
        private List<string> SearchInExcelFile(string filePath, string searchText)
        {
            List<string> results = new List<string>();

            try
            {
                string extension = Path.GetExtension(filePath).ToLower();

                if (extension == ".xlsx")
                {
                    // Обработка .xlsx с помощью EPPlus
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        var workbook = package.Workbook;
                        foreach (var worksheet in workbook.Worksheets)
                        {
                            for (int row = 1; row <= worksheet.Dimension.Rows; row++)
                            {
                                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                                {
                                    var cellValue = worksheet.Cells[row, col].Text;
                                    if (cellValue.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        results.Add($"Файл: {Path.GetFileName(filePath)}{Environment.NewLine}Путь: {filePath}{Environment.NewLine}Лист: {worksheet.Name}{Environment.NewLine}Ячейка: {worksheet.Cells[row, col].Address}");
                                    }
                                }
                            }
                        }
                    }
                }
                else if (extension == ".xls")
                {
                    // Обработка .xls с помощью NPOI
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        var workbook = new HSSFWorkbook(stream); // Для .xls
                        for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                        {
                            var sheet = workbook.GetSheetAt(sheetIndex);
                            for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                            {
                                var row = sheet.GetRow(rowIndex);
                                if (row != null)
                                {
                                    for (int colIndex = 0; colIndex < row.LastCellNum; colIndex++)
                                    {
                                        var cell = row.GetCell(colIndex);
                                        if (cell != null)
                                        {
                                            var cellValue = cell.ToString();
                                            if (cellValue.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                                            {
                                                results.Add($"Файл: {Path.GetFileName(filePath)}{Environment.NewLine}Путь: {filePath}{Environment.NewLine}Лист: {sheet.SheetName}{Environment.NewLine}Ячейка: {cell.Address}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем выполнение
                Debug.WriteLine($"Ошибка при обработке файла {filePath}: {ex.Message}");
            }

            return results;
        }

        // Поиск в PowerPoint
        private List<string> SearchInPowerPointFile(string filePath, string searchText)
        {
            List<string> results = new List<string>();

            using (PresentationDocument doc = PresentationDocument.Open(filePath, false))
            {
                var slideIds = doc.PresentationPart.Presentation.SlideIdList.ChildElements;
                int slideIndex = 1;

                foreach (var slideId in slideIds)
                {
                    var slidePart = (SlidePart)doc.PresentationPart.GetPartById(((SlideId)slideId).RelationshipId);

                    foreach (var paragraph in slidePart.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                    {
                        if (paragraph.InnerText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            results.Add($"Файл: {Path.GetFileName(filePath)}{Environment.NewLine}Путь: {filePath}{Environment.NewLine}Слайд: {slideIndex}");
                        }
                    }

                    slideIndex++;
                }
            }

            return results;
        }

        // Отрисовка элементов ListBox
        private void ResultsListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            string itemText = ResultsListBox.Items[e.Index].ToString();
            string[] lines = itemText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // Получаем расширение файла
            string filePath = lines[1].Replace("Путь: ", "");
            string extension = Path.GetExtension(filePath).ToLower();

            // Получаем иконку для расширения
            Image icon = FileIcons.ContainsKey(extension) ? FileIcons[extension] : null;

            // Настраиваем отрисовку
            e.DrawBackground();
            e.DrawFocusRectangle();

            // Рисуем иконку
            if (icon != null)
            {
                e.Graphics.DrawImage(icon, e.Bounds.Left, e.Bounds.Top, 32, 32);
            }

            // Рисуем текст с отступом для иконки
            using (var brush = new SolidBrush(e.ForeColor))
            {
                float y = e.Bounds.Top;
                float x = e.Bounds.Left + 40; // Отступ для иконки

                foreach (var line in lines)
                {
                    e.Graphics.DrawString(line, e.Font, brush, x, y);
                    y += e.Font.GetHeight();
                }
            }

            // Рисуем разделительную линию
            using (var pen = new Pen(System.Drawing.Color.LightGray, 1))
            {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }
        }

        // Расчет высоты элементов ListBox
        private void ResultsListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0) return;

            string itemText = ResultsListBox.Items[e.Index].ToString();
            string[] lines = itemText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            float totalHeight = 0;
            foreach (var line in lines)
            {
                totalHeight += e.Graphics.MeasureString(line, ResultsListBox.Font).Height;
            }

            // Добавляем отступ для иконки
            e.ItemHeight = (int)totalHeight + 10; // +10 для разделительной линии
        }

        private void CancelMenu_Click(object sender, EventArgs e)
        {
            mainForm.SelectUserControl(new MainMenu(mainForm));
        }

        private void ListFilesPanel_Load(object sender, EventArgs e)
        {
            originalFormWidth = this.Width;
            originalFormHeight = this.Height;

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
            {
                originalSizes[ctrl] = new Rectangle(ctrl.Location, ctrl.Size);
                originalFontSizes[ctrl] = ctrl.Font.Size; // Запоминаем оригинальный размер шрифта
            }
        }

        private void ListFilesPanel_Resize(object sender, EventArgs e)
        {
            float scaleWidth = this.Width / originalFormWidth;
            float scaleHeight = this.Height / originalFormHeight;
            float scaleFactor = Math.Min(scaleWidth, scaleHeight); // Сохраняем пропорции

            foreach (System.Windows.Forms.Control ctrl in this.Controls)
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

                    ctrl.Font = new System.Drawing.Font(ctrl.Font.FontFamily, newFontSize, ctrl.Font.Style);
                }
            }
        }
    }
}