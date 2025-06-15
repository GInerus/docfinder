using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SchoolDocManager
{
    class DirectoryScanner
    {
        // Строка подключения к базе данных
        private readonly string _connectionString;

        public DirectoryScanner(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveFilesToDatabase(string folderPath, int folderId)
        {
            List<FileInfo> files = GetFilesInFolder(folderPath);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (FileInfo fileInfo in files)
                {
                    // Проверяем данные перед вставкой
                    if (fileInfo == null || string.IsNullOrEmpty(fileInfo.Name))
                    {
                        MessageBox.Show($"Ошибка: файл в папке {folderPath} имеет некорректное имя.", "Ошибка");
                        continue;
                    }

                    string insertQuery = @"
                INSERT INTO Files (FileName, CreationDate, FileSize, FolderId)
                VALUES (@FileName, @CreationDate, @FileSize, @FolderId)";

                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@FileName", fileInfo.Name);
                        insertCommand.Parameters.AddWithValue("@CreationDate", fileInfo.CreationTime);
                        insertCommand.Parameters.AddWithValue("@FileSize", fileInfo.Length);
                        insertCommand.Parameters.AddWithValue("@FolderId", folderId);

                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }


        // Метод для рекурсивного сканирования папок
        public List<string> ScanFolders(string path)
        {
            List<string> folders = new List<string>();

            try
            {
                // Добавляем текущую папку
                folders.Add(path);

                // Рекурсивно сканируем все поддиректории
                foreach (string directory in Directory.GetDirectories(path))
                {
                    folders.AddRange(ScanFolders(directory));
                }
            }
            catch (Exception ex)
            {
                // В случае ошибки выводим сообщение
                MessageBox.Show($"Ошибка при сканировании папок: {ex.Message}");
            }

            return folders;
        }

        //получить файлы из папок
        public List<FileInfo> GetFilesInFolder(string folderPath)
        {
            List<FileInfo> files = new List<FileInfo>();

            try
            {
                foreach (string filePath in Directory.GetFiles(folderPath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    files.Add(fileInfo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении файлов из папки {folderPath}: {ex.Message}", "Ошибка");
            }

            return files;
        }

        // Метод для записи папок в базу данных
        public void SaveFoldersToDatabase(string path)
        {
            List<string> folders = ScanFolders(path);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    // Удаляем все данные из таблицы
                    string deleteQuery = "DELETE FROM Folders";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.ExecuteNonQuery();
                    }

                    // Сбрасываем автоинкремент (ID) к начальному значению
                    string resetIdentityQuery = "DBCC CHECKIDENT ('Folders', RESEED, 0)";
                    using (SqlCommand resetIdentityCommand = new SqlCommand(resetIdentityQuery, connection))
                    {
                        resetIdentityCommand.ExecuteNonQuery();
                    }

                    foreach (string folder in folders)
                    {
                        // SQL-запрос для вставки папки в таблицу
                        string query = "INSERT INTO Folders (FolderPath) VALUES (@FolderPath)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@FolderPath", folder);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при записи в базу данных: {ex.Message}", "Ошибка");
                }
            }
        }

        public void SaveFoldersAndFilesToDatabase(string rootPath)
        {
            List<string> folders = ScanFolders(rootPath);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                connection.Open();

                // Удаляем старые данные из таблиц
                string deleteFoldersQuery = "DELETE FROM Folders";
                string deleteFilesQuery = "DELETE FROM Files";
                using (SqlCommand deleteFoldersCommand = new SqlCommand(deleteFoldersQuery, connection))
                using (SqlCommand deleteFilesCommand = new SqlCommand(deleteFilesQuery, connection))
                {
                    deleteFoldersCommand.ExecuteNonQuery();
                    deleteFilesCommand.ExecuteNonQuery();
                }

                // Сбрасываем автоинкремент
                string resetFoldersIdentityQuery = "DBCC CHECKIDENT ('Folders', RESEED, 0)";
                string resetFilesIdentityQuery = "DBCC CHECKIDENT ('Files', RESEED, 0)";
                using (SqlCommand resetFoldersCommand = new SqlCommand(resetFoldersIdentityQuery, connection))
                using (SqlCommand resetFilesCommand = new SqlCommand(resetFilesIdentityQuery, connection))
                {
                    resetFoldersCommand.ExecuteNonQuery();
                    resetFilesCommand.ExecuteNonQuery();
                }

                // Добавляем папки и файлы
                foreach (string folderPath in folders)
                {
                    // Добавляем папку в таблицу Folders
                    string insertFolderQuery = "INSERT INTO Folders (FolderPath) VALUES (@FolderPath); SELECT SCOPE_IDENTITY();";
                    int folderId;

                    using (SqlCommand insertFolderCommand = new SqlCommand(insertFolderQuery, connection))
                    {
                        insertFolderCommand.Parameters.AddWithValue("@FolderPath", folderPath);
                        folderId = Convert.ToInt32(insertFolderCommand.ExecuteScalar()); // Получаем Id добавленной папки
                    }

                    // Добавляем файлы из этой папки в таблицу Files
                    SaveFilesToDatabase(folderPath, folderId);
                }
            }
        }
    }
}