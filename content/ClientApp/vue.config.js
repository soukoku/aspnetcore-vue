module.exports = {
  chainWebpack: config => {
    // aspnet uses the other hmr so remove this one
    // see https://github.com/webpack/webpack/issues/1583
    //config.plugins.delete('hmr');
  }
};
