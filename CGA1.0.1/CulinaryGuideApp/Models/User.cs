using SQLite;

namespace CulinaryGuideApp.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Username { get; set; }

        // 注意：这个 Password 属性仅用于接收注册时的原始密码，不会直接存储
        public string Password { get; set; }

        // 存储哈希后的密码
        public string PasswordHash { get; set; }

        // 存储用于哈希的盐
        public string Salt { get; set; }
    }
}