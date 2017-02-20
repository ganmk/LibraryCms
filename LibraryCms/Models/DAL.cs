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
                Sex = reader["Sex"].ToString(),
            };
            string roleId = reader["RoleID"].ToString();
            reader.Close();
            user.Role = GetRoleById(roleId);

            return user;
        }

        public static List<Department> GetDepartment(string str)
        {
            string sql = "select * from tb_Department_B where DepartmentName like @name";
            SqlDataReader reader = SqlHelper.GetReader(sql, new SqlParameter("@name", "%" + str + "%"));
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
            reader = SqlHelper.GetReader(sql, new SqlParameter("@name", "%" + str + "%"));
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
            reader = SqlHelper.GetReader(sql, new SqlParameter("@name", "%" + str + "%"));
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

        public static List<Role> GetRole(string str)
        {
            string sql = "select * from tb_Role where RoleName like @name";
            SqlDataReader reader = SqlHelper.GetReader(sql, new SqlParameter("@name", "%" + str + "%"));
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
                Formart = reader["Formart"].ToString(),
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
    }
}