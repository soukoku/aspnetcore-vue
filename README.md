# aspnetcore-vue

A sample aspnet project template with these features:

* asp.net core 2.1 for server-side code
* vue.js for client-side code (created with cli v3)
* both live in one project and debugging is done on the aspnet project
* working HMR in vue app when debugging the aspnet site

Below are the steps used to create this.


# Aspnet Core Project
Create a new dotnet core project with aspnet core template.

Then In `Startup.cs`, add
`services.AddSpaStaticFiles()` in `ConfigureServices()` method,
and `app.UseSpaStaticFiles()` and `app.UseSpa()` in `Configure()` method.


```cs
public void ConfigureServices(IServiceCollection services)
{
  services.AddMvc();

  // new addition here
  services.AddSpaStaticFiles(spa =>
  {
    spa.RootPath = @"ClientApp\dist";
  });
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
  // ... other aspnet configuration skipped here

  app.UseStaticFiles();
  app.UseSpaStaticFiles(); // new addition
  app.UseMvc();

  // new addition
  app.UseSpa(spa =>
  {
    if (env.IsDevelopment())
    {
      // change this to whatever webpack dev server says it's running on
      spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
    }
  });
}
```

# Vue Project
Create a client-side project using vue cli 3 
into a folder called ClientApp in the aspnet project folder.



# Csproj File
Some edits to the .csproj file are also needed for proper 
release/publish using dotnet.

The `PropertyGroup` defines a new variable `SpaRoot` for use later.

The `ItemGroup` makes vue's project folder visible in VS
but not include in build.

`DebugEnsureNodeEnv` target installs npm packages if necessary
on project builds.

`PublishRunWebpack` target builds the vue app and 
include the **dist** folder in the published files.

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <!-- ...default stuff skipped here... -->

  <PropertyGroup>
    <SpaRoot>ClientApp\</SpaRoot>
  </PropertyGroup>

  <ItemGroup>
    <Content Remove="$(SpaRoot)**" />
    <None Include="$(SpaRoot)**" Exclude="$(SpaRoot)dist\**" />
  </ItemGroup>
  
  <Target Name="DebugEnsureNodeEnv" BeforeTargets="Build" Condition=" '$(Configuration)' == 'Debug' And !Exists('$(SpaRoot)node_modules') ">
    <!-- Ensure Node.js is installed -->
    <Exec Command="node --version" ContinueOnError="true">
      <Output TaskParameter="ExitCode" PropertyName="ErrorCode" />
    </Exec>
    <Error Condition="'$(ErrorCode)' != '0'" Text="Node.js is required to build and run this project. To continue, please install Node.js from https://nodejs.org/, and then restart your command prompt or IDE." />
    <Message Importance="high" Text="Restoring dependencies using 'npm'. This may take several minutes..." />
    <Exec WorkingDirectory="$(SpaRoot)" Command="npm install" />
  </Target>

  <Target Name="PublishRunWebpack" AfterTargets="ComputeFilesToPublish">
    <!-- As part of publishing, ensure the JS resources are freshly built in production mode -->
    <Exec WorkingDirectory="$(SpaRoot)" Command="npm install" />
    <Exec WorkingDirectory="$(SpaRoot)" Command="npm run build" />

    <!-- Include the newly-built files in the publish output -->
    <ItemGroup>
      <DistFiles Include="$(SpaRoot)dist\**" />
      <ResolvedFileToPublish Include="@(DistFiles->'%(FullPath)')" Exclude="@(ResolvedFileToPublish)">
        <RelativePath>%(DistFiles.Identity)</RelativePath>
        <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
      </ResolvedFileToPublish>
    </ItemGroup>
  </Target>
</Project>

```
