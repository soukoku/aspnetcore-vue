# aspnetcore-vue

A sample project setup with these features:

* asp.net core 2.1 for server-side code
* vue.js for client-side code (created with cli v3)
* both live in one project, only one aspnet site to debug in VS
* working HMR when using debugging the aspnet site

Below are the mods to make this happen.

# Server-side mods

In Startup.cs, particularly the additions with
`UseWebpackDevMiddleware()` and `MapSpaFallbackRoute()` calls.

```cs
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
  if (env.IsDevelopment())
  {
    app.UseDeveloperExceptionPage();
    app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
    {
      HotModuleReplacement = true,
      ConfigFile = Path.Combine(env.ContentRootPath, @"node_modules\@vue\cli-service\webpack.config.js")
    });
  }

  app.UseStaticFiles();

  app.UseMvc(routes =>
  {
    routes.MapRoute(
      name: "default",
      template: "{controller=Home}/{action=Index}/{id?}");

    routes.MapSpaFallbackRoute(
      name: "spa-fallback",
      defaults: new { controller = "Home", action = "Index" });
  });
}
```

# Client-side mods

Install these dev dependencies.

```
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

If needs to use HMR in IE/Edge, copy the 
`node_modules/eventsource-polyfill/dist/eventsource.js` file to 
somewhere in the public folder. Modify `index.html` to include
it only during dev time.

```html
<% if (NODE_ENV === 'development') { %>
<script src="<%= BASE_URL %>js/eventsource.js"></script>
<% } %>
```


# CS project mods

These are needed for proper build/release using dotnet.

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