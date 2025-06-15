using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolDocManager
{
    public partial class ListFilesTable : UserControl
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
            // Добавьте другие расширения и иконки по мере необходимости
        };

        public ListFilesTable(MainForm mainForm, string initialFolderPath = null)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.currentFolderPath = initialFolderPath ?? GetRootFolder(); // По умолчанию корневая папка
            _searchCancellationTokenSource = new CancellationTokenSource();

            // Отображаем текущую папку
            currentFolderLabel.Text = currentFolderPath;

            // Настройка ListView
            InitializeListView();

            // Загружаем данные
            LoadData();

            // Обновляем видимость кнопки "Назад"
            UpdateBackButtonVisibility();
        }

        private void InitializeListView()
        {
            // Создаем ImageList
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(32, 32); // Размер иконок

            // Добавляем иконки в ImageList
            imageList.Images.Add("folder", Properties.Resources.folder); // Иконка для папки
            imageList.Images.Add("unknown", Properties.Resources.unknown_file_icon); // Иконка по умолчанию
            foreach (var icon in FileIcons)
            {
                imageList.Images.Add(icon.Key, icon.Value); // Добавляем иконки для файлов
            }

            // Привязываем ImageList к ListView
            listView.SmallImageList = imageList;
        }

        private void ChangeDisplayButton_Click(object sender, EventArgs e)
        {
            // Переключаемся обратно в ListFilesPanel
            mainForm.SelectUserControl(new ListFilesPanel(mainForm, currentFolderPath));
        }

        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            var listView = sender as System.Windows.Forms.ListView;
            if (listView != null && listView.SelectedItems.Count > 0)
            {
                var selectedItem = listView.SelectedItems[0];
                if (selectedItem.Tag is Folder folder)
                {
                    // Переход в папку
                    currentFolderPath = folder.Path;
                    currentFolderLabel.Text = currentFolderPath; // Обновляем текст
                    LoadData();
                }
                else if (selectedItem.Tag is FileData file)
                {
                    // Открытие файла
                    OpenFile(Path.Combine(currentFolderPath, file.FileName));
                }
            }
        }

        // Метод для обновления видимости кнопки "Назад"
        private void UpdateBackButtonVisibility()
        {
            // Получаем корневую папку
            string rootFolder = GetRootFolder();

            // Если текущая папка — корневая, скрываем кнопку "Назад"
            if (currentFolderPath == rootFolder)
            {
                BackButton.Visible = false;
            }
            else
            {
                BackButton.Visible = true;
            }
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
                currentFolderLabel.Text = currentFolderPath; // Обновляем текст
                LoadData(); // Загружаем данные для новой папки
            }
        }

        private long CalculateFolderSize(string folderPath)
        {
            long totalSize = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["FilesDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Получаем Id папки
                int folderId = GetFolderIdByPath(folderPath);

                if (folderId != -1)
                {
                    // Получаем размер всех файлов в папке
                    string query = "SELECT SUM(FileSize) FROM Files WHERE FolderId = @FolderId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FolderId", folderId);
                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            totalSize = Convert.ToInt64(result);
                        }
                    }
                }
            }

            return totalSize;
        }

        private async void LoadData()
        {
            // Отменяем предыдущую загрузку, если она есть
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                listView.Items.Clear();

                // Получаем список дочерних папок и файлов из базы данных (асинхронно)
                var (folders, files) = await Task.Run(() => GetFoldersAndFilesFromDatabase(currentFolderPath), _cancellationTokenSource.Token);

                // Проверяем, не была ли отменена загрузка
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                // Добавляем папки
                foreach (var folder in folders)
                {
                    long folderSize = CalculateFolderSize(folder.Path); // Вычисляем размер папки
                    var item = new ListViewItem(System.IO.Path.GetFileName(folder.Path), 0); // Иконка папки (индекс 0)
                    item.SubItems.Add("Папка");
                    item.SubItems.Add(FormatFileSize(folderSize)); // Отображаем размер папки
                    item.SubItems.Add("");
                    item.Tag = folder; // Сохраняем объект папки в Tag
                    listView.Items.Add(item);
                }

                // Добавляем файлы
                foreach (var file in files)
                {
                    string extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                    int iconIndex = listView.SmallImageList.Images.IndexOfKey(extension); // Получаем индекс иконки
                    if (iconIndex == -1) // Если иконка не найдена, используем иконку по умолчанию
                    {
                        iconIndex = listView.SmallImageList.Images.IndexOfKey("unknown");
                    }

                    var item = new ListViewItem(file.FileName, iconIndex); // Используем индекс иконки
                    item.SubItems.Add("Файл");
                    item.SubItems.Add(FormatFileSize(file.FileSize));
                    item.SubItems.Add(file.CreationDate.ToShortDateString());
                    item.Tag = file; // Сохраняем объект файла в Tag
                    listView.Items.Add(item);
                }

                // Обновляем видимость кнопки "Назад"
                UpdateBackButtonVisibility();
            }
            catch (OperationCanceledException)
            {
                // Загрузка была отменена, ничего не делаем
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetRootFolder()
        {
            // Реализация метода (аналогично ListFilesPanel)
            string connectionString = ConfigurationManager.ConnectionStrings["FilesDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TOP 1 FolderPath FROM Folders ORDER BY LEN(FolderPath) ASC";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    return command.ExecuteScalar() as string;
                }
            }
        }

        private (List<Folder> folders, List<FileData> files) GetFoldersAndFilesFromDatabase(string parentFolderPath)
        {
            // Реализация метода (аналогично ListFilesPanel)
            List<Folder> folders = new List<Folder>();
            List<FileData> files = new List<FileData>();

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
                      AND LEN(FolderPath) - LEN(REPLACE(FolderPath, '\', '')) = LEN(@ParentPath) - LEN(REPLACE(@ParentPath, '\', '')) + 1";
                using (SqlCommand foldersCommand = new SqlCommand(foldersQuery, connection))
                {
                    foldersCommand.Parameters.AddWithValue("@ParentPath", parentFolderPath);

                    using (SqlDataReader foldersReader = foldersCommand.ExecuteReader())
                    {
                        while (foldersReader.Read())
                        {
                            Folder folder = new Folder
                            {
                                Id = foldersReader.GetInt32(0),
                                Path = foldersReader.GetString(1)
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
                    string filesQuery = "SELECT Id, FileName, CreationDate, FileSize FROM Files WHERE FolderId = @FolderId";
                    using (SqlCommand filesCommand = new SqlCommand(filesQuery, connection))
                    {
                        filesCommand.Parameters.AddWithValue("@FolderId", currentFolderId);

                        using (SqlDataReader filesReader = filesCommand.ExecuteReader())
                        {
                            while (filesReader.Read())
                            {
                                FileData file = new FileData
                                {
                                    Id = filesReader.GetInt32(0),
                                    FileName = filesReader.GetString(1),
                                    CreationDate = filesReader.GetDateTime(2),
                                    FileSize = filesReader.GetInt64(3)
                                };
                                files.Add(file);
                            }
                        }
                    }
                }
            }

            return (folders, files);
        }

        private int GetFolderIdByPath(string folderPath)
        {
            // Реализация метода (аналогично ListFilesPanel)
            string connectionString = ConfigurationManager.ConnectionStrings["FilesDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT IdForder FROM Folders WHERE FolderPath = @FolderPath";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FolderPath", folderPath);

                    object result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }

        private string FormatFileSize(long fileSize)
        {
            // Реализация метода (аналогично ListFilesPanel)
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (fileSize >= 1024 && order < sizes.Length - 1)
            {
                order++;
                fileSize = fileSize / 1024;
            }
            return $"{fileSize:0.##} {sizes[order]}";
        }

        private void OpenFile(string fullPath)
        {
            try
            {
                if (File.Exists(fullPath))
                {
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

        // Диалог выбора элемента (файла или папки)
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

        // Диалог для ввода нового имени
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

        // Переименование элемента (файла или папки)
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

        // Обработчик кнопки "Переименовать"
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
                    LoadData();
                }
            }
        }

        // Удаление элемента (файла или папки)
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

        // Обработчик кнопки "Удалить"
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
                    LoadData();
                }
            }
        }

        private void CancelMenu_Click(object sender, EventArgs e)
        {
            mainForm.SelectUserControl(new MainMenu(mainForm));
        }

        private void ListFilesTable_Load(object sender, EventArgs e)
        {
            // Загружаем данные
            //LoadData();

            originalFormWidth = this.Width;
            originalFormHeight = this.Height;

            foreach (Control ctrl in this.Controls)
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
    }
}