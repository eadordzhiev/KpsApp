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
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
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
    }
}
