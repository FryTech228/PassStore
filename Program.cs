using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace PassStore
{
    static class Program
    {
        private static string dataFolderPath { get; set; } //путь до папки с данными
        public static string dataPath { get; set; } //путь до данных
        private static XmlDocument xmlDoc { get; set; } //объект XML документа (в нем хранятся данные)
        public static List<Field> fields { get; set; } //Список полей, загруженных из файла с данными, объекты класса Field
        public static string hash { get; set; } // хэш для сверки введенного ключа с настоящим
        public static string key { get; set; } // ключ

        [STAThread]
        static void Main()
        {
			//Настройка стилей для главного окна
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			//Создаем список полей данных
            fields = new List<Field>();

			//Получачем путь до папки и до данных
            dataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\PassStore";
            dataPath = dataFolderPath + @"\data.psx";

			//Если не существует такой папки, то создаем
            if (!Directory.Exists(dataFolderPath))
                Directory.CreateDirectory(dataFolderPath);

			//Если не существует такого файла, то создаем и вписываем первоначальный шаблон XML документа
            if (!File.Exists(dataPath))
                File.WriteAllText(dataPath, "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<fields>\n</fields>");
			
			//Пытаемся загрузить файл с данными в объект XML документа
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(dataPath);

                loadXml();
				
				//В конструктор формы передаем значение, если хэш в файле существует, значит УЖЕ был задан какой то ключ
				//Значит будем вызывать форму с вводом ключа для проверки, если хэша нет, то форма с вводом нового ключа
                EnterKeyForm keyForm = new EnterKeyForm(hash.Length != 0 ? EnterKeyForm.mode.CHECK : EnterKeyForm.mode.NEW);
                keyForm.ShowDialog(); //Если смогли, то появится форма с вводом ключа
            }
            catch (XmlException ex) //Если файл не был открыт, то выводим ошибку
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

			//Если получили какой то ключ из формы и он верный (в форме идет сопоставление ключа и хэша)
            if (key != null)
            {
                try //Пытаемся расшифровать пароли из файла с данными и выводим основную форму приложения со списком всех аккаунтов
                {
                    decryptFields();
                    Application.Run(new Form1());
                }
                catch (Exception ex) // Если не смогли, ошибка
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

		//Метод для загрузки файла с данными в xml объект
        public static void loadXml()
        {
            int count = 0;
            var root = xmlDoc.DocumentElement;

            hash = root.GetAttribute("hash"); //Получаем хэш из файла

            foreach (XmlNode node in root) //пробегаемся по каждому полю из XML файла
            {
                fields.Add(new Field()); //добавляем в список полей новое поле

                foreach (XmlNode childnode in node.ChildNodes) //Пробегаемся по ПОДполю xml файлу
                {
                    if (childnode.Name == "login") //если password то в переменную объекта поля записываем текст логина
                        fields[count].Login = childnode.InnerText;
                    else if (childnode.Name == "password") //если password то в переменную объекта поля записываем текст пароля
                        fields[count].EnPassword = childnode.InnerText;
                    else if (childnode.Name == "comment") //если comment то в переменную объекта поля записываем текст комментария
                        fields[count].Comment = childnode.InnerText;
                }

                count++;
            }
        }
		
		//Метод для расшифровки паролей
        public static void decryptFields()
        {
			//Пробегаемся по каждому полю и применяем метод decrypt из скачанного класса RC4, передаем зашифрованный пароль и ключ
            foreach (var f in fields)
                f.Password = RC4.Decrypt(f.EnPassword, key);
        }

		//Метод для повторного шифрования если ключ был изменен
        public static void reEncryptFields()
        {
            foreach (var f in fields)
                f.EnPassword = RC4.Encrypt(f.Password, key);
        }
        
		//Метод для сохранения xml файла после любых изменений (взяла шаблон из интернета)
        public static void saveXml()
        {
            File.WriteAllText(dataPath, $"<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<fields hash=\"{hash}\">\n</fields>");
            xmlDoc.Load(dataPath);

            var root = xmlDoc.DocumentElement;

            foreach (var f in fields)
            {
                XmlElement xField = xmlDoc.CreateElement("field");
                XmlElement xLogin = xmlDoc.CreateElement("login");
                XmlElement xPassword = xmlDoc.CreateElement("password");
                XmlElement xComment = xmlDoc.CreateElement("comment");

                xLogin.InnerText = f.Login;
                xPassword.InnerText = f.EnPassword;
                xComment.InnerText = f.Comment;

                xField.AppendChild(xLogin);
                xField.AppendChild(xPassword);
                xField.AppendChild(xComment);

                root.AppendChild(xField);
            }

            xmlDoc.Save(dataPath);
        }
    }
}
