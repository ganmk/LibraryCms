﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LibraryCms.Models
{
    public class DAL
    {
        /// <summary>
        /// 检测是否能登陆，若成功登陆则保存登陆信息
        /// </summary>
        /// <param name="account">学号/邮箱/手机号</param>
        /// <param name="password">密码</param>
        public static User CheckLogin(string account, string password)
        {
            string strPassword = MD5.MD5_Encode(password);
            string sql = "select * from tb_User where Number = @value or Mail = @value or Phone = @value";
            SqlDataReader reader = SqlHelper.GetReader(sql, new SqlParameter("@value", account));
            if (reader.HasRows)
            {
                reader.Read();
                string userId = reader["UserID"].ToString();
                if (strPassword == reader["Password"].ToString())
                {
                    User user = GetUserById(userId);
                    return user;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// 读取用户信息
        /// </summary>
        /// <param name="id">UserID</param>
        public static User GetUserById(string id)
        {
            string sql = "select * from tb_User where UserID = @value";
            SqlDataReader reader = SqlHelper.GetReader(sql, new SqlParameter("@value", id));
            if (!reader.HasRows)
            {
                return null;
            }
            reader.Read();
            User user = new User
            {
                UserID = id,
                Password = reader["Password"].ToString(),
                Mail = reader["Mail"].ToString(),
                Name = reader["Name"].ToString(),
                Number = reader["Number"].ToString(),
                Phone = reader["Phone"].ToString(),
                QQ = reader["QQ"].ToString(),
                Sex = reader["Sex"].ToString().Trim(),
            };
            string roleId = reader["RoleID"].ToString();
            reader.Close();
            user.Role = GetRoleById(roleId);

            return user;
        }

        public static List<Department> GetDepartment(string str, bool isEx = false)
        {
            string sql = "select * from tb_Department_B where DepartmentName like @name";
            if(isEx)
            {
                sql = "select * from tb_Department_B where DepartmentName = @name";
            }
            SqlDataReader reader = SqlHelper.GetReader(sql, new SqlParameter("@name", isEx ? str : "%" + str + "%"));
            List<Department> ret = new List<Department>();
            while (reader.Read())
            {
                Department tmpDepartment = new Department
                {
                    DepartmentId = reader["DepartmentID"].ToString(),
                    DepartmentName = reader["DepartmentName"].ToString(),
                    DepartmentType = DepartmentType.B
                };
                ret.Add(tmpDepartment);
            }
            reader.Close();
            sql = "select * from tb_Department_X where DepartmentName like @name";
            if (isEx)
            {
                sql = "select * from tb_Department_X where DepartmentName = @name";
            }
            reader = SqlHelper.GetReader(sql, new SqlParameter("@name", isEx ? str : "%" + str + "%"));
            while (reader.Read())
            {
                Department tmpDepartment = new Department
                {
                    DepartmentId = reader["DepartmentID"].ToString(),
                    DepartmentName = reader["DepartmentName"].ToString(),
                    DepartmentType = DepartmentType.X
                };
                ret.Add(tmpDepartment);
            }
            reader.Close();
            sql = "select * from tb_Department_A where DepartmentName like @name";
            if (isEx)
            {
                sql = "select * from tb_Department_A where DepartmentName = @name";
            }
            reader = SqlHelper.GetReader(sql, new SqlParameter("@name", isEx ? str : "%" + str + "%"));
            while (reader.Read())
            {
                Department tmpDepartment = new Department
                {
                    DepartmentId = reader["DepartmentID"].ToString(),
                    DepartmentName = reader["DepartmentName"].ToString(),
                    DepartmentType = DepartmentType.A
                };
                ret.Add(tmpDepartment);
            }
            reader.Close();
            return ret;
        }

        public static Department GetDepartmentById(string id,string deptTyp)
        {
            string sql = "";
            switch (deptTyp)
            {
                case "X":
                    sql = "select * from tb_Department_X where DepartmentID = @id";
                    break;
                case "B":
                    sql = "select * from tb_Department_B where DepartmentID = @id";
                    break;
                case "A":
                    sql = "select * from tb_Department_A where DepartmentID = @id";
                    break;
            }
            SqlDataReader reader = SqlHelper.GetReader(sql, new SqlParameter("@id", id));
            if (reader.HasRows)
            {
                reader.Read();
                Department tmpDepartment = new Department()
                {
                    DepartmentId = reader["DepartmentID"].ToString(),
                    DepartmentName = reader["DepartmentName"].ToString(),
                };
                switch (deptTyp)
                {
                    case "X":
                        tmpDepartment.DepartmentType = DepartmentType.X;
                        break;
                    case "B":
                        tmpDepartment.DepartmentType = DepartmentType.B;
                        break;
                    case "A":
                        tmpDepartment.DepartmentType = DepartmentType.A;
                        break;
                }
                reader.Close();
                return tmpDepartment;
            }
            reader.Close();
            return null;
        }

        public static List<Role> GetRole(string str, bool isEx)
        {
            string strSearch = "%" + str + "%";
            string sql = "select * from tb_Role where RoleName like @name";
            if(isEx)
            {
                sql = "select * from tb_Role where RoleName = @name";
                strSearch = str;
            }
            SqlDataReader reader = SqlHelper.GetReader(sql, new SqlParameter("@name", strSearch));
            List<string> tmp = new List<string>();
            if(reader.HasRows)
            {
                while (reader.Read())
                {
                    tmp.Add(reader["RoleID"].ToString());
                }
            }
            reader.Close();
            return tmp.Select(GetRoleById).ToList();
        }

        public static User GetUser(string str)
        {
            string sql = "select * from tb_User where Number=@str or Mail=@str or Phone=@str";
            SqlDataReader reader = SqlHelper.GetReader(sql, new SqlParameter("@str", str));
            if (reader.HasRows)
            {
                reader.Read();
                User tmpUser = GetUserById(reader["UserID"].ToString());
                reader.Close();
                return tmpUser;
            }
            reader.Close();
            return null;
        }

        public static Role GetRoleById(string id)
        {
            string sql = "select * from tb_Role where RoleID=@id";
            SqlDataReader reader = SqlHelper.GetReader(sql, new SqlParameter("@id", id));
            if (!reader.HasRows)
            {
                return null;
            }
            reader.Read();
            Role role = new Role
            {
                RoleId = reader["RoleID"].ToString(),
                Rights = reader["Rights"].ToString(),
                RoleName = reader["RoleName"].ToString(),
                Department = GetDepartmentById(reader["DepartmentID"].ToString(), reader["DepartmentType"].ToString())
            };
            reader.Close();
            return role;
        }

        public static List<Book> GetBook(string str)
        {
            string sql = "select * from tb_Book where Author like @str or Publisher like @str or BookName like @str";
            SqlDataReader reader = SqlHelper.GetReader(sql, new SqlParameter("@str", "%" + str + "%"));
            if (!reader.HasRows)
            {
                return null;
            }
            List<string> tmp = new List<string>();
            while (reader.Read())
            {
                tmp.Add(reader["BookID"].ToString());
            }
            reader.Close();
            return tmp.Select(GetBookById).ToList();
        }

        public static Book GetBookById(string id)
        {
            string sql = "select * from tb_Book where BookID=@id";
            SqlDataReader reader = SqlHelper.GetReader(sql, new SqlParameter("@id", id));
            if (!reader.HasRows)
            {
                return null;
            }
            reader.Read();
            Book book = new Book
            {
                BookId = reader["BookID"].ToString(),
                BookName = reader["BookName"].ToString(),
                Author = reader["Author"].ToString(),
                Format = reader["Formart"].ToString(),
                Publisher = reader["Publisher"].ToString(),
                DepartmentId = reader["DepartmentId"].ToString(),
                BookPath = reader["BookPath"].ToString(),
                DownloadNumber = int.Parse(reader["BookID"].ToString()),
                Pages = int.Parse(reader["BookID"].ToString()),
                Point = float.Parse(reader["BookID"].ToString()),
                PublicTime = reader["BookID"].ToString()
            };
            reader.Close();
            return book;
        }

        /// <summary>
        /// 获取推荐书籍
        /// </summary>
        /// <returns></returns>
        public static List<Book> GetRecomendBooks()
        {
            string sql = "select * from tb_Book order by Point DESC";
            SqlDataReader reader = SqlHelper.GetReader(sql);
            if (!reader.HasRows)
            {
                return null;
            }
            List<string> bookId = new List<string>();
            while (reader.Read())
            {
                bookId.Add(reader["BookID"].ToString());
            }
            reader.Close();
            return bookId.Select(GetBookById).ToList();
        }

        /// <summary>
        /// 插入书籍信息
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public static int InsetrBook(Book book)
        {
            string sql = "insert into tb_Book values(@bookName,@author,";
            if(book.Publisher == "")
            {
                sql += "NULL,";
            }
            else
            {
                sql += "@publisher,";
            }
            sql += "@pages,@publicTime,@format,@bookPath,@downloadNumber,@point,@departmentId)";
            SqlParameter[] value = new SqlParameter[]{
                new SqlParameter("@bookName",book.BookName),
                new SqlParameter("@author",book.Author),
                new SqlParameter("@publisher",book.Publisher),
                new SqlParameter("@pages",book.Pages),
                new SqlParameter("@publicTime",book.PublicTime),
                new SqlParameter("@format",book.Format),
                new SqlParameter("@bookPath",book.BookPath),
                new SqlParameter("@downloadNumber",book.DownloadNumber),
                new SqlParameter("@point",book.Point),
                new SqlParameter("@departmentId",book.DepartmentId)
            };
            return SqlHelper.ExecuteCommand(sql, value);
        }

        /// <summary>
        /// 添加系部信息
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public static int InsertDepartment(Department dept)
        {
            string sql = "insert into tb_Department_";
            switch (dept.DepartmentType)
            {
                case DepartmentType.X:
                    sql += "X";
                    break;
                case DepartmentType.B:
                    sql += "B";
                    break;
                case DepartmentType.A:
                    sql += "A";
                    break;
            }
            sql += " values(@departmentName)";
            SqlParameter value = new SqlParameter("@departmentName",dept.DepartmentName);
            return SqlHelper.ExecuteCommand(sql, value);
        }

        public static int DeleteDepartment(Department dept)
        {
            string sql = "delete from tb_Department_";
            switch (dept.DepartmentType)
            {
                case DepartmentType.X:
                    sql += "X";
                    break;
                case DepartmentType.B:
                    sql += "B";
                    break;
                case DepartmentType.A:
                    sql += "A";
                    break;
            }
            sql += " where DepartmentName=@deptName";
            SqlParameter value = new SqlParameter("@deptName", dept.DepartmentName);
            return SqlHelper.ExecuteCommand(sql, value);
        }

        public static int UpdateUserInfo(User user)
        {
            string sql = "UPDATE tb_User SET Password=@pwd,RoleID=@roleId,Name=@name";
            if (user.Mail != "")
            {
                sql += ",Mail=@mail";
            }
            else
            {
                sql += ",Mail=NULL";
            }
            if(user.Phone != "")
            {
                sql += ",Phone=@phone";
            }
            else
            {
                sql += ",Phone=NULL";
            }
            if (user.QQ != "")
            {
                sql += ",QQ=@qq";
            }
            else
            {
                sql += ",QQ=NULL";
            }
            if (user.Sex != "")
            {
                sql += ",Sex=@sex";
            }
            else
            {
                sql += ",Sex=NULL";
            }
            sql += " WHERE UserID=@userId";
            SqlParameter[] value = new SqlParameter[]{
                new SqlParameter("@pwd",user.Password),
                new SqlParameter("@roleId",user.Role.RoleId),
                new SqlParameter("@name",user.Name),
                new SqlParameter("@mail",user.Mail),
                new SqlParameter("@phone",user.Phone),
                new SqlParameter("@qq",user.QQ),
                new SqlParameter("@sex",user.Sex),
                new SqlParameter("@userId",user.UserID)
            };
            return SqlHelper.ExecuteCommand(sql, value);
        }

        #region 私信
        //获取全部私信
        public static List<Message> GetPrivateMessage(String userid)
        {
            string sql = "if (object_id('tb_Message_" + userid + "') is not null) begin exec('select * from tb_Message_" + userid + " order by Time') end";
            SqlDataReader reader = SqlHelper.GetReader(sql);
            if (!reader.HasRows)
            {
                return null;
            }
            List<Message> messages = new List<Message>();
            while (reader.Read())
            {
                Message message = new Message
                {
                    MessageID = int.Parse(reader["MessageID"].ToString()),
                    From = reader["From"].ToString(),
                    Time = DateTime.Parse(reader["Time"].ToString()),
                    Subject = reader["Subject"].ToString(),
                    Content = reader["Content"].ToString(),
                    Status = int.Parse(reader["Status"].ToString())
                };
                messages.Add(message);
            }
            reader.Close();
            return messages;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="userid">收件人ID</param>
        /// <param name="msg">邮件</param>
        /// <returns></returns>
        public static int InsertMessage(String userid, Message msg)
        {
            string sql = "if (object_id('tb_Message_" + userid + "') is null)";
            sql += "BEGIN ";
            sql += "EXEC('";
            sql += "CREATE TABLE tb_Message_" + userid + "(";
            sql += "[MessageID] INT IDENTITY(1,1)NOT NULL PRIMARY KEY,";
            sql += "[From] INT NOT NULL,";
	        sql += "[Time] VARCHAR(20) NOT NULL,";
	        sql += "[Subject] TEXT NOT NULL,";
            sql += "[Content] TEXT NULL,";
	        sql += "[Status] INT NOT NULL,";
	        sql += "FOREIGN KEY([From]) REFERENCES tb_User([UserID])";
            sql += ")";
            sql += "')";
            sql += "END";
            int i = SqlHelper.ExecuteCommand(sql);
            if (i == 0) return i;
            sql = "if (object_id('tb_Message_" + msg.From + "') is null)";
            sql += "BEGIN ";
            sql += "EXEC('";
            sql += "CREATE TABLE tb_Message_" + msg.From + "(";
            sql += "[MessageID] INT IDENTITY(1,1)NOT NULL PRIMARY KEY,";
            sql += "[From] INT NOT NULL,";
            sql += "[Time] VARCHAR(20) NOT NULL,";
            sql += "[Subject] TEXT NOT NULL,";
            sql += "[Content] TEXT NULL,";
            sql += "[Status] INT NOT NULL,";
            sql += "FOREIGN KEY([From]) REFERENCES tb_User([UserID])";
            sql += ")";
            sql += "')";
            sql += "END";
            i = SqlHelper.ExecuteCommand(sql);
            if (i == 0) return i;
            sql = "insert into tb_Message_" + userid + " values(@from, @time, @subject,";
            if(msg.Content == "")
            {
                sql += "NULL,";
            }
            else
            {
                sql += "@content,";
            }
            sql += "@status)";
            SqlParameter[] values = new SqlParameter[] {
                new SqlParameter("@from",msg.From),
                new SqlParameter("@time",msg.Time.ToString()),
                new SqlParameter("@subject",msg.Subject),
                new SqlParameter("@content",msg.Content),
                new SqlParameter("@status",msg.Status)
            };
            i = SqlHelper.ExecuteCommand(sql, values);
            if (i == 0) return i;
            msg.Status = 1;
            sql = "insert into tb_Message_" + msg.From + " values(@from, @time, @subject,";
            if (msg.Content == "")
            {
                sql += "NULL,";
            }
            else
            {
                sql += "@content,";
            }
            sql += "@status)";
            values = new SqlParameter[] {
                new SqlParameter("@from",msg.From),
                new SqlParameter("@time",msg.Time.ToString()),
                new SqlParameter("@subject",msg.Subject),
                new SqlParameter("@content",msg.Content),
                new SqlParameter("@status",msg.Status)
            };
            return SqlHelper.ExecuteCommand(sql, values);
        }

        //删除单个私信
        public static int DeletePrivateMessage(String userid,String messageid)
        {
            string sql = "delete from tb_Message_"+userid+" where MessageID="+messageid;
            return SqlHelper.ExecuteCommand(sql);
        }
#endregion

        #region 书籍测试
        //获取该书的全部问题
        public static List<Question> GetQuestion(String bookid)
        {
            string sql = "select * from tb_Question_" + bookid;
            SqlDataReader reader = SqlHelper.GetReader(sql);
            if (!reader.HasRows)
            {
                return null;
            }
            List<Question> questions = new List<Question>();
            while (reader.Read())
            {
                Question question = new Question
                {
                    QuestionId = reader["QuestionID"].ToString(),
                    Content = reader["Content"].ToString(),
                    answerA = reader["answerA"].ToString(),
                    answerB = reader["answerB"].ToString(),
                    answerC = reader["answerC"].ToString(),
                    answerD = reader["answerD"].ToString(),
                    answerE = reader["answerE"].ToString(),
                    Correct = reader["Correct"].ToString(),
                    Type =int.Parse(reader["Type"].ToString())
                };
                questions.Add(question);
            }
            reader.Close();
            return questions;
        }

        //添加单个问题
        public static int InsertQuestion(Question question,String bookid)
        {
            string sql = "insert into tb_Question_"+bookid +" values(@Content,";
            switch (question.Type) {
                case 0: sql += "@answerA,@answerB,@answerC,@answerD,NULL,@Correct,@Type)"; break;
                case 1: sql += "@answerA,@answerB,@answerC,@answerD,@answerE,@Correct,@Type)"; break;
                default: sql += "@answerA,NULL,NULL,NULL,NULL,@Correct,@Type)"; break;
            }
            SqlParameter[] value = new SqlParameter[]{
                new SqlParameter("@Content",question.Content),
                new SqlParameter("@answerA",question.answerA),
                new SqlParameter("@answerB",question.answerB),
                new SqlParameter("@answerC",question.answerC),
                new SqlParameter("@answerD",question.answerD),
                new SqlParameter("@answerE",question.answerE),
                new SqlParameter("@Correct",question.Correct),
                new SqlParameter("@Type",question.Type)
            };
            return SqlHelper.ExecuteCommand(sql, value);
        }

        //更新单个问题
        public static int UpdateQuestion(Question question,String bookid)
        {
            string sql = "UPDATE tb_Question_"+bookid+" SET Content=@Content,";
            switch (question.Type)
            {
                case 0: sql += "answerA=@answerA,answerB=@answerB,answerC=@answerC,answerD=@answerD,answerE='',Correct=@Correct,Type=@Type"; break;
                case 1: sql += "answerA=@answerA,answerB=@answerB,answerC=@answerC,answerD=@answerD,answerE=@answerE,Correct=@Correct,Type=@Type"; break;
                default: sql += "@answerA=@answerA,answerB='',answerC='',answerD='',answerE='',Correct=@Correct,Type=@Type"; break;
            }
            sql += " where QuestionID=@QuestionId";
            SqlParameter[] value = new SqlParameter[]{
                new SqlParameter("@Content",question.Content),
                new SqlParameter("@answerA",question.answerA),
                new SqlParameter("@answerB",question.answerB),
                new SqlParameter("@answerC",question.answerC),
                new SqlParameter("@answerD",question.answerD),
                new SqlParameter("@answerE",question.answerE),
                new SqlParameter("@Correct",question.Correct),
                new SqlParameter("@Type",question.Type),
                new SqlParameter("@QuestionId",question.QuestionId)
            };
            return SqlHelper.ExecuteCommand(sql, value);
        }

        //删除单个问题
        public static int DeleteQuestion(String questionid, String bookid)
        {
            string sql = "delete from tb_Question_"+bookid;
            sql += " where QuestionID=@QuestionId";
            SqlParameter value = new SqlParameter("@QuestionID", questionid);
            return SqlHelper.ExecuteCommand(sql, value);
        }
        #endregion

        public static int InsertGroup(Role role)
        {
            string sql = "insert into tb_Role values(@roleName,@deptType,@deptId,@rights)";
            SqlParameter[] value = new SqlParameter[]{
                new SqlParameter("@roleName",role.RoleName),
                new SqlParameter("@deptType",role.Department.DepartmentType.ToString()),
                new SqlParameter("@deptId",role.Department.DepartmentId),
                new SqlParameter("@rights",role.Rights),
            };
            return SqlHelper.ExecuteCommand(sql, value);
        }

        public static int DeleteGroup(Role role)
        {
            string sql_delUser = "delete from tb_User where RoleID = " + role.RoleId;
            string sql_delRole = "delete from tb_Role where RoleID = " + role.RoleId;
            int i = SqlHelper.ExecuteCommand(sql_delUser);
            i = SqlHelper.ExecuteCommand(sql_delRole);
            return i;
        }

        public static int UpdateGroup(Role role)
        {
            string sql = "update tb_Role set RoleName=@roleName,DepartmentType=@deptType,DepartmentID=@deptId,Rights=@rights where RoleID=@roleId";
            SqlParameter[] value = new SqlParameter[]{
                new SqlParameter("@roleName",role.RoleName),
                new SqlParameter("@deptType",role.Department.DepartmentType.ToString()),
                new SqlParameter("@deptId",role.Department.DepartmentId),
                new SqlParameter("@rights",role.Rights),
                new SqlParameter("@roleId",role.RoleId),
            };
            return SqlHelper.ExecuteCommand(sql, value);
        }
    }
}