using CulinaryGuideApp.Models;
using CulinaryGuideApp.Services;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text.Json;
using CulinaryGuideApp.Views;

namespace CulinaryGuideApp.Views
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Restaurant> Restaurants { get; set; } = new();
        private DatabaseService _databaseService;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            _databaseService = new DatabaseService(); // 创建 DatabaseService 实例
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _databaseService.Init(); // 初始化数据库服务

            var restaurants = await _databaseService.GetRestaurants();
            Console.WriteLine($"Number of restaurants loaded: {restaurants?.Count}"); // 添加这行
            if (restaurants != null)
            {
                foreach (var restaurant in restaurants)
                {
                    Console.WriteLine($"Loaded restaurant: {restaurant.Name}"); // 输出每个餐厅的名称
                }
            }
            else
            {
                Console.WriteLine("Restaurants list is null.");
            }

            // 如果没有餐厅数据，则添加三个示例餐厅
            if (restaurants == null || !restaurants.Any())
            {
                var sampleRestaurants = new List<Restaurant>
                {
                    new Restaurant
                    {
                        Name = "Pizza Roma",
                        Location = "Rome, Italy",
                        Description = "Authentic wood-fired pizza from Italy. A cozy place for pizza lovers.",
                        ImagePathsJson = "[\"pizza.jpg\"]"
                    },
                    new Restaurant
                    {
                        Name = "Sushi Master",
                        Location = "Tokyo, Japan",
                        Description = "The best sushi in Tokyo with a view of Mount Fuji. Traditional sushi-making techniques.",
                        ImagePathsJson = "[\"sushi.jpg\"]"
                    },
                    new Restaurant
                    {
                        Name = "Burger King",
                        Location = "New York, USA",
                        Description = "Classic American burgers and fries. A casual dining experience.",
                        ImagePathsJson = "[\"burger.jpg\"]"
                    },
                    new Restaurant
                    {
                        Name = "Curry House",
                        Location = "Mumbai, India",
                        Description = "Spicy and flavorful Indian curries served with naan and rice. A paradise for spice lovers.",
                        ImagePathsJson = "[\"curry.jpg\"]"
                    },
                    new Restaurant
                    {
                        Name = "Le Bistrot",
                        Location = "Paris, France",
                        Description = "A charming French bistro offering exquisite wines and classic French cuisine.",
                        ImagePathsJson = "[\"le.jpg\"]"
                    }

                };
                // 插入这些示例餐厅
                foreach (var restaurant in sampleRestaurants)
                {
                    await _databaseService.AddRestaurant(restaurant);
                }

                // 再次获取餐厅数据
                restaurants = await _databaseService.GetRestaurants();
                Console.WriteLine($"Number of restaurants loaded (after insert): {restaurants?.Count}");
                if (restaurants != null)
                {
                    foreach (var restaurant in restaurants)
                    {
                        Console.WriteLine($"Loaded restaurant (after insert): {restaurant.Name}");
                    }
                }
                else
                {
                    Console.WriteLine("Restaurants list is null (after insert).");
                }
            }
            Restaurants.Clear();
            if (restaurants != null)
            {
                foreach (var restaurant in restaurants)
                {
                    Restaurants.Add(restaurant);
                }
            }
            // 设置餐厅列表项
            RestaurantList.ItemsSource = restaurants;
        }

        private async void RestaurantList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 获取选中的餐厅
            var selected = (Restaurant)e.CurrentSelection.FirstOrDefault();
            if (selected != null)
            {
                // 跳转到餐厅详情页面
                await Navigation.PushAsync(new RestaurantDetailPage(selected));
            }
        }
    }
}
