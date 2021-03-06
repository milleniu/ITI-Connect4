namespace ITI.Connect4

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open ITI.Connect4.Models
open ITI.Connect4.Services

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        services
            .AddMemoryCache()
            .AddSingleton<IGameManager, GameManager>()
            .AddSingleton<IPersistenceService, PersistenceService>()
            .AddSingleton<IViewModelConverter, ViewModelConverter>()
            .AddControllers()
        |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app
                .UseDeveloperExceptionPage()
            |> ignore

        app
            .UseRouting()
            .UseAuthorization()
            .UseDefaultFiles()
            .UseStaticFiles()
            .UseEndpoints(fun endpoints -> endpoints.MapControllers() |> ignore)
        |> ignore

    member val Configuration : IConfiguration = null with get, set
