using System;
using System.Collections.Generic;
using System.Text.Json;
using SQLite;

namespace CulinaryGuideApp.Models
{
    public class Restaurant
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string ImagePathsJson { get; set; } // 存储图片路径的 JSON 字符串

        public List<string> ImagePaths
        {
            get
            {
                return string.IsNullOrEmpty(ImagePathsJson)
                    ? new List<string>() // 如果为空，则返回空的列表
                    : JsonSerializer.Deserialize<List<string>>(ImagePathsJson); // 否则解析 JSON
            }
        }
    }
}
