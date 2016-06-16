using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;

namespace KpsApp
{
    public sealed partial class MainPage : Page
    {
        private Geolocator _geolocator;

        public MainPage()
        {
            this.InitializeComponent();
            
            mapControl.MapServiceToken = "4BEoUusiuTJmpkxoFp5G~DLYesDtCdhqlWnZGYDI5Aw~AqNBQSShEr89ei6Y-bKHjHqDSMXZpDdB04a3T9se9YBKwEs7bbFrGvlHPUfVAYxh";
            mapControl.TileSources.Add(new MapTileSource(new HttpMapTileDataSource("http://kpsserver.azurewebsites.net/api/map?zoomLevel={zoomlevel}&x={x}&y={y}")));

            directionsFromTextBox.ItemsSource = new[] { "Моё текущее расположение" };
            directionsToTextBox.ItemsSource = new[] { "Центр Здоровой Кожи" };

            OnLoad();
        }
        
        async void OnLoad()
        {
            var moscowPosition = new BasicGeoposition { Latitude = 55.751244, Longitude = 37.618423 };
            mapControl.Scene = MapScene.CreateFromLocationAndRadius(new Geopoint(moscowPosition), 25000);

            var accessStatus = await Geolocator.RequestAccessAsync();
            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                _geolocator = new Geolocator();
                _geolocator.PositionChanged += OnPositionChanged;
            }
        }

        private async void OnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 mePoiControl.Visibility = Visibility.Visible;
                 MapControl.SetLocation(mePoiControl, args.Position.Coordinate.Point);
             });
        }

        private async void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            await mapControl.TryZoomInAsync();
        }

        private async void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            await mapControl.TryZoomOutAsync();
        }

        private async void ShowMyLocationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_geolocator != null)
            {
                var position = await _geolocator.GetGeopositionAsync();
                await mapControl.TrySetSceneAsync(MapScene.CreateFromLocationAndRadius(position.Coordinate.Point, 5000));
            }
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            mapControl.Routes.Clear();
            directionsPanel.Visibility = directionsPanel.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void ShowRouteButtonClick(object sender, RoutedEventArgs e)
        {
            if (directionsFromTextBox.Text != "Моё текущее расположение"
                || directionsToTextBox.Text != "Центр Здоровой Кожи")
            {
                var dialog = new MessageDialog("Невозможно проложить маршрут.", "Ошибка");
                await dialog.ShowAsync();
                return;
            }

            mapControl.Routes.Clear();

            directionsPanel.Visibility = Visibility.Collapsed;

            // Start at Microsoft in Redmond, Washington.
            var startLocation = new BasicGeoposition() { Latitude = 55.837403, Longitude = 37.481816 };

            // End at the city of Seattle, Washington.
            BasicGeoposition endLocation = new BasicGeoposition() { Latitude = 55.788629, Longitude = 37.447972 };

            MapRouteFinderResult routeResult;
            if (isSafeCheckBox.IsChecked == true)
            {
                var p1 = new BasicGeoposition() { Latitude = 55.792096, Longitude = 37.509935 };
                var p2 = new BasicGeoposition() { Latitude = 55.777090, Longitude = 37.507182 };
                routeResult =
                    await
                        MapRouteFinder.GetWalkingRouteFromWaypointsAsync(new Geopoint[]
                        { new Geopoint(startLocation), new Geopoint(p1), new Geopoint(p2),  new Geopoint(endLocation), });
            }
            else
            {
                routeResult =
                  await MapRouteFinder.GetWalkingRouteAsync(
                  new Geopoint(startLocation),
                  new Geopoint(endLocation));
            }
            // Get the route between the points.
            

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = (Color) Resources["SystemAccentColor"];

                // Add the new MapRouteView to the Routes collection
                // of the MapControl.
                mapControl.Routes.Add(viewOfRoute);

                // Fit the MapControl to the route.
                await mapControl.TrySetViewBoundsAsync(
                      routeResult.Route.BoundingBox,
                      null,
                      Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
            }
        }

        private void DirectionsFromTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var s = (AutoSuggestBox) sender;
            s.IsSuggestionListOpen = true;
        }
    }
}
