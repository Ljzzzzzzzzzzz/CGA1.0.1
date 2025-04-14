using CulinaryGuideApp.Models;
using CulinaryGuideApp.Services;
using System.Collections.Generic;
using System.Text.Json;

namespace CulinaryGuideApp.Views
{
    public partial class RestaurantDetailPage : ContentPage
    {
        private Restaurant _restaurant;
        private DatabaseService _dbService = new();
        private List<string> _images;
        private List<Comment> _comments;

        public RestaurantDetailPage(Restaurant restaurant)
        {
            InitializeComponent();
            _restaurant = restaurant;  // 将传入的餐厅对象赋值给 _restaurant
            BindingContext = _restaurant;  // 设置绑定上下文为当前餐厅
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _dbService.Init();

            // 解析餐厅的图片路径
            _images = _dbService.ParseImagePaths(_restaurant.ImagePathsJson);

            // 获取餐厅的评论
            _comments = await _dbService.GetComments(_restaurant.Id);

            // 更新图片和评论的绑定
            ImageCollection.ItemsSource = _images;
            CommentList.ItemsSource = _comments;
        }

        private async void OnSubmitComment(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CommentEntry.Text))
            {
                var comment = new Comment
                {
                    RestaurantId = _restaurant.Id,
                    UserName = "Anonymous",  // 用户名可以根据需要获取
                    Text = CommentEntry.Text,
                    Date = DateTime.Now
                };

                // 保存评论到数据库
                await _dbService.AddComment(comment);
                CommentEntry.Text = string.Empty;  // 清空输入框

                // 刷新评论列表
                _comments = await _dbService.GetComments(_restaurant.Id);
                CommentList.ItemsSource = _comments;
            }
        }

        private async void OnAddPhoto(object sender, EventArgs e)
        {
            // 让用户选择照片
            FileResult photo = await MediaPicker.PickPhotoAsync();
            if (photo != null)
            {
                _images.Add(photo.FullPath);  // 添加图片路径到图片列表

                // 更新餐厅的图片
                await _dbService.UpdateRestaurantImages(_restaurant.Id, _images);
            }
        }
    }
}

                // 刷新
