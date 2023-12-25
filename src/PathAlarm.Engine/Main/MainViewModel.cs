using System.Collections.ObjectModel;
using System.Windows.Input;
using MVVMBase.Commands;
using MVVMBase.Infrastructure;
using PathAlarm.Engine.Coordinates;
using PathAlarm.Engine.Notifications;
using PathAlarm.Engine.Permissions;
using PathAlarm.Engine.Shared;

namespace PathAlarm.Engine.Main;

public class MainViewModel : ViewModel
{
    private readonly IPermissionManager permissionManager;
    private readonly IForegroundService foregroundService;
    private readonly LocationChecker locationChecker;
    private readonly CoordinateNotificationManager coordinateNotificationManager;
    private readonly IDeviceNotificationService deviceNotificationService;
    private string currentCoordinateStatus = "";
    private string newCoordinateText = String.Empty;
    private string distance = "20";

    public MainViewModel(IPermissionManager permissionManager, IForegroundService foregroundService, LocationChecker locationChecker,
        CoordinateNotificationManager coordinateNotificationManager, IDeviceNotificationService deviceNotificationService)
    {
        this.permissionManager = permissionManager;
        this.foregroundService = foregroundService;
        this.locationChecker = locationChecker;
        this.coordinateNotificationManager = coordinateNotificationManager;
        this.deviceNotificationService = deviceNotificationService;
        StartServiceCommand = new SimpleCommand(StartService);
        StopServiceCommand = new SimpleCommand(StopService);
        AddCoordinateCommand = new SimpleCommand(AddCoordinate);
        RemovePointCommand = new ActionCommand<CoordinatePointViewModel?>(RemovePoint);
        ChangeDistanceCommand = new SimpleCommand(ChangeDistance);
    }
    
    private void RemovePoint(CoordinatePointViewModel? point)
    {
        if (point == null)
        {
            return;
        }
        Context.InvokeOnUiThread(() =>
        {
            CoordinatePoints.Remove(point);
        });
        coordinateNotificationManager.Remove();
    }

    public ICommand StartServiceCommand { get; }
    public ICommand StopServiceCommand { get; }

    public ICommand AddCoordinateCommand { get; }

    public ICommand RemovePointCommand { get; }
    public ICommand ChangeDistanceCommand { get; }

    public string NewCoordinateText
    {
        get => newCoordinateText;
        set
        {
            if (value == newCoordinateText)
            {
                return;
            }
            newCoordinateText = value;
            OnPropertyChanged();
        }
    }

    public string CurrentCoordinateStatus
    {
        get => currentCoordinateStatus;
        set
        {
            if (value == currentCoordinateStatus)
            {
                return;
            }
            currentCoordinateStatus = value;
            OnPropertyChanged();
        }
    }

    public string Distance
    {
        get => distance;
        set
        {
            if (value == distance)
            {
                return;
            }
            distance = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<CoordinatePointViewModel> CoordinatePoints { get; } = new();

    private void StopService()
    {
        foregroundService.Stop();
    }

    private async void StartService()
    {
        var granted = await permissionManager.GetPermission(PermissionType.Location);
        if (!granted)
        {
            return;
        }
        foregroundService.Start();
        locationChecker.Start();
        StartLocationCheck();
        CurrentCoordinateStatus = "Started!";
    }

    private void StartLocationCheck()
    {
        Task.Run(() =>
        {
            while (true)
            {
                var messageReceived = locationChecker.GetMessage();
                var call = coordinateNotificationManager.ShouldNotify(messageReceived, out var distance);
                if (call)
                {
                    deviceNotificationService.NotifyUser($"You are {distance} meter(s) away", "Path Alarm");
                }
                CurrentCoordinateStatus = $"Coordinates are {messageReceived}\nDistance is {distance}";
                Console.WriteLine(messageReceived);
            }
        });
    }


    private void AddCoordinate()
    {
        coordinateNotificationManager.Add(NewCoordinateText);
        if (!CoordinateNotificationManager.ExtractCoordinate(NewCoordinateText, out var gpsCoordinate))
        {
            return;
        }
        var pointViewModel = new CoordinatePointViewModel(gpsCoordinate);
        Context.InvokeOnUiThread(() =>
        {
            CoordinatePoints.Add(pointViewModel);
        });
    }

    private void ChangeDistance()
    {
        coordinateNotificationManager.UpdateDistance(Distance);
    }
}