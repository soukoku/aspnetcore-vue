# aspnetcore-vue

A sample aspnet project template with these features:

* asp.net core 2.1 for server-side code
* vue.js for client-side code (created with cli v3)
* both live in one project with only one aspnet site to debug in VS
* working HMR when debugging the aspnet site

Below are the mods to make this happen.

# Server-side mods

In `Startup.cs`, particularly the 
`services.AddSpaStaticFiles()` and `app.UseSpa()` calls
after what's normally there in an aspnet project template.

```cs
public void ConfigureServices(IServiceCollection services)
{
  services.AddMvc();
  services.AddSpaStaticFiles(config =>
  {
    config.RootPath = "dist";
  });
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
  // ... other aspnet configuration skipped here

  app.UseStaticFiles();
  app.UseMvc();

  app.UseSpa(config =>
  {
    if (env.IsDevelopment())
    {
      config.ApplicationBuilder.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
      {
        HotModuleReplacement = true,
        ConfigFile = Path.Combine(env.ContentRootPath, 
          @"node_modules\@vue\cli-service\webpack.config.js")
      });
      }
  });
}
```

# Client-side mods

Install these dev dependencies.

```bash
npm install -D aspnet-webpack webpack-hot-middleware eventsource-polyfill
```

Delete HMR plugin in `vue.config.js` to actually get HMR working.

```js
module.exports = {
  chainWebpack: config => {
    config.plugins.delete('hmr');
  }
};
```

If you needs to use HMR in IE/Edge, copy the 
`node_modules/eventsource-polyfill/dist/eventsource.js` file to 
somewhere in the public folder. Modify `index.html` to include
it only during dev time.

```html
<% if (NODE_ENV === 'development') { %>
<script src="<%= BASE_URL %>js/eventsource.js"></script>
<% } %>
```


# CS project mods

These are needed for proper release/publish using dotnet.
What this does is exclude the **/dist** folder from project but
still build the vue app and include it in published build.

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  ...blah blah...


  <ItemGroup>
    <Compile Remove="dist\**" />
    <Content Remove="dist\**" />
    <EmbeddedResource Remove="dist\**" />
    <None Remove="dist\**" />
  </ItemGroup>
  <Target Name="RunWebpack" AfterTargets="ComputeFilesToPublish">
    <!-- As part of publishing, ensure the JS resources are freshly built in production mode -->
    <!--<Exec Command="npm install" />-->
    <Exec Command="npm run build" />

    <!-- Include the newly-built files in the publish output -->
    <ItemGroup>
      <DistFiles Include="dist\**" />
      <ResolvedFileToPublish Include="@(DistFiles->'%(FullPath)')" Exclude="@(ResolvedFileToPublish)">
        <RelativePath>%(DistFiles.Identity)</RelativePath>
        <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
      </ResolvedFileToPublish>
    </ItemGroup>
  </Target>
</Project>
```
