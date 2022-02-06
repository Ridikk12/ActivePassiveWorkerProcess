using ActivePassive.Services.Interfaces;

namespace ActivePassive
{

    public class RegisterInstanceWorkerProcess : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        public static readonly string InstanceName =
            $"{nameof(RegisterInstanceWorkerProcess)}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        private IRegisterInstanceService? _registerInstanceService;

        private readonly IHostApplicationLifetime _applicationLifetime;

        public RegisterInstanceWorkerProcess(ILogger<Worker> logger, IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _applicationLifetime = applicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using var scope = _serviceProvider.CreateScope();
            _registerInstanceService = scope.ServiceProvider.GetService<IRegisterInstanceService>();
            ArgumentNullException.ThrowIfNull(_registerInstanceService);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _registerInstanceService.Register(InstanceName);
                    await _registerInstanceService.DeleteOrphanInstances();
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Critical error forced worker to shutdown");
                    _applicationLifetime.StopApplication();
                }
            }
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _registerInstanceService.UnRegister(InstanceName);
                _logger.LogInformation("UnRegister Instance {instanceName} running at: {time}.", InstanceName, DateTimeOffset.Now);
            }
            catch
            {
                _logger.LogError("Unable to UnRegister Instance {instanceName} running at: {time}.", InstanceName, DateTimeOffset.Now);
            }

            await base.StopAsync(cancellationToken);
        }
    }
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var registerInstanceService = scope.ServiceProvider.GetService<IRegisterInstanceService>();
                ArgumentNullException.ThrowIfNull(registerInstanceService);

                while (!stoppingToken.IsCancellationRequested)
                {
                    if (await (registerInstanceService.IsActive(RegisterInstanceWorkerProcess.InstanceName)))
                    {
                        await DoInActive(stoppingToken);
                    }
                    else
                    {
                        await DoInPassive(stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error forced worker to shutdown");
                _hostApplicationLifetime.StopApplication();
            }
        }

        private async Task DoInActive(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time} in {mode} mode", DateTimeOffset.Now, "Active");
            await Task.Delay(1000, stoppingToken);
        }

        private async Task DoInPassive(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time} in {mode} mode", DateTimeOffset.Now, "Passive");
            await Task.Delay(1000, stoppingToken);
        }
    }
}