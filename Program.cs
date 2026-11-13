
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FinancialFormulas.ServiceExtensions;
using FinancialFormulas.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add the financial service to the DI container
builder.Services.AddScoped<IFinancialService, FinancialService>();

// AddControllers() is a built-in method that registers the necessary
// services for controllers in an ASP.NET Core application.
// It sets up the MVC framework, enabling the application to
// handle HTTP requests and responses using controllers.
// This method is essential for building APIs or web applications
// that rely on the Model-View-Controller (MVC) pattern.
// It configures routing, model binding, validation,
// and other features required for controller-based request handling.
builder.Services.AddControllers();

// Add API versioning and configure the API explorer for versioned APIs.
builder.Services.ConfigureControllers();

// AddConfiguredMVCOptions() is a custom extension method that
// configures MVC options for the application. It allows you to
// customize various aspects of the MVC framework, such as model binding,
// validation, and formatting. This method is typically defined
// in a separate class (e.g., ServiceExtensions) and is used
// to centralize and organize the configuration of MVC options
// for better maintainability and readability of the code.
builder.Services.AddConfiguredMVCOptions();

// Add Kafka messaging services to the DI container using the provided configuration.
builder.Services.AddKafkaMessaging(builder.Configuration);

// provide more details for problems
builder.Services.AddProblemDetails();

// Configure API versioning
builder.Services.AddConfiguredVersioning();

// Configure Swagger/OpenAI
builder.Services.AddEndpointsApiExplorer();

// Add Swagger configuration from ServiceExtensions/SwaggerServiceExtensions.cs
builder.Services.AddConfiguredSwagger();


// build web application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Execute develoeprs error page
    //app.UseDeveloperExceptionPage();

    // Swager activation can be added here to be enabled 
    // under development settings. For overal use, set outside
}

// Let anyone access swagger for now
// Swagger activation
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // Disvover versionin within the application
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    // Swagger json file location per version
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }

    // Option 1: setting swagger to appear when accessing the root address
    // For example: https://localhost:7003/index.html
    //   Step1: Set inside the Properties/launchSettings.json
    //           profiles/http/launchUrl = ""
    //   Step 2: Set here the RoutePrefix parameter to empty
    //           c.RoutePrefix = string.Empty;

    // Option 2: setting swagger to appear when accessing the address with 
    //           key word 'swagger'
    // For example: https://localhost:7003/swagger/index.html
    //    Step 1: Set inside the Properties/launchSettings.json
    //            profiles/http/launchUrl = "swagger"
    //   Step 2: Set here the RoutePrefix parameter to "swagger"
    //           c.RoutePrefix = c.RoutePrefix = "swagger";

    options.RoutePrefix = "swagger"; // Set Swagger UI at app's root

    // Note: in the launchSettings.json, the  profiles/http/launchBrowser = true
    // configuration point, if you don't want the browser to open
    // automatically (e.g., for background services or APIs),
    // you can set launchBrowser to false. This is useful for scenarios
    // where you don't need a browser interface.

});

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

