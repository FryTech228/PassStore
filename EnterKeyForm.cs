using System;
using System.IO;
using System.Windows.Forms;

namespace PassStore
{
    public partial class EnterKeyForm : Form // Подформа для ввода ключа
    {
		//конструкция для определения, какой вид должен быть у формы
        public enum mode
        {
            NEW = 0, // для ввода нового ключа
            CHECK = 1, // для ввода ключа на проверку (при попытке запустить программу с уже имеющимся ключом)
            CHANGE = 2 // для изменения ключа
        }

		//Здесь храним текущий вид формы
        private mode curMode;

        public EnterKeyForm() => InitializeComponent();

		//Конструктор формы, если ничего не передали в аргумент, то по умолчанию считаем, что у нас режим CHECK
        public EnterKeyForm(mode m = mode.CHECK) : this()
        {
            curMode = m;

            if (curMode == mode.NEW) // если режим new
                label1.Text = "Придумайте ключ дешифрования:";
            else if (curMode == mode.CHANGE) // если режим change
                label1.Text = "Введите новый ключ дешифрования:";
        }

		//При нажатии на кнопку на форме
        private void button1_Click(object sender, EventArgs e)
        {
            if (curMode == mode.CHECK) //Проверяем введенный пароль с хэшем
            {
				//Способ таков: шифруем любую строку, например "2360da5a9341f" (можно было делать любую) используя введенный пользователем ключ
				//Если полученная зашифрованная строка совпадает с той, что хранилась в поле хэш в файле XML, то пароль верный
                if (textBox1.TextLength != 0 && RC4.Encrypt("2360da5a9341f", textBox1.Text) == Program.hash)
                {
                    Program.key = textBox1.Text;
                    Close();
                }
                else MessageBox.Show("Введен неверный ключ!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (textBox1.TextLength != 0)
                {
                    Program.key = textBox1.Text;
                    Program.hash = RC4.Encrypt("2360da5a9341f", Program.key); // Шифруем строку "2360da5a9341f" с использованием ключа, сохраняем ее в поле хэш xml файла

                    if (curMode == mode.NEW)
                        File.WriteAllText(Program.dataPath, $"<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<fields hash=\"{Program.hash}\">\n</fields>");
                    else
                    {
                        Program.reEncryptFields();
                        Program.saveXml();
                    }

                    if (MessageBox.Show("Новый ключ успешно назначен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                        Close();
                }
            }
        }
    }
}
