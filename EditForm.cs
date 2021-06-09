using System;
using System.Windows.Forms;

namespace PassStore
{
    public partial class EditForm : Form //Форма редактирования/добавления аккаунта
    {
        private bool toEdit = true; //По умолчанию мы редактируем аккаунт
        private int rowInd; //индекс поля из таблицы, которое редактируем

        public EditForm() => InitializeComponent();

        public EditForm(bool toEdit) : this() // Конструктор для добавления
        {
            if (!toEdit)
            {
                this.toEdit = false;
                Text = "Добавление";
                label1.Text = "Добавление поля";
            }
        }

        public EditForm(int rowInd) : this() // Конструктор для редактирования, передаем индекс поля
        {
            this.rowInd = rowInd;

            textBox1.Text = Program.fields[rowInd].Login;
            textBox2.Text = Program.fields[rowInd].Password;
            textBox3.Text = Program.fields[rowInd].Comment;
        }

		//Метод, вызываемый при нажатии на кнопку формы
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 0 && textBox2.TextLength > 0)
            {
                Form1 baseForm = Owner as Form1;
                if (!toEdit) // либо добавляем либо изменяем поле
                    baseForm.addRow(textBox1.Text, textBox2.Text, textBox3.Text);
                else
                    baseForm.editRow(rowInd, textBox1.Text, textBox2.Text, textBox3.Text);

                Close();
            }
        }
    }
}
