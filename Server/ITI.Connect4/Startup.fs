namespace ITI.Connect4

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
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
            .AddSingleton<GameManagerDependency>({
                CreateNewBoardState = GameManager.createNewBoardState
            })
            .AddSingleton<PersistenceServiceDependency>({
                Get = PersistenceService.get
                Set = PersistenceService.set
            })
            .AddSingleton<ViewModelConverterDependency>({
                PlayerAsViewModel = ViewModelConverter.playerAsViewModel
                BoardStateAsViewModel = ViewModelConverter.boardStateAsViewModel
                PlayerFromViewModel = ViewModelConverter.playerFromViewModel
            })
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
            .UseEndpoints(fun endpoints -> endpoints.MapControllers() |> ignore)
        |> ignore

    member val Configuration : IConfiguration = null with get, set
