using System;
using System.Windows.Forms;

namespace PassStore
{
    public partial class Form1 : Form
    {
		//Инициализация формы и вывод всех данных в таблицу
        public Form1()
        {
            InitializeComponent();

            foreach (var f in Program.fields)
                dataGridView1.Rows.Add(f.Login, f.Password, f.Comment);
        }

		//Метод для добавления нового аккаунта
        public void addRow(string login, string password, string comment)
        {
            Program.fields.Add(new Field(login, password, comment) { EnPassword = RC4.Encrypt(password, Program.key) });

            dataGridView1.Rows.Add(login, password, comment);
            dataGridView1.Refresh();
        }

		//Метод для редактирования аккаунта
        public void editRow(int rowInd, string login, string password, string comment)
        {
            Program.fields[rowInd].Login = login;
            Program.fields[rowInd].Password = password;
            Program.fields[rowInd].EnPassword = RC4.Encrypt(password, Program.key);
            Program.fields[rowInd].Comment = comment;

            dataGridView1.Rows.RemoveAt(rowInd);
            dataGridView1.Rows.Insert(rowInd, login, password, comment);
            dataGridView1.Refresh();
        }

		//Метод, который вызывается при нажатии на какое либо место в таблице (кнопки удалить, редактировать)
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex + 1 <= Program.fields.Count) // Удаление
            {
                Program.fields.RemoveAt(e.RowIndex);
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                dataGridView1.Refresh();
            }

            if (e.ColumnIndex == 3 && e.RowIndex + 1 <= Program.fields.Count) // Редактирование
            {
                EditForm eForm = new EditForm(e.RowIndex);
                eForm.ShowDialog(this);
            }
        }

		//Метод, который вызывается при нажатии на кнопку изменить ключ
        private void button1_Click(object sender, EventArgs e)
        {
            EnterKeyForm keyForm = new EnterKeyForm(EnterKeyForm.mode.CHANGE);
            keyForm.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e) //  Добавить данные
        {
            EditForm eForm = new EditForm(false);
            eForm.ShowDialog(this);
        }

        private void button3_Click(object sender, EventArgs e) => Program.saveXml(); // Сохранить изменения
    }
}
