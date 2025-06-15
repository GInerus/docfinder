using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolDocManager
{
    public partial class ListFilesPanel : UserControl
    {
        private MainForm mainForm;
        private string currentFolderPath; // Текущая папка
        private CancellationTokenSource _cancellationTokenSource; // Для отмены загрузки
        private CancellationTokenSource _searchCancellationTokenSource; // Для отмены поиска
        private Dictionary<Control, Rectangle> originalSizes = new Dictionary<Control, Rectangle>();
        private Dictionary<Control, float> originalFontSizes = new Dictionary<Control, float>();
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


            //{ ".doc", Properties.Resources.doc_icon },
            //{ ".pdf", Properties.Resources.pdf_icon },
            //{ ".mp4", Properties.Resources.mp4_icon },
            // Добавьте другие расширения и иконки по мере необходимости
        };

        public ListFilesPanel(MainForm mainForm, string initialFolderPath = null)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.currentFolderPath = initialFolderPath ?? GetRootFolder(); // По умолчанию корневая папка
            _searchCancellationTokenSource = new CancellationTokenSource();

            // Настройка текстового поля для поиска
            searchTextBox.Text = "Поиск документов или папки";
            searchTextBox.ForeColor = Color.Gray;


            // Скрываем кнопку "Назад" при запуске, если текущая папка — корневая
            BackButton.Visible = (currentFolderPath != GetRootFolder());
        }

        // Метод для получения корневой папки
        private string GetRootFolder()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FilesDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1 FolderPath FROM Folders ORDER BY LEN(FolderPath) ASC"; // Корневая папка — самая короткая
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    return command.ExecuteScalar() as string;
                }
            }
        }

        // Метод для загрузки папок и файлов
        private async void LoadFolders()
        {
            // Отменяем предыдущую загрузку, если она есть
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Очищаем текущие плитки
                flowLayoutPanel1.Controls.Clear();

                // Управляем видимостью кнопки "Назад"
                BackButton.Visible = (currentFolderPath != GetRootFolder());

                // Получаем токен отмены
                var cancellationToken = _cancellationTokenSource.Token;

                // Получаем список дочерних папок и файлов из базы данных (асинхронно)
                var (folders, files) = await Task.Run(() => GetFoldersAndFilesFromDatabase(currentFolderPath), cancellationToken);

                // Проверяем, не была ли отменена загрузка
                cancellationToken.ThrowIfCancellationRequested();

                // Создаем плитки для каждой папки
                foreach (Folder folder in folders)
                {
                    // Проверяем, не была ли отменена загрузка
                    cancellationToken.ThrowIfCancellationRequested();

                    // Создаем плитку
                    Panel tile = CreateTile(folder, Properties.Resources.folder);

                    // Добавляем обработчик клика по плитке
                    tile.Click += (sender, e) => OnFolderClick(folder.Path);

                    // Добавляем обработчик клика ко всем внутренним элементам
                    foreach (Control control in tile.Controls)
                    {
                        control.Click += (sender, e) => OnFolderClick(folder.Path);
                    }

                    // Добавляем плитку в FlowLayoutPanel
                    flowLayoutPanel1.Controls.Add(tile);

                    // Обновляем интерфейс
                    flowLayoutPanel1.Update();

                    // Добавляем небольшую задержку для плавного появления
                    await Task.Delay(50, cancellationToken); // Задержка в 50 мс
                }

                // Создаем элементы для отображения файлов
                foreach (FileData file in files)
                {
                    // Проверяем, не была ли отменена загрузка
                    cancellationToken.ThrowIfCancellationRequested();

                    // Создаем элемент для файла
                    Panel filePanel = CreateFilePanel(file);

                    // Добавляем элемент в FlowLayoutPanel
                    flowLayoutPanel1.Controls.Add(filePanel);

                    // Обновляем интерфейс
                    flowLayoutPanel1.Update();

                    // Добавляем небольшую задержку для плавного появления
                    await Task.Delay(50, cancellationToken); // Задержка в 50 мс
                }
            }
            catch (OperationCanceledException)
            {
                // Загрузка была отменена, ничего не делаем
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке папок и файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик клика по плитке
        private void OnFolderClick(string folderPath)
        {
            // Обновляем текущую папку
            currentFolderPath = folderPath;

            // Загружаем папки и файлы для новой текущей папки
            LoadFolders();
        }

        // Метод для получения дочерних папок и файлов
        private (List<Folder> folders, List<FileData> files) GetFoldersAndFilesFromDatabase(string parentFolderPath)
        {
            currentFolderLabel.Text = parentFolderPath;

            List<Folder> folders = new List<Folder>();
            List<FileData> files = new List<FileData>();

            // Получаем строку подключения из конфигурации
            string connectionString = ConfigurationManager.ConnectionStrings["FilesDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Получаем дочерние папки
                string foldersQuery = @"
                    SELECT IdForder, FolderPath 
                    FROM Folders 
                    WHERE FolderPath LIKE @ParentPath + '%' 
                      AND FolderPath <> @ParentPath
                      AND LEN(FolderPath) - LEN(REPLACE(FolderPath, '\', '')) = LEN(@ParentPath) - LEN(REPLACE(@ParentPath, '\', '')) + 1"; // Только дочерние папки
                using (SqlCommand foldersCommand = new SqlCommand(foldersQuery, connection))
                {
                    foldersCommand.Parameters.AddWithValue("@ParentPath", parentFolderPath);

                    using (SqlDataReader foldersReader = foldersCommand.ExecuteReader())
                    {
                        while (foldersReader.Read())
                        {
                            Folder folder = new Folder
                            {
                                Id = foldersReader.GetInt32(0), // IdForder
                                Path = foldersReader.GetString(1) // FolderPath
                            };
                            folders.Add(folder);
                        }
                    }
                }

                // Получаем Id текущей папки
                int currentFolderId = GetFolderIdByPath(parentFolderPath);

                // Если текущая папка существует, загружаем файлы
                if (currentFolderId != -1)
                {
                    string filesQuery = "SELECT Id, FileName, CreationDate, FileSize FROM Files WHERE FolderId = @FolderId"; // SQL-запрос
                    using (SqlCommand filesCommand = new SqlCommand(filesQuery, connection))
                    {
                        filesCommand.Parameters.AddWithValue("@FolderId", currentFolderId);

                        using (SqlDataReader filesReader = filesCommand.ExecuteReader())
                        {
                            while (filesReader.Read())
                            {
                                FileData file = new FileData
                                {
                                    Id = filesReader.GetInt32(0), // Id
                                    FileName = filesReader.GetString(1), // FileName
                                    CreationDate = filesReader.GetDateTime(2), // CreationDate
                                    FileSize = filesReader.GetInt64(3) // FileSize
                                };
                                files.Add(file);
                            }
                        }
                    }
                }
            }

            return (folders, files);
        }

        // Метод для получения информации о файлах в папке
        private (int FileCount, long TotalSize) GetFolderFileInfo(int folderId)
        {
            int fileCount = 0;
            long totalSize = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["FilesDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(Id), SUM(FileSize) FROM Files WHERE FolderId = @FolderId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FolderId", folderId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            fileCount = reader.GetInt32(0); // Количество файлов
                            if (!reader.IsDBNull(1)) // Проверяем, не null ли сумма
                            {
                                totalSize = reader.GetInt64(1); // Общий размер файлов
                            }
                        }
                    }
                }
            }

            return (fileCount, totalSize);
        }

        // Метод для получения Id папки по её пути
        private int GetFolderIdByPath(string folderPath)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FilesDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT IdForder FROM Folders WHERE FolderPath = @FolderPath"; // SQL-запрос
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FolderPath", folderPath);

                    object result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1; // Возвращаем Id или -1, если папка не найдена
                }
            }
        }

        // Метод для создания плитки
        int PanelWidth = 195; // ширина плитки
        private Panel CreateTile(Folder folder, Image image)
        {
            // Создаем панель для плитки
            Panel tilePanel = new Panel();
            tilePanel.Size = new Size(PanelWidth, 140); // Размер плитки
            tilePanel.BackColor = Color.LightGray;
            tilePanel.BorderStyle = BorderStyle.FixedSingle;

            // Добавляем PictureBox для изображения
            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = image;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            int PictureWidth = 100;
            pictureBox.Size = new Size(PictureWidth, 80);
            int PicturePositX = ((PanelWidth / 2) - (PictureWidth / 2));
            pictureBox.Location = new Point(PicturePositX, 20);
            tilePanel.Controls.Add(pictureBox);

            // Добавляем Label для заголовка (название папки)
            Label titleLabel = new Label();
            titleLabel.Text = System.IO.Path.GetFileName(folder.Path); // Имя папки из пути
            titleLabel.Font = new Font("Arial", 8, FontStyle.Bold);
            titleLabel.AutoSize = true;
            Size textSize = TextRenderer.MeasureText(titleLabel.Text, titleLabel.Font); // Измерьте ширину текста
            int LabelPositX = ((PanelWidth / 2) - (textSize.Width / 2));
            titleLabel.Location = new Point(LabelPositX, 0);
            tilePanel.Controls.Add(titleLabel);

            // Получаем информацию о файлах в папке
            var (fileCount, totalSize) = GetFolderFileInfo(folder.Id);

            // Добавляем Label для отображения количества файлов и их общего размера
            Label infoLabel = new Label();
            infoLabel.Text = $"Файлов: {fileCount}\nРазмер: {FormatFileSize(totalSize)}"; // Количество файлов и их размер
            infoLabel.Font = new Font("Arial", 7); // Уменьшаем шрифт для экономии места
            infoLabel.AutoSize = false; // Отключаем AutoSize
            infoLabel.MaximumSize = new Size(PanelWidth - 10, 40); // Максимальная ширина с отступами
            infoLabel.Location = new Point(5, 110); // Позиция под изображением
            infoLabel.TextAlign = ContentAlignment.TopLeft; // Выравнивание текста
            tilePanel.Controls.Add(infoLabel);

            return tilePanel;
        }

        // Метод для создания панели файла
        private Panel CreateFilePanel(FileData file)
        {
            // Создаем панель для файла
            Panel filePanel = new Panel();
            filePanel.Size = new Size(PanelWidth, 140); // Размер панели
            filePanel.BackColor = Color.LightGray; // Цвет фона, как у папок
            filePanel.BorderStyle = BorderStyle.FixedSingle;

            // Добавляем PictureBox для изображения
            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = GetFileIcon(file.FileName); // Получаем иконку для файла
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            int PictureWidth = 100;
            pictureBox.Size = new Size(PictureWidth, 80);
            int PicturePositX = ((PanelWidth / 2) - (PictureWidth / 2));
            pictureBox.Location = new Point(PicturePositX, 20);
            filePanel.Controls.Add(pictureBox);

            // Добавляем Label для имени файла
            Label fileNameLabel = new Label();
            fileNameLabel.Text = file.FileName; // Имя файла
            fileNameLabel.Font = new Font("Arial", 8, FontStyle.Bold);
            fileNameLabel.AutoSize = true;
            Size textSize = TextRenderer.MeasureText(fileNameLabel.Text, fileNameLabel.Font); // Измеряем ширину текста
            int LabelPositX = ((PanelWidth / 2) - (textSize.Width / 2));
            fileNameLabel.Location = new Point(LabelPositX, 0); // Позиция над иконкой
            filePanel.Controls.Add(fileNameLabel);

            // Добавляем Label для дополнительной информации (размер файла и дата создания)
            Label infoLabel = new Label();
            infoLabel.Text = $"{FormatFileSize(file.FileSize)}\n{file.CreationDate.ToShortDateString()}"; // Размер файла и дата создания
            infoLabel.Font = new Font("Arial", 7); // Уменьшаем шрифт для экономии места
            infoLabel.AutoSize = false; // Отключаем автоматическое растягивание
            infoLabel.Size = new Size(180, 40); // Фиксированная ширина и высота
            infoLabel.Location = new Point(3, 110); // Позиция под изображением
            infoLabel.TextAlign = ContentAlignment.TopLeft; // Выравнивание текста
            infoLabel.AutoEllipsis = false; // Отключаем обрезку текста
            filePanel.Controls.Add(infoLabel);

            // Получаем полный путь к файлу
            string fullPath = Path.Combine(currentFolderPath, file.FileName);

            // Добавляем обработчик клика по панели файла
            filePanel.Click += (sender, e) => OpenFile(fullPath);

            // Добавляем обработчик клика ко всем внутренним элементам
            foreach (Control control in filePanel.Controls)
            {
                control.Click += (sender, e) => OpenFile(fullPath);
            }

            return filePanel;
        }

        // Метод для форматирования размера файла
        private string FormatFileSize(long fileSize)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (fileSize >= 1024 && order < sizes.Length - 1)
            {
                order++;
                fileSize = fileSize / 1024;
            }
            return $"{fileSize:0.##} {sizes[order]}";
        }

        // Метод для получения иконки файла
        private Image GetFileIcon(string fileName)
        {
            // Получаем расширение файла
            string extension = System.IO.Path.GetExtension(fileName).ToLower();

            // Проверяем, есть ли иконка для этого расширения
            if (FileIcons.ContainsKey(extension))
            {
                return FileIcons[extension];
            }

            // Возвращаем иконку "неизвестного файла"
            return Properties.Resources.unknown_file_icon;
        }

        // Обработчик кнопки "Назад"
        private void BackButton_Click(object sender, EventArgs e)
        {
            // Получаем родительскую папку
            string parentFolderPath = System.IO.Path.GetDirectoryName(currentFolderPath);

            // Если родительская папка существует, обновляем текущую папку
            if (!string.IsNullOrEmpty(parentFolderPath))
            {
                currentFolderPath = parentFolderPath;
                LoadFolders();
            }
        }

        private void CancelMenu_Click(object sender, EventArgs e)
        {
            mainForm.SelectUserControl(new MainMenu(mainForm));
        }

        private void ChangeDisplay_Click(object sender, EventArgs e)
        {
            // Передаем текущую папку в ListFilesTable
            mainForm.SelectUserControl(new ListFilesTable(mainForm, currentFolderPath));
        }

        private void ListFilesPanel_Load(object sender, EventArgs e)
        {
            // Загружаем папки и файлы при загрузке панели
            LoadFolders();

            originalFormWidth = this.Width;
            originalFormHeight = this.Height;

            foreach (Control ctrl in this.Controls)
            {
                originalSizes[ctrl] = new Rectangle(ctrl.Location, ctrl.Size);
                originalFontSizes[ctrl] = ctrl.Font.Size; // Запоминаем оригинальный размер шрифта
            }
        }

        // Метод для открытия файла
        private void OpenFile(string fullPath)
        {
            try
            {
                // Проверяем, существует ли файл
                if (File.Exists(fullPath))
                {
                    // Открываем файл с помощью стандартного приложения
                    System.Diagnostics.Process.Start(fullPath);
                }
                else
                {
                    MessageBox.Show("Файл не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchTextBox_Enter(object sender, EventArgs e)
        {
            // Проверяем, равен ли текст подсказке
            if (searchTextBox.Text == "Поиск документов или папки")
            {
                searchTextBox.Text = ""; // Очищаем подсказку
                searchTextBox.ForeColor = Color.Black; // Устанавливаем черный цвет текста
            }
        }


        // Обработчик события TextChanged (изменение текста в TextBox)
        private async void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            // Отменяем предыдущий поиск, если он был
            _searchCancellationTokenSource?.Cancel();
            _searchCancellationTokenSource = new CancellationTokenSource();

            string searchQuery = searchTextBox.Text;

            if (string.IsNullOrWhiteSpace(searchQuery) || searchQuery == "Поиск документов или папки")
            {
                // Если строка поиска пуста, загружаем все файлы и папки
                LoadFolders();
            }
            else
            {
                // Выполняем асинхронный поиск
                try
                {
                    await SearchFilesAndFoldersAsync(searchQuery, _searchCancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // Поиск был отменен, ничего не делаем
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при поиске: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

       
        private async Task SearchFilesAndFoldersAsync(string searchQuery, CancellationToken cancellationToken)
        {
            // Очищаем текущие плитки
            flowLayoutPanel1.Controls.Clear();

            // Получаем список файлов и папок для текущей папки
            var (folders, files) = await Task.Run(() => GetFoldersAndFilesFromDatabase(currentFolderPath), cancellationToken);

            // Фильтруем папки
            foreach (Folder folder in folders)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                if (System.IO.Path.GetFileName(folder.Path).IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Создаем плитку для папки
                    Panel tile = CreateTile(folder, Properties.Resources.folder);
                    flowLayoutPanel1.Controls.Add(tile);
                }
            }

            // Фильтруем файлы
            foreach (FileData file in files)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                if (file.FileName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Создаем плитку для файла
                    Panel filePanel = CreateFilePanel(file);
                    flowLayoutPanel1.Controls.Add(filePanel);
                }
            }
        }

        private void ListFilesPanel_Resize(object sender, EventArgs e)
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

        private (string SelectedPath, bool IsFile) ShowSelectItemDialog()
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Выберите файл или папку";
                dialog.Size = new Size(400, 300); // Увеличиваем размер окна
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;

                var listBox = new ListBox();
                listBox.Dock = DockStyle.Fill;
                listBox.Font = new Font("Arial", 16); // Увеличиваем шрифт
                listBox.DoubleClick += (sender, e) => dialog.DialogResult = DialogResult.OK; // Двойной клик для выбора

                // Загружаем файлы и папки текущей папки
                var (folders, files) = GetFoldersAndFilesFromDatabase(currentFolderPath);

                // Добавляем папки
                foreach (var folder in folders)
                {
                    listBox.Items.Add($"[Папка] {System.IO.Path.GetFileName(folder.Path)}");
                }

                // Добавляем файлы
                foreach (var file in files)
                {
                    listBox.Items.Add($"[Файл] {file.FileName}");
                }

                var okButton = new Button();
                okButton.Text = "OK";
                okButton.Font = new Font("Arial", 16); // Увеличиваем шрифт кнопки
                okButton.Size = new Size(100, 40); // Увеличиваем размер кнопки
                okButton.Dock = DockStyle.Bottom;
                okButton.DialogResult = DialogResult.OK;

                dialog.Controls.Add(listBox);
                dialog.Controls.Add(okButton);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (listBox.SelectedItem != null)
                    {
                        string selectedItem = listBox.SelectedItem.ToString();
                        bool isFile = selectedItem.StartsWith("[Файл]");
                        string name = selectedItem.Substring(selectedItem.IndexOf(' ') + 1);
                        string fullPath = Path.Combine(currentFolderPath, name);

                        return (fullPath, isFile);
                    }
                }

                return (null, false);
            }
        }

        private string ShowRenameDialog(string currentName)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Переименовать";
                dialog.Size = new Size(400, 150); // Увеличиваем размер окна
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;

                var textBox = new TextBox();
                textBox.Text = currentName;
                textBox.Font = new Font("Arial", 16); // Увеличиваем шрифт
                textBox.Dock = DockStyle.Top;

                var okButton = new Button();
                okButton.Text = "OK";
                okButton.Font = new Font("Arial", 16); // Увеличиваем шрифт кнопки
                okButton.Size = new Size(100, 40); // Увеличиваем размер кнопки
                okButton.Dock = DockStyle.Bottom;
                okButton.DialogResult = DialogResult.OK;

                dialog.Controls.Add(textBox);
                dialog.Controls.Add(okButton);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return textBox.Text;
                }

                return null;
            }
        }
        private void RenameItem(string oldPath, string newName, bool isFile)
        {
            try
            {
                string newPath = Path.Combine(Path.GetDirectoryName(oldPath), newName);

                // Переименовываем в файловой системе
                if (isFile)
                {
                    File.Move(oldPath, newPath);
                }
                else
                {
                    Directory.Move(oldPath, newPath);
                }

                // Обновляем базу данных
                string connectionString = ConfigurationManager.ConnectionStrings["FilesDB"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (isFile)
                    {
                        string query = "UPDATE Files SET FileName = @NewName WHERE FolderId = @FolderId AND FileName = @OldName";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@NewName", newName);
                            command.Parameters.AddWithValue("@FolderId", GetFolderIdByPath(Path.GetDirectoryName(oldPath)));
                            command.Parameters.AddWithValue("@OldName", Path.GetFileName(oldPath));
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string query = "UPDATE Folders SET FolderPath = @NewPath WHERE FolderPath = @OldPath";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@NewPath", newPath);
                            command.Parameters.AddWithValue("@OldPath", oldPath);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Переименование выполнено успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при переименовании: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            // Показываем диалог выбора элемента
            var (selectedPath, isFile) = ShowSelectItemDialog();

            if (selectedPath != null)
            {
                // Показываем диалог ввода нового имени
                string newName = ShowRenameDialog(Path.GetFileName(selectedPath));

                if (!string.IsNullOrEmpty(newName))
                {
                    // Переименовываем элемент
                    RenameItem(selectedPath, newName, isFile);

                    // Обновляем интерфейс
                    LoadFolders();
                }
            }
        }

        private void DeleteItem(string path, bool isFile)
        {
            try
            {
                // Удаляем в файловой системе
                if (isFile)
                {
                    File.Delete(path);
                }
                else
                {
                    Directory.Delete(path, true); // true для удаления вложенных файлов и папок
                }

                // Удаляем из базы данных
                string connectionString = ConfigurationManager.ConnectionStrings["FilesDB"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (isFile)
                    {
                        string query = "DELETE FROM Files WHERE FolderId = @FolderId AND FileName = @FileName";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FolderId", GetFolderIdByPath(Path.GetDirectoryName(path)));
                            command.Parameters.AddWithValue("@FileName", Path.GetFileName(path));
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string query = "DELETE FROM Folders WHERE FolderPath = @FolderPath";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FolderPath", path);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Удаление выполнено успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // Показываем диалог выбора элемента
            var (selectedPath, isFile) = ShowSelectItemDialog();

            if (selectedPath != null)
            {
                // Подтверждение удаления
                var result = MessageBox.Show("Вы уверены, что хотите удалить этот элемент?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Удаляем элемент
                    DeleteItem(selectedPath, isFile);

                    // Обновляем интерфейс
                    LoadFolders();
                }
            }
        }
    }

    public class Folder
    {
        public int Id { get; set; }
        public string Path { get; set; }
    }

    public class FileData
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime CreationDate { get; set; }
        public long FileSize { get; set; }
    }
}