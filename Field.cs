using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassStore
{
    class Field //Класс, необходимый для хранения каждого аккаунта (каждый объект этого класса содержит в себе логин, расшифрованный пароль (после дешифровки), зашифрованный пароль и комментарий)
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string EnPassword { get; set; }
        public string Comment { get; set; }

		//Конструктор класса
        public Field(string Login, string Password, string Comment)
        {
            this.Login = Login;
            this.Password = Password;
            this.Comment = Comment;
        }
		
		//Пустой конструктор (все переменные будут пустыми)
        public Field(){ }
    }
}
